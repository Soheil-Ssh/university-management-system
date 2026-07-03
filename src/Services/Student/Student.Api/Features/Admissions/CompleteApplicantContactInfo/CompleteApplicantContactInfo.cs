using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Student.Api.Features.Admissions.CompleteApplicantContactInfo;

public static class CompleteApplicantContactInfo
{
    public sealed record ContactInfoRequest(
        string Mobile,
        string Phone,
        string Email,
        string Province,
        string City,
        string Street,
        string BuildingNumber,
        string PostalCode,
        string? Unit,
        string? AdditionalInfo);

    public sealed record ContactInfoResponse(
        Guid AdmissionRequestId,
        string TrackingCode,
        string RegistrationToken,
        AdmissionRequestStep CurrentStep);

    public sealed record Command(
        Guid AdmissionRequestId,
        string RegistrationToken,
        string Mobile,
        string Phone,
        string Email,
        string Province,
        string City,
        string Street,
        string BuildingNumber,
        string PostalCode,
        string? Unit,
        string? AdditionalInfo) : ICommand<Result<ContactInfoResponse>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AdmissionRequestId).NotEmpty();
            RuleFor(x => x.RegistrationToken).NotEmpty();
            RuleFor(x => x.Mobile).NotEmpty().Matches(@"^(?:\+98|98|0)?(9\d{9})$", RegexOptions.Compiled);
            RuleFor(x => x.Phone).NotEmpty().Matches(@"^(?:\+98|98|0)?([1-9]\d{1,2})(\d{8})$", RegexOptions.Compiled);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(256).EmailAddress();
            RuleFor(x => x.Province).NotEmpty().MaximumLength(50);
            RuleFor(x => x.City).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
            RuleFor(x => x.BuildingNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Unit).MaximumLength(20);
            RuleFor(x => x.AdditionalInfo).MaximumLength(500);
        }
    }

    public class Handler(
        IAdmissionRequestRepository admissionRequestRepository,
        IRegistrationTokenGenerator registrationTokenGenerator,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<ContactInfoResponse>>
    {
        public async Task<Result<ContactInfoResponse>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var admissionRequestId = new AdmissionRequestId(request.AdmissionRequestId);

            var admissionRequest = await admissionRequestRepository.GetByIdAsync(admissionRequestId, cancellationToken);

            if (admissionRequest is null)
                return AdmissionRequestErrors.NotFound;

            var contactInfoResult = ApplicantContactInfo.Create(
                request.Mobile,
                request.Phone,
                request.Email,
                request.Province,
                request.City,
                request.Street,
                request.BuildingNumber,
                request.PostalCode,
                request.Unit,
                request.AdditionalInfo);

            if (contactInfoResult.IsFailure)
                return contactInfoResult.Error;

            var nextRegistrationToken = registrationTokenGenerator.Generate();

            var completeContactInfoResult = admissionRequest.CompleteApplicantContactInfo(
                contactInfoResult.Data,
                request.RegistrationToken,
                nextRegistrationToken);

            if (completeContactInfoResult.IsFailure)
                return completeContactInfoResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return new ContactInfoResponse(
                admissionRequest.Id.Value,
                admissionRequest.TrackingCode.Value,
                admissionRequest.RegistrationToken,
                admissionRequest.Step);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/admission-requests/{admissionRequestId:guid}/applicant-contact-info",
                    async (ISender sender, Guid admissionRequestId, ContactInfoRequest request,
                        [FromHeader(Name = "X-Registration-Token")] string registrationToken) =>
                    {
                        var command = new Command(
                            admissionRequestId,
                            registrationToken,
                            request.Mobile,
                            request.Phone,
                            request.Email,
                            request.Province,
                            request.City,
                            request.Street,
                            request.BuildingNumber,
                            request.PostalCode,
                            request.Unit,
                            request.AdditionalInfo);

                        var result = await sender.Send(command);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("CompleteApplicantContactInfo")
                .WithTags("Admission Requests");
        }
    }
}