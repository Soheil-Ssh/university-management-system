using Microsoft.AspNetCore.Mvc;

namespace Student.Api.Features.Admissions.v1.CompleteDiplomaInfo;

public static class CompleteDiplomaInfo
{
    public sealed record DiplomaInfoRequest(decimal Average, string Field, int GraduationYear);

    public sealed record Response(
        Guid AdmissionRequestId,
        string TrackingCode,
        string RegistrationToken,
        AdmissionRequestStep CurrentStep);

    public sealed record Command(
        Guid AdmissionRequestId,
        string RegistrationToken,
        decimal Average,
        string Field,
        int GraduationYear) : ICommand<Result<Response>>;

    public class Validator : AbstractValidator<Command>
    {
        private const decimal MinAverage = 10m;
        private const decimal MaxAverage = 20m;
        private const int MinGraduationYear = 1950;
        private const int MaxFieldLength = 200;

        public Validator()
        {
            RuleFor(x => x.AdmissionRequestId).NotEmpty();
            RuleFor(x => x.RegistrationToken).NotEmpty();
            RuleFor(x => x.Average).InclusiveBetween(MinAverage, MaxAverage);
            RuleFor(x => x.Field).NotEmpty().MaximumLength(MaxFieldLength);
            RuleFor(x => x.GraduationYear).GreaterThanOrEqualTo(MinGraduationYear).LessThanOrEqualTo(DateTime.UtcNow.Year);
        }
    }

    public class Handler(
        IAdmissionRequestRepository admissionRequestRepository,
        IRegistrationTokenGenerator registrationTokenGenerator,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            Command request,
            CancellationToken cancellationToken)
        {
            var admissionRequestId = new AdmissionRequestId(request.AdmissionRequestId);

            var admissionRequest = await admissionRequestRepository.GetByIdAsync(
                admissionRequestId,
                cancellationToken);

            if (admissionRequest is null)
                return AdmissionRequestErrors.NotFound;

            var diplomaInfoResult = DiplomaInfo.Create(
                request.Average,
                request.Field,
                request.GraduationYear);

            if (diplomaInfoResult.IsFailure)
                return diplomaInfoResult.Error;

            var nextRegistrationToken = registrationTokenGenerator.Generate();

            var completeDiplomaInfoResult = admissionRequest.CompleteDiplomaInfo(diplomaInfoResult.Data,
                request.RegistrationToken,
                nextRegistrationToken);

            if (completeDiplomaInfoResult.IsFailure)
                return completeDiplomaInfoResult.Error;

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
            app.MapPut("api/v{v:apiVersion}/admission-registration/{admissionRequestId:guid}/diploma-info",
                    async (ISender sender, Guid admissionRequestId, DiplomaInfoRequest request,
                        [FromHeader(Name = "X-Registration-Token")] string registrationToken) =>
                    {
                        var command = new Command(
                            admissionRequestId,
                            registrationToken,
                            request.Average,
                            request.Field,
                            request.GraduationYear);

                        var result = await sender.Send(command);

                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("CompleteDiplomaInfo")
                .WithTags("Admission Requests");
        }
    }
}