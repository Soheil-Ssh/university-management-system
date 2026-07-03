using Microsoft.AspNetCore.Mvc;
using SharedKernel.Domain.Identifiers;

namespace Student.Api.Features.Admissions.CompleteApplicantPersonalInfo;

public static class CompleteApplicantPersonalInfo
{
    public sealed record Request(string FirstName,
        string LastName,
        string EnFirstName,
        string EnLastName,
        string NationalCode,
        string BirthPlace,
        string IssuePlace,
        DateTime BirthDate,
        Gender Gender,
        MaritalStatus MaritalStatus,
        Guid PersonalImageFileId);

    public sealed record Command(
        Guid AdmissionRequestId,
        string RegistrationToken,
        string FirstName,
        string LastName,
        string EnFirstName,
        string EnLastName,
        string NationalCode,
        string BirthPlace,
        string IssuePlace,
        DateTime BirthDate,
        Gender Gender,
        MaritalStatus MaritalStatus,
        Guid PersonalImageFileId) : ICommand<Result<Response>>;

    public sealed record Response(
        Guid AdmissionRequestId,
        string TrackingCode,
        string RegistrationToken,
        AdmissionRequestStep CurrentStep);

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AdmissionRequestId).NotEmpty();

            RuleFor(x => x.RegistrationToken).NotEmpty();

            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.EnFirstName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.EnLastName).NotEmpty().MaximumLength(100);

            RuleFor(x => x.NationalCode).NotEmpty().MaximumLength(10);

            RuleFor(x => x.BirthPlace).NotEmpty().MaximumLength(100);

            RuleFor(x => x.IssuePlace).NotEmpty().MaximumLength(100);

            RuleFor(x => x.BirthDate).NotEmpty();

            RuleFor(x => x.PersonalImageFileId).NotEmpty();
        }
    }

    public class Handler(IAdmissionRequestRepository admissionRequestRepository,
        IRegistrationTokenGenerator registrationTokenGenerator,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var admissionRequestId = new AdmissionRequestId(request.AdmissionRequestId);

            var admissionRequest = await admissionRequestRepository.GetByIdAsync(admissionRequestId, cancellationToken);
            if (admissionRequest is null)
                return AdmissionRequestErrors.NotFound;

            var personalImageFileId = new FileId(request.PersonalImageFileId);
            var personalInfoResult = ApplicantPersonalInfo.Create(
                request.FirstName,
                request.LastName,
                request.EnFirstName,
                request.EnLastName,
                request.NationalCode,
                request.BirthPlace,
                request.IssuePlace,
                request.BirthDate,
                request.Gender,
                request.MaritalStatus,
                personalImageFileId);

            if (personalInfoResult.IsFailure)
                return personalInfoResult.Error;

            var nextRegistrationToken = registrationTokenGenerator.Generate();

            var savePersonalInfoResult = admissionRequest.SaveApplicantPersonalInfo(personalInfoResult.Data,
                request.RegistrationToken,
                nextRegistrationToken);

            if (savePersonalInfoResult.IsFailure)
                return savePersonalInfoResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return new Response(admissionRequest.Id.Value,
                admissionRequest.TrackingCode.Value,
                admissionRequest.RegistrationToken,
                admissionRequest.Step);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("api/v{v:apiVersion}/admission-requests/{admissionRequestId:guid}/applicant-personal-info",
                    async (ISender sender, Guid admissionRequestId, Request request,
                        [FromHeader(Name = "X-Registration-Token")] string registrationToken) =>
                    {
                        var command = new Command(
                            admissionRequestId,
                            registrationToken,
                            request.FirstName,
                            request.LastName,
                            request.EnFirstName,
                            request.EnLastName,
                            request.NationalCode,
                            request.BirthPlace,
                            request.IssuePlace,
                            request.BirthDate,
                            request.Gender,
                            request.MaritalStatus,
                            request.PersonalImageFileId);

                        var result = await sender.Send(command);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("SaveApplicantPersonalInfo")
                .WithTags("Admission Requests");
        }
    }
}