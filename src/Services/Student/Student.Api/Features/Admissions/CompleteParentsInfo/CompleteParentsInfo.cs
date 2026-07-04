using Microsoft.AspNetCore.Mvc;

namespace Student.Api.Features.Admissions.CompleteParentsInfo;

public static class CompleteParentsInfo
{
    public sealed record ParentsInfoRequest(
        string FatherFirstName,
        string FatherLastName,
        string FatherNationalCode,
        string FatherMobile,
        string MotherFirstName,
        string MotherLastName,
        string MotherNationalCode,
        string MotherMobile);

    public sealed record Response(
        Guid AdmissionRequestId,
        string TrackingCode,
        string RegistrationToken,
        AdmissionRequestStep CurrentStep);

    public sealed record Command(
        Guid AdmissionRequestId,
        string RegistrationToken,
        string FatherFirstName,
        string FatherLastName,
        string FatherNationalCode,
        string FatherMobile,
        string MotherFirstName,
        string MotherLastName,
        string MotherNationalCode,
        string MotherMobile) : ICommand<Result<Response>>;

    public class Validator : AbstractValidator<Command>
    {
        private const string PersianNamePattern = @"^[آابپتثجچحخدذرزژسشصضطظعغفقکگلمنوهیيىكۀةؤئإأء\s‌'-]+$";
        private const string NationalCodePattern = @"^\d{10}$";
        private const string IranianMobilePattern = @"^(09\d{9}|\+989\d{9}|00989\d{9})$";

        public Validator()
        {
            RuleFor(x => x.AdmissionRequestId).NotEmpty();
            RuleFor(x => x.RegistrationToken).NotEmpty();
            RuleFor(x => x.FatherFirstName).NotEmpty().MaximumLength(100).Matches(PersianNamePattern);
            RuleFor(x => x.FatherLastName).NotEmpty().MaximumLength(100).Matches(PersianNamePattern);
            RuleFor(x => x.FatherNationalCode).NotEmpty().Matches(NationalCodePattern);
            RuleFor(x => x.FatherMobile).NotEmpty().Matches(IranianMobilePattern);
            RuleFor(x => x.MotherFirstName).NotEmpty().MaximumLength(100).Matches(PersianNamePattern);
            RuleFor(x => x.MotherLastName).NotEmpty().MaximumLength(100).Matches(PersianNamePattern);
            RuleFor(x => x.MotherNationalCode).NotEmpty().Matches(NationalCodePattern);
            RuleFor(x => x.MotherMobile).NotEmpty().Matches(IranianMobilePattern);
            RuleFor(x => x).Must(x => x.FatherNationalCode != x.MotherNationalCode).WithMessage("Father and mother national codes must be different.");
        }
    }

    public class Handler(
        IAdmissionRequestRepository admissionRequestRepository,
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

            var fatherInfoResult = ParentInfo.Create(
                request.FatherFirstName,
                request.FatherLastName,
                request.FatherNationalCode,
                request.FatherMobile);

            if (fatherInfoResult.IsFailure)
                return fatherInfoResult.Error.WithPath("Father");

            var motherInfoResult = ParentInfo.Create(
                request.MotherFirstName,
                request.MotherLastName,
                request.MotherNationalCode,
                request.MotherMobile);

            if (motherInfoResult.IsFailure)
                return motherInfoResult.Error.WithPath("Mother");

            var nextRegistrationToken = registrationTokenGenerator.Generate();

            var completeParentsInfoResult = admissionRequest.CompleteParentsInfo(
                fatherInfoResult.Data,
                motherInfoResult.Data,
                request.RegistrationToken,
                nextRegistrationToken);

            if (completeParentsInfoResult.IsFailure)
                return completeParentsInfoResult.Error;

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
                    "api/v{v:apiVersion}/admission-registration/{admissionRequestId:guid}/parents-info",
                    async (ISender sender, Guid admissionRequestId, ParentsInfoRequest request,
                        [FromHeader(Name = "X-Registration-Token")] string registrationToken) =>
                    {
                        var command = new Command(
                            admissionRequestId,
                            registrationToken,
                            request.FatherFirstName,
                            request.FatherLastName,
                            request.FatherNationalCode,
                            request.FatherMobile,
                            request.MotherFirstName,
                            request.MotherLastName,
                            request.MotherNationalCode,
                            request.MotherMobile);

                        var result = await sender.Send(command);

                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("CompleteParentsInfo")
                .WithTags("Admission Requests");
        }
    }
}