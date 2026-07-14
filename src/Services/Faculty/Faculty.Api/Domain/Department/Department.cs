using Faculty.Api.Domain.Department.Errors;

namespace Faculty.Api.Domain.Department;

public sealed class Department : AggregateRoot<DepartmentId>
{
    private const int NameMaxLength = 150;
    private const int ShortNameMaxLength = 50;
    private const int DescriptionMaxLength = 500;
    private const int InternalPhoneNumberMaxLength = 20;
    private const int OfficeLocationMaxLength = 200;

    public FacultyId FacultyId { get; private set; }
    public DepartmentCode Code { get; private set; }
    public string Name { get; private set; }
    public string? ShortName { get; private set; }
    public string? Description { get; private set; }
    public ProfessorId? HeadProfessorId { get; private set; }
    public EmployeeId? PrimaryExpertEmployeeId { get; private set; }
    public Email? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public string? InternalPhoneNumber { get; private set; }
    public string? OfficeLocation { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618
    private Department() { }
#pragma warning restore CS8618

    private Department(DepartmentId id,
        FacultyId facultyId,
        DepartmentCode code,
        string name,
        string? shortName,
        string? description,
        Email? email,
        PhoneNumber? phoneNumber,
        string? internalPhoneNumber,
        string? officeLocation)
        : base(id)
    {
        FacultyId = facultyId;
        Code = code;
        Name = name;
        ShortName = shortName;
        Description = description;
        Email = email;
        PhoneNumber = phoneNumber;
        InternalPhoneNumber = internalPhoneNumber;
        OfficeLocation = officeLocation;

        HeadProfessorId = null;
        PrimaryExpertEmployeeId = null;
        IsActive = true;
    }

    public static Result<Department> Create(FacultyId facultyId,
        DepartmentCode code,
        string name,
        string? shortName,
        string? description,
        string? email,
        string? phoneNumber,
        string? internalPhoneNumber,
        string? officeLocation)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DepartmentErrors.NameEmpty;

        name = name.Trim();
        if (name.Length > NameMaxLength)
            return DepartmentErrors.NameTooLong;

        shortName = NormalizeOptional(shortName);

        if (shortName?.Length > ShortNameMaxLength)
            return DepartmentErrors.ShortNameTooLong;

        description = NormalizeOptional(description);

        Email? emailValueObject = null;
        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email).WithPath(nameof(Email));

            if (emailResult.IsFailure)
                return emailResult.Error;

            emailValueObject = emailResult.Data;
        }

        PhoneNumber? phoneNumberValueObject = null;
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phoneNumberResult = PhoneNumber.Create(phoneNumber).WithPath(nameof(PhoneNumber));

            if (phoneNumberResult.IsFailure)
                return phoneNumberResult.Error;

            phoneNumberValueObject = phoneNumberResult.Data;
        }

        if (description?.Length > DescriptionMaxLength)
            return DepartmentErrors.DescriptionTooLong;

        internalPhoneNumber = NormalizeOptional(internalPhoneNumber);

        if (internalPhoneNumber?.Length > InternalPhoneNumberMaxLength)
            return DepartmentErrors.InternalPhoneNumberTooLong;

        officeLocation = NormalizeOptional(officeLocation);

        if (officeLocation?.Length > OfficeLocationMaxLength)
            return DepartmentErrors.OfficeLocationTooLong;

        return new Department(
            DepartmentId.New(),
            facultyId,
            code,
            name,
            shortName,
            description,
            emailValueObject,
            phoneNumberValueObject,
            internalPhoneNumber,
            officeLocation);
    }

    public Result UpdateDetails(string name, string? shortName, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DepartmentErrors.NameEmpty;

        name = name.Trim();
        if (name.Length > NameMaxLength)
            return DepartmentErrors.NameTooLong;

        shortName = NormalizeOptional(shortName);
        if (shortName?.Length > ShortNameMaxLength)
            return DepartmentErrors.ShortNameTooLong;

        description = NormalizeOptional(description);
        if (description?.Length > DescriptionMaxLength)
            return DepartmentErrors.DescriptionTooLong;

        if (Name == name && ShortName == shortName && Description == description)
            return Result.Success();

        Name = name;
        ShortName = shortName;
        Description = description;

        return Result.Success();
    }

    public Result UpdateContactInformation(string? email, string? phoneNumber, string? internalPhoneNumber, string? officeLocation)
    {
        Email? emailValueObject = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailResult = Email.Create(email).WithPath(nameof(Email));
            if (emailResult.IsFailure)
                return emailResult.Error;

            emailValueObject = emailResult.Data;
        }

        PhoneNumber? phoneNumberValueObject = null;
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            var phoneNumberResult = PhoneNumber.Create(phoneNumber).WithPath(nameof(PhoneNumber));
            if (phoneNumberResult.IsFailure)
                return phoneNumberResult.Error;

            phoneNumberValueObject = phoneNumberResult.Data;
        }

        internalPhoneNumber = NormalizeOptional(internalPhoneNumber);
        if (internalPhoneNumber?.Length > InternalPhoneNumberMaxLength)
            return DepartmentErrors.InternalPhoneNumberTooLong;

        officeLocation = NormalizeOptional(officeLocation);
        if (officeLocation?.Length > OfficeLocationMaxLength)
            return DepartmentErrors.OfficeLocationTooLong;

        if (Email == emailValueObject && 
            PhoneNumber == phoneNumberValueObject &&
            InternalPhoneNumber == internalPhoneNumber && 
            OfficeLocation == officeLocation)
            return Result.Success();

        Email = emailValueObject;
        PhoneNumber = phoneNumberValueObject;
        InternalPhoneNumber = internalPhoneNumber;
        OfficeLocation = officeLocation;

        return Result.Success();
    }


    public Result AssignHead(ProfessorId professorId)
    {
        if (!IsActive)
            return DepartmentErrors.CannotAssignHeadToInactiveDepartment;

        if (HeadProfessorId == professorId)
            return Result.Success();

        HeadProfessorId = professorId;

        return Result.Success();
    }

    public Result RemoveHead()
    {
        if (HeadProfessorId is null)
            return Result.Success();

        HeadProfessorId = null;

        return Result.Success();
    }


    public Result AssignPrimaryExpert(EmployeeId employeeId)
    {
        if (!IsActive)
            return DepartmentErrors.CannotAssignExpertToInactiveDepartment;

        if (PrimaryExpertEmployeeId == employeeId)
            return Result.Success();

        PrimaryExpertEmployeeId = employeeId;

        return Result.Success();
    }

    public Result RemovePrimaryExpert()
    {
        if (PrimaryExpertEmployeeId is null)
            return Result.Success();

        PrimaryExpertEmployeeId = null;

        return Result.Success();
    }

    public Result Activate()
    {
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
        return Result.Success();
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}