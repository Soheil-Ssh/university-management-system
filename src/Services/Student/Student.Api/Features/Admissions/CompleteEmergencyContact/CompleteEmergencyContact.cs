using Microsoft.AspNetCore.Mvc;

namespace Student.Api.Features.Admissions.CompleteEmergencyContact;

public static class CompleteEmergencyContact
{
    public sealed record EmergencyContactRequest(string FirstName, string LastName, string Relation, string Mobile);

    public sealed record Response(
        Guid AdmissionRequestId,
        string TrackingCode,
        string RegistrationToken,
        AdmissionRequestStep CurrentStep);

    public sealed record Command(
        Guid AdmissionRequestId,
        string RegistrationToken,
        string FirstName,
        string LastName,
        string Relation,
        string Mobile) : ICommand<Result<Response>>;

    public class Validator : AbstractValidator<Command>
    {
        private const string PersianNamePattern = @"^[آابپتثجچحخدذرزژسشصضطظعغفقکگلمنوهیيىكۀةؤئإأء\s‌'-]+$";
        private const string IranianMobilePattern = @"^(09\d{9}|\+989\d{9}|00989\d{9})$";
        
        public Validator()
        {
            RuleFor(x => x.AdmissionRequestId).NotEmpty();
            RuleFor(x => x.RegistrationToken).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100).Matches(PersianNamePattern);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100).Matches(PersianNamePattern);
            RuleFor(x => x.Relation).NotEmpty().MaximumLength(100).Matches(PersianNamePattern);
            RuleFor(x => x.Mobile).NotEmpty().Matches(IranianMobilePattern);
        }
    }

    public class Handler(IAdmissionRequestRepository admissionRequestRepository,
        IRegistrationTokenGenerator registrationTokenGenerator,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request,
            CancellationToken cancellationToken)
        {
            var admissionRequestId = new AdmissionRequestId(request.AdmissionRequestId);

            var admissionRequest = await admissionRequestRepository.GetByIdAsync(admissionRequestId, cancellationToken);

            if (admissionRequest is null)
                return AdmissionRequestErrors.NotFound;

            var emergencyContactResult = EmergencyContact.Create(
                request.FirstName,
                request.LastName,
                request.Relation,
                request.Mobile);

            if (emergencyContactResult.IsFailure)
                return emergencyContactResult.Error;

            var nextRegistrationToken = registrationTokenGenerator.Generate();

            var completeEmergencyContactResult = admissionRequest.CompleteEmergencyContact(
                emergencyContactResult.Data,
                request.RegistrationToken,
                nextRegistrationToken);

            if (completeEmergencyContactResult.IsFailure)
                return completeEmergencyContactResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return new Response(
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
            app.MapPut("api/v{v:apiVersion}/admission-registration/{admissionRequestId:guid}/emergency-contact",
                    async (ISender sender, Guid admissionRequestId, EmergencyContactRequest request,
                        [FromHeader(Name = "X-Registration-Token")] string registrationToken) =>
                    {
                        var command = new Command(
                            admissionRequestId,
                            registrationToken,
                            request.FirstName,
                            request.LastName,
                            request.Relation,
                            request.Mobile);

                        var result = await sender.Send(command);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("CompleteEmergencyContact")
                .WithTags("Admission Requests");
        }
    }
}