using Academic.Domain.Major.Errors;
using Academic.Domain.Major.ValueObjects;

namespace Academic.Domain.Major;

public sealed class Major : AggregateRoot<MajorId>
{
    private const int NameMaxLength = 150;
    private const int DescriptionMaxLength = 500;

    public DepartmentId DepartmentId { get; private set; }
    public MajorCode Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618
    private Major() { }
#pragma warning restore CS8618

    private Major(MajorId id, DepartmentId departmentId, MajorCode code, string name, string? description) : base(id)
    {
        DepartmentId = departmentId;
        Code = code;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public static Result<Major> Create(DepartmentId departmentId, MajorCode code, string name, string? description)
    {
        var validationResult = ValidateDetails(departmentId, name, description);
        if (validationResult.IsFailure)
            return validationResult.Error;

        return new Major(MajorId.New(), departmentId, code, name.Trim(), TrimOrNull(description));
    }

    public Result Update(DepartmentId departmentId, string name, string? description)
    {
        var validationResult = ValidateDetails(departmentId, name, description);
        if (validationResult.IsFailure)
            return validationResult.Error;

        DepartmentId = departmentId;
        Name = name.Trim();
        Description = TrimOrNull(description);

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

    private static Result ValidateDetails(DepartmentId departmentId, string name, string? description)
    {
        if (departmentId.Value == Guid.Empty)
            return MajorErrors.DepartmentIdEmpty;

        if (string.IsNullOrWhiteSpace(name))
            return MajorErrors.NameEmpty;

        if (name.Trim().Length > NameMaxLength)
            return MajorErrors.NameTooLong;

        if (!string.IsNullOrWhiteSpace(description) && description.Trim().Length > DescriptionMaxLength)
            return MajorErrors.DescriptionTooLong;

        return Result.Success();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}