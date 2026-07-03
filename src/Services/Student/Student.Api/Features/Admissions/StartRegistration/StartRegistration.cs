namespace Student.Api.Features.Admissions.StartRegistration;

public static class StartRegistration
{
    public sealed record Response(Guid AdmissionRequestId, string TrackingCode, string RegistrationToken);

    public sealed record Command : ICommand<Result<Response>>;

    public class Handler(IAdmissionRequestRepository admissionRequestRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var nextSequenceNumber = await admissionRequestRepository
                .GetNextTrackingCodeSequenceAsync(DateTime.Now.Year, cancellationToken);

            var trackingCode = TrackingCode.Generate(nextSequenceNumber);

            var admissionRequestResult = AdmissionRequest.StartRegistration(trackingCode);

            if (admissionRequestResult.IsFailure)
                return admissionRequestResult.Error;

            var admissionRequest = admissionRequestResult.Data;

            await admissionRequestRepository.AddAsync(admissionRequest, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return new Response(admissionRequest.Id.Value,
                admissionRequest.TrackingCode.Value,
                admissionRequest.RegistrationToken);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/admission-requests/start-registration", async (ISender sender) => 
                { 
                    var result = await sender.Send(new Command());
                    return result.ToHttpResult();
                })
                .Version(app, 1.0)
                .WithName("StartAdmissionRegistration")
                .WithTags("Admission Requests");
        }
    }
}