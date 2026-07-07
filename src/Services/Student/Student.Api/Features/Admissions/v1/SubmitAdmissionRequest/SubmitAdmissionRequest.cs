using Microsoft.AspNetCore.Mvc;

namespace Student.Api.Features.Admissions.v1.SubmitAdmissionRequest;

public static class SubmitAdmissionRequest
{
    public sealed record Response(
        Guid AdmissionRequestId,
        string TrackingCode,
        string RegistrationToken,
        AdmissionRequestStatus Status,
        AdmissionRequestStep Step);

    public sealed record Command(Guid AdmissionRequestId,
        string RegistrationToken) : ICommand<Result<Response>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AdmissionRequestId).NotEmpty();
            RuleFor(x => x.RegistrationToken).NotEmpty();
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

            var nextRegistrationToken = registrationTokenGenerator.Generate();

            var submitResult = admissionRequest.Submit(request.RegistrationToken, nextRegistrationToken);

            if (submitResult.IsFailure)
                return submitResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return new Response(admissionRequest.Id.Value,
                admissionRequest.TrackingCode.Value,
                admissionRequest.RegistrationToken,
                admissionRequest.Status,
                admissionRequest.Step);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/admission-registration/{admissionRequestId:guid}/submit",
                    async (ISender sender, Guid admissionRequestId, [FromHeader(Name = "X-Registration-Token")] string registrationToken) =>
                    {
                        var command = new Command(admissionRequestId, registrationToken);
                        var result = await sender.Send(command);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("SubmitAdmissionRequest")
                .WithTags("Admission Requests");
        }
    }
}