using Microsoft.AspNetCore.Mvc;

namespace Student.Api.Features.Admissions.CompleteEntranceInfo;

public static class CompleteEntranceInfo
{
    public sealed record EntranceInfoRequest(
        Quota Quota,
        int? EntranceExamRank,
        double? EntranceScore,
        AdmissionMethod AdmissionMethod,
        AdmissionType AdmissionType);

    public sealed record Response(
        Guid AdmissionRequestId,
        string TrackingCode,
        string RegistrationToken,
        AdmissionRequestStep CurrentStep);

    public sealed record Command(
        Guid AdmissionRequestId,
        string RegistrationToken,
        Quota Quota,
        int? EntranceExamRank,
        double? EntranceScore,
        AdmissionMethod AdmissionMethod,
        AdmissionType AdmissionType) : ICommand<Result<Response>>;

    public class Validator : AbstractValidator<Command>
    {
        private const double MinEntranceScore = 0;
        private const double MaxEntranceScore = 13000;

        public Validator()
        {
            RuleFor(x => x.AdmissionRequestId)
                .NotEmpty();

            RuleFor(x => x.RegistrationToken)
                .NotEmpty();

            RuleFor(x => x.Quota)
                .IsInEnum()
                .WithMessage("Quota is invalid.");

            RuleFor(x => x.AdmissionMethod)
                .IsInEnum()
                .WithMessage("Admission method is invalid.");

            RuleFor(x => x.AdmissionType)
                .IsInEnum()
                .WithMessage("Admission type is invalid.");

            RuleFor(x => x.EntranceExamRank)
                .GreaterThan(0)
                .When(x => x.EntranceExamRank.HasValue)
                .WithMessage("Entrance exam rank must be greater than zero.");

            RuleFor(x => x.EntranceScore)
                .InclusiveBetween(MinEntranceScore, MaxEntranceScore)
                .When(x => x.EntranceScore.HasValue)
                .WithMessage("Entrance score must be between 0 and 13000.");

            When(x => x.AdmissionMethod == AdmissionMethod.NationalExam, () =>
            {
                RuleFor(x => x.EntranceExamRank)
                    .NotNull()
                    .WithMessage("Entrance exam rank is required for national exam admission.");

                RuleFor(x => x.EntranceScore)
                    .NotNull()
                    .WithMessage("Entrance score is required for national exam admission.");

                RuleFor(x => x.AdmissionType)
                    .Must(IsValidDomesticAdmissionType)
                    .WithMessage("Admission type is invalid for national exam admission.");
            });

            When(x => x.AdmissionMethod == AdmissionMethod.AcademicRecord, () =>
            {
                RuleFor(x => x.EntranceExamRank)
                    .Null()
                    .WithMessage("Entrance exam rank must be empty for academic record admission.");

                RuleFor(x => x.EntranceScore)
                    .NotNull()
                    .WithMessage("Entrance score is required for academic record admission.");

                RuleFor(x => x.AdmissionType)
                    .Must(IsValidDomesticAdmissionType)
                    .WithMessage("Admission type is invalid for academic record admission.");
            });

            When(x => x.AdmissionMethod == AdmissionMethod.InternationalScholarship, () =>
            {
                RuleFor(x => x.EntranceExamRank)
                    .Null()
                    .WithMessage("Entrance exam rank must be empty for international scholarship admission.");

                RuleFor(x => x.Quota)
                    .Equal(Quota.Free)
                    .WithMessage("Quota must be free for international scholarship admission.");

                RuleFor(x => x.AdmissionType)
                    .Equal(AdmissionType.International)
                    .WithMessage("Admission type must be international.");
            });

            RuleFor(x => x)
                .Must(x => !IsRegionalQuota(x.Quota) ||
                           x.AdmissionMethod == AdmissionMethod.NationalExam)
                .WithMessage("Regional quota is only allowed for national exam admission.");
        }

        private static bool IsValidDomesticAdmissionType(AdmissionType admissionType)
        {
            return admissionType is AdmissionType.Daytime
                or AdmissionType.Nighttime;
        }

        private static bool IsRegionalQuota(Quota quota)
        {
            return quota is Quota.Region1
                or Quota.Region2
                or Quota.Region3;
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

            var entranceInfoResult = EntranceInfo.Create(
                request.Quota,
                request.EntranceExamRank,
                request.EntranceScore,
                request.AdmissionMethod,
                request.AdmissionType);

            if (entranceInfoResult.IsFailure)
                return entranceInfoResult.Error;

            var nextRegistrationToken = registrationTokenGenerator.Generate();

            var completeEntranceInfoResult = admissionRequest.CompleteEntranceInfo(
                entranceInfoResult.Data,
                request.RegistrationToken,
                nextRegistrationToken);

            if (completeEntranceInfoResult.IsFailure)
                return completeEntranceInfoResult.Error;

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
            app.MapPut(
                    "api/v{v:apiVersion}/admission-registration/{admissionRequestId:guid}/entrance-info",
                    async (ISender sender, Guid admissionRequestId, EntranceInfoRequest request,
                        [FromHeader(Name = "X-Registration-Token")] string registrationToken) =>
                    {
                        var command = new Command(
                            admissionRequestId,
                            registrationToken,
                            request.Quota,
                            request.EntranceExamRank,
                            request.EntranceScore,
                            request.AdmissionMethod,
                            request.AdmissionType);

                        var result = await sender.Send(command);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("CompleteEntranceInfo")
                .WithTags("Admission Requests");
        }
    }
}