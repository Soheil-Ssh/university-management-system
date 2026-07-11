namespace Faculty.Api.Features.Professors.v1.Create;

public static class Create
{
    public sealed record CreateProfessorRequest(
        string FirstName,
        string LastName,
        string FatherName,
        string NationalCode,
        string Email,
        string MobileNumber,
        string Specialization,
        AcademicRank AcademicRank,
        ProfessorEmploymentType EmploymentType,
        DateOnly EmploymentStartDate,
        Guid? ProfileImageFileId);

    public sealed record Command(
        string FirstName,
        string LastName,
        string FatherName,
        string NationalCode,
        string Email,
        string MobileNumber,
        string Specialization,
        AcademicRank AcademicRank,
        ProfessorEmploymentType EmploymentType,
        DateOnly EmploymentStartDate,
        Guid? ProfileImageFileId) : ICommand<Result<Guid>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.FatherName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NationalCode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(320).EmailAddress();
            RuleFor(x => x.MobileNumber).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Specialization).NotEmpty().MaximumLength(150);
            RuleFor(x => x.AcademicRank).IsInEnum();
            RuleFor(x => x.EmploymentType).IsInEnum();
            RuleFor(x => x.EmploymentStartDate).NotEmpty();

            RuleFor(x => x.ProfileImageFileId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Profile image file id is invalid.");
        }
    }

    public sealed class Handler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            // National code exists check
            var nationalCodeResult = NationalCode.Create(request.NationalCode).WithPath(nameof(NationalCode));
            if (nationalCodeResult.IsFailure)
                return nationalCodeResult.Error;

            if (await professorRepository.ExistsByNationalCodeAsync(nationalCodeResult.Data, cancellationToken))
                return ProfessorErrors.NationalCodeAlreadyExists;

            // Email exists check
            var emailResult = Email.Create(request.Email).WithPath(nameof(Email));
            if (emailResult.IsFailure)
                return emailResult.Error;

            if (await professorRepository.ExistsByEmailAsync(emailResult.Data, cancellationToken))
                return ProfessorErrors.EmailAlreadyExists;

            // Mobile number exists check
            var mobileNumberResult = MobileNumber.Create(request.MobileNumber).WithPath(nameof(MobileNumber));
            if (mobileNumberResult.IsFailure)
                return mobileNumberResult.Error;

            if (await professorRepository.ExistsByMobileNumberAsync(mobileNumberResult.Data, cancellationToken))
                return ProfessorErrors.MobileNumberAlreadyExists;

            // Professor code generation
            var nextCodeNumber = await professorRepository.GetNextCodeNumberAsync(cancellationToken);
            var codeResult = ProfessorCode.Create(nextCodeNumber);
            if (codeResult.IsFailure)
                return codeResult.Error;

            // Create professor
            var professorResult = Professor.Create(
                codeResult.Data,
                request.FirstName,
                request.LastName,
                request.FatherName,
                nationalCodeResult.Data,
                emailResult.Data,
                mobileNumberResult.Data,
                request.Specialization,
                request.AcademicRank,
                request.EmploymentType,
                request.EmploymentStartDate,
                request.ProfileImageFileId);

            if (professorResult.IsFailure)
                return professorResult.Error;

            await professorRepository.AddAsync(professorResult.Data, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return professorResult.Data.Id.Value;
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/professors", async (CreateProfessorRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = request.Adapt<Command>();
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.ProfessorsCreate)
                .Version(app, 1.0)
                .WithName("CreateProfessor")
                .WithTags("Professors");
        }
    }
}