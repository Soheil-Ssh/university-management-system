using Faculty.Api.Domain.Professor.Events;

namespace Faculty.Api.Domain.Professor;

public sealed class Professor : AggregateRoot<ProfessorId>
{
    private const int SpecializationMaxLength = 150;
    private const int IdentityProvisioningFailureReasonMaxLength = 1000;

    public ProfessorCode Code { get; private set; }
    public Name FirstName { get; private set; }
    public Name LastName { get; private set; }
    public Name FatherName { get; private set; }
    public NationalCode NationalCode { get; private set; }
    public Email Email { get; private set; }
    public MobileNumber MobileNumber { get; private set; }
    public string Specialization { get; private set; }
    public AcademicRank AcademicRank { get; private set; }
    public ProfessorEmploymentType EmploymentType { get; private set; }
    public DateOnly EmploymentStartDate { get; private set; }
    public FileId? ProfileImageFileId { get; private set; }
    public UserId? IdentityUserId { get; private set; }
    public IdentityProvisioningStatus IdentityProvisioningStatus { get; private set; }
    public string? IdentityProvisioningFailureReason { get; private set; }
    public bool IsActive { get; private set; }
    public string FullName => $"{FirstName.Value} {LastName.Value}";

#pragma warning disable CS8618
    private Professor() { }
#pragma warning restore CS8618

    private Professor(ProfessorId id,
        ProfessorCode code,
        Name firstName,
        Name lastName,
        Name fatherName,
        NationalCode nationalCode,
        Email email,
        MobileNumber mobileNumber,
        string specialization,
        AcademicRank academicRank,
        ProfessorEmploymentType employmentType,
        DateOnly employmentStartDate,
        FileId? profileImageFileId) : base(id)
    {
        Code = code;
        FirstName = firstName;
        LastName = lastName;
        FatherName = fatherName;
        NationalCode = nationalCode;
        Email = email;
        MobileNumber = mobileNumber;
        Specialization = specialization;
        AcademicRank = academicRank;
        EmploymentType = employmentType;
        EmploymentStartDate = employmentStartDate;
        ProfileImageFileId = profileImageFileId;

        IsActive = false;
        IdentityUserId = null;
        IdentityProvisioningStatus = IdentityProvisioningStatus.Pending;
        IdentityProvisioningFailureReason = null;

        AddDomainEvent(new ProfessorIdentityProvisioningRequestedDomainEvent(
            ProfessorId: id.Value,
            ProfessorCode: Code.Value,
            NationalCode: NationalCode.Value,
            FirstName: FirstName.Value,
            LastName: LastName.Value,
            Email: Email.Value,
            MobileNumber: MobileNumber.Value));
    }

    public static Result<Professor> Create(ProfessorCode code,
        string firstName,
        string lastName,
        string fatherName,
        NationalCode nationalCode,
        Email email,
        MobileNumber mobileNumber,
        string specialization,
        AcademicRank academicRank,
        ProfessorEmploymentType employmentType,
        DateOnly employmentStartDate,
        Guid? profileImageFileId)
    {
        var firstNameResult = Name.Create(firstName).WithPath(nameof(FirstName));
        if (firstNameResult.IsFailure)
            return firstNameResult.Error;

        var lastNameResult = Name.Create(lastName).WithPath(nameof(LastName));
        if (lastNameResult.IsFailure)
            return lastNameResult.Error;

        var fatherNameResult = Name.Create(fatherName).WithPath(nameof(FatherName));
        if (fatherNameResult.IsFailure)
            return fatherNameResult.Error;

        if (string.IsNullOrWhiteSpace(specialization))
            return ProfessorErrors.SpecializationEmpty;

        specialization = specialization.Trim();
        if (specialization.Length > SpecializationMaxLength)
            return ProfessorErrors.SpecializationTooLong;

        if (!Enum.IsDefined(typeof(AcademicRank), academicRank))
            return ProfessorErrors.AcademicRankInvalid;

        if (!Enum.IsDefined(typeof(ProfessorEmploymentType), employmentType))
            return ProfessorErrors.EmploymentTypeInvalid;

        if (employmentStartDate == default)
            return ProfessorErrors.EmploymentStartDateInvalid;

        FileId? profileFileId = null;

        if (profileImageFileId.HasValue)
        {
            if (profileImageFileId.Value == Guid.Empty)
                return ProfessorErrors.ProfileImageFileIdInvalid;

            profileFileId = new FileId(profileImageFileId.Value);
        }

        return new Professor(ProfessorId.New(),
            code,
            firstNameResult.Data,
            lastNameResult.Data,
            fatherNameResult.Data,
            nationalCode,
            email,
            mobileNumber,
            specialization,
            academicRank,
            employmentType,
            employmentStartDate,
            profileFileId);
    }

    public Result Update(string firstName,
        string lastName,
        string fatherName,
        Email email,
        MobileNumber mobileNumber,
        string specialization,
        AcademicRank academicRank,
        ProfessorEmploymentType employmentType,
        DateOnly employmentStartDate)
    {
        var firstNameResult = Name.Create(firstName).WithPath(nameof(FirstName));
        if (firstNameResult.IsFailure)
            return firstNameResult.Error;

        var lastNameResult = Name.Create(lastName).WithPath(nameof(LastName));
        if (lastNameResult.IsFailure)
            return lastNameResult.Error;

        var fatherNameResult = Name.Create(fatherName).WithPath(nameof(FatherName));
        if (fatherNameResult.IsFailure)
            return fatherNameResult.Error;

        if (string.IsNullOrWhiteSpace(specialization))
            return ProfessorErrors.SpecializationEmpty;

        specialization = specialization.Trim();
        if (specialization.Length > SpecializationMaxLength)
            return ProfessorErrors.SpecializationTooLong;

        if (!Enum.IsDefined(typeof(AcademicRank), academicRank))
            return ProfessorErrors.AcademicRankInvalid;

        if (!Enum.IsDefined(typeof(ProfessorEmploymentType), employmentType))
            return ProfessorErrors.EmploymentTypeInvalid;

        if (employmentStartDate == default)
            return ProfessorErrors.EmploymentStartDateInvalid;

        FirstName = firstNameResult.Data;
        LastName = lastNameResult.Data;
        FatherName = fatherNameResult.Data;
        Email = email;
        MobileNumber = mobileNumber;
        Specialization = specialization;
        AcademicRank = academicRank;
        EmploymentType = employmentType;
        EmploymentStartDate = employmentStartDate;

        return Result.Success();
    }

    public Result MarkIdentityProvisioningSucceeded(UserId identityUserId)
    {
        if (identityUserId.Value == Guid.Empty)
            return ProfessorErrors.IdentityUserIdInvalid;

        if (IdentityProvisioningStatus == IdentityProvisioningStatus.Succeeded)
        {
            if (IdentityUserId == identityUserId)
                return Result.Success();

            return ProfessorErrors.IdentityAlreadyProvisioned;
        }

        IdentityUserId = identityUserId;
        IdentityProvisioningStatus = IdentityProvisioningStatus.Succeeded;
        IdentityProvisioningFailureReason = null;
        IsActive = true;

        return Result.Success();
    }

    public Result MarkIdentityProvisioningFailed(string? reason)
    {
        if (IdentityProvisioningStatus == IdentityProvisioningStatus.Succeeded)
            return ProfessorErrors.IdentityAlreadyProvisioned;

        if (string.IsNullOrWhiteSpace(reason))
            reason = "Identity provisioning failed.";

        reason = reason.Trim();

        if (reason.Length > IdentityProvisioningFailureReasonMaxLength)
            reason = reason[..IdentityProvisioningFailureReasonMaxLength];

        IdentityUserId = null;
        IdentityProvisioningStatus = IdentityProvisioningStatus.Failed;
        IdentityProvisioningFailureReason = reason;
        IsActive = false;

        return Result.Success();
    }

    public Result EnsureEligibleForDeanAssignment()
    {
        if (IdentityProvisioningStatus != IdentityProvisioningStatus.Succeeded || IdentityUserId is null)
            return ProfessorErrors.IdentityNotProvisionedForDeanAssignment;

        if (!IsActive)
            return ProfessorErrors.InactiveForDeanAssignment;

        return Result.Success();
    }

    public Result EnsureEligibleForAcademicManagement()
    {
        if (IdentityProvisioningStatus != IdentityProvisioningStatus.Succeeded || IdentityUserId is null)
            return ProfessorErrors.IdentityNotProvisionedForAcademicManagement;

        if (!IsActive)
            return ProfessorErrors.InactiveForAcademicManagement;

        return Result.Success();
    }

    public Result ChangeProfileImage(FileId profileImageFileId)
    {
        if (profileImageFileId.Value == Guid.Empty)
            return ProfessorErrors.ProfileImageFileIdInvalid;

        if (ProfileImageFileId == profileImageFileId)
            return Result.Success();

        ProfileImageFileId = profileImageFileId;

        return Result.Success();
    }

    public Result RemoveProfileImage()
    {
        if (ProfileImageFileId is null)
            return Result.Success();

        ProfileImageFileId = null;

        return Result.Success();
    }

    public Result Activate()
    {
        if (IdentityProvisioningStatus != IdentityProvisioningStatus.Succeeded || IdentityUserId is null)
            return ProfessorErrors.IdentityNotProvisioned;

        if (IsActive)
            return Result.Success();

        IsActive = true;

        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Success();

        IsActive = false;
        AddDomainEvent(new ProfessorDeactivatedDomainEvent(Id));

        return Result.Success();
    }
}