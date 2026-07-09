using CentralOrganization.Api.Domain.Employee.Enums;
using CentralOrganization.Api.Domain.Employee.Errors;
using CentralOrganization.Api.Domain.Employee.Events;
using SharedKernel.Domain.Enums;
using SharedKernel.Domain.Identifiers;

namespace CentralOrganization.Api.Domain.Employee;

public sealed class Employee : AggregateRoot<EmployeeId>
{
    private const int MaxEducationFieldLength = 150;
    private const int MaxJobTitleLength = 150;

    public UnitId UnitId { get; private set; }
    public PersonnelCode PersonnelCode { get; private set; }
    public Name FirstName { get; private set; }
    public Name LastName { get; private set; }
    public Name? FatherName { get; private set; }
    public NationalCode NationalCode { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public MobileNumber MobileNumber { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public string EducationField { get; private set; }
    public string JobTitle { get; private set; }
    public EmploymentStatus EmploymentStatus { get; private set; }
    public IdentityProvisioningStatus IdentityProvisioningStatus { get; private set; }
    public string? IdentityProvisioningFailureReason { get; set; }
    public UserId? IdentityUserId { get; private set; }
    public FileId? ProfileImageFileId { get; private set; }
    public string FullName => $"{FirstName} {LastName}";

#pragma warning disable CS8618
    private Employee(IdentityProvisioningStatus identityProvisioningStatus)
    {
        IdentityProvisioningStatus = identityProvisioningStatus;
    }
#pragma warning restore CS8618

    private Employee(EmployeeId id,
        UnitId unitId,
        PersonnelCode personnelCode,
        Name firstName,
        Name lastName,
        Name? fatherName,
        NationalCode nationalCode,
        DateOnly birthDate,
        Gender gender,
        MobileNumber mobileNumber,
        PhoneNumber? phoneNumber,
        Email email,
        string educationField,
        string jobTitle,
        FileId profileImageFileId)
        : base(id)
    {
        UnitId = unitId;
        PersonnelCode = personnelCode;
        FirstName = firstName;
        LastName = lastName;
        FatherName = fatherName;
        NationalCode = nationalCode;
        BirthDate = birthDate;
        Gender = gender;
        MobileNumber = mobileNumber;
        PhoneNumber = phoneNumber;
        Email = email;
        EducationField = educationField;
        JobTitle = jobTitle;
        ProfileImageFileId = profileImageFileId;

        EmploymentStatus = EmploymentStatus.Inactive;
        IdentityProvisioningStatus = IdentityProvisioningStatus.Pending;
        IdentityProvisioningFailureReason = null;
        IdentityUserId = null;

        // Add domain event for identity provisioning request
        AddDomainEvent(new EmployeeIdentityProvisioningRequestedDomainEvent(
            EmployeeId: id.Value,
            PersonnelCode: PersonnelCode.Value,
            NationalCode: NationalCode.Value,
            FirstName: FirstName.Value,
            LastName: LastName.Value,
            Email: Email.Value,
            MobileNumber: MobileNumber.Value));
    }

    public static Result<Employee> Create(UnitId unitId,
        PersonnelCode personnelCode,
        string firstName,
        string lastName,
        string? fatherName,
        string nationalCode,
        DateOnly birthDate,
        Gender gender,
        string mobileNumber,
        string? phoneNumber,
        string email,
        string educationField,
        string jobTitle,
        Guid profileImageFileId)
    {
        var firstNameResult = Name.Create(firstName).WithPath(nameof(FirstName));
        if (firstNameResult.IsFailure)
            return firstNameResult.Error;

        var lastNameResult = Name.Create(lastName).WithPath(nameof(LastName));
        if (lastNameResult.IsFailure)
            return lastNameResult.Error;

        Result<Name>? fatherNameResult = null;
        if (!string.IsNullOrWhiteSpace(fatherName))
        {
            fatherNameResult = Name.Create(fatherName).WithPath(nameof(FatherName));

            if (fatherNameResult.IsFailure)
                return fatherNameResult.Error;
        }

        var nationalCodeResult = NationalCode.Create(nationalCode).WithPath(nameof(NationalCode));
        if (nationalCodeResult.IsFailure)
            return nationalCodeResult.Error;

        if (birthDate >= DateOnly.FromDateTime(DateTime.UtcNow))
            return EmployeeErrors.BirthDateInvalid;

        if (!Enum.IsDefined(typeof(Gender), gender))
            return EmployeeErrors.GenderInvalid;

        var mobileResult = MobileNumber.Create(mobileNumber).WithPath(nameof(MobileNumber));
        if (mobileResult.IsFailure)
            return mobileResult.Error;

        Result<PhoneNumber>? phoneResult = null;

        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            phoneResult = PhoneNumber.Create(phoneNumber).WithPath(nameof(PhoneNumber));

            if (phoneResult.IsFailure)
                return phoneResult.Error;
        }

        var emailResult = Email.Create(email).WithPath(nameof(Email));
        if (emailResult.IsFailure)
            return emailResult.Error;

        if (string.IsNullOrWhiteSpace(educationField))
            return EmployeeErrors.EducationFieldEmpty;

        educationField = educationField.Trim();
        if (educationField.Length > MaxEducationFieldLength)
            return EmployeeErrors.EducationFieldTooLong;

        if (string.IsNullOrWhiteSpace(jobTitle))
            return EmployeeErrors.JobTitleEmpty;

        jobTitle = jobTitle.Trim();
        if (jobTitle.Length > MaxJobTitleLength)
            return EmployeeErrors.JobTitleTooLong;

        if (profileImageFileId == Guid.Empty)
            return EmployeeErrors.ProfileImageFileIdInvalid;

        return new Employee(EmployeeId.New(),
            unitId,
            personnelCode,
            firstNameResult.Data,
            lastNameResult.Data,
            fatherNameResult?.Data,
            nationalCodeResult.Data,
            birthDate,
            gender,
            mobileResult.Data,
            phoneResult?.Data,
            emailResult.Data,
            educationField,
            jobTitle,
            new FileId(profileImageFileId));
    }

    public Result MarkIdentityProvisioningSucceeded(UserId identityUserId)
    {
        if (IdentityProvisioningStatus == IdentityProvisioningStatus.Succeeded)
        {
            if (IdentityUserId == identityUserId)
                return Result.Success();

            return EmployeeErrors.IdentityAlreadyProvisioned;
        }

        IdentityUserId = identityUserId;
        EmploymentStatus = EmploymentStatus.Active;
        IdentityProvisioningStatus = IdentityProvisioningStatus.Succeeded;
        IdentityProvisioningFailureReason = null;

        return Result.Success();
    }

    public Result MarkIdentityProvisioningFailed(string reason)
    {
        if (IdentityProvisioningStatus == IdentityProvisioningStatus.Succeeded)
            return EmployeeErrors.IdentityAlreadyProvisioned;

        if (string.IsNullOrWhiteSpace(reason))
            reason = "Identity provisioning failed.";

        if (reason.Length > 1000)
            reason = reason[..1000];

        EmploymentStatus = EmploymentStatus.Inactive;
        IdentityProvisioningStatus = IdentityProvisioningStatus.Failed;
        IdentityProvisioningFailureReason = reason;

        return Result.Success();
    }
}