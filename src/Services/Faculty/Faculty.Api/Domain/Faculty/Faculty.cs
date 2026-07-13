namespace Faculty.Api.Domain.Faculty;

public sealed class Faculty : AggregateRoot<FacultyId>
{
    private const int NameMaxLength = 150;
    private const int DescriptionMaxLength = 500;

    public FacultyCode Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public ProfessorId? DeanProfessorId { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618
    private Faculty() { }
#pragma warning restore CS8618

    private Faculty(FacultyId id, FacultyCode code, string name, string? description) : base(id)
    {
        Code = code;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public static Result<Faculty> Create(FacultyCode code, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return FacultyErrors.NameEmpty;

        name = name.Trim();

        if (name.Length > NameMaxLength)
            return FacultyErrors.NameTooLong;

        description = NormalizeDescription(description);

        if (description?.Length > DescriptionMaxLength)
            return FacultyErrors.DescriptionTooLong;

        return new Faculty(FacultyId.New(), code, name, description);
    }

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return FacultyErrors.NameEmpty;

        name = name.Trim();

        if (name.Length > NameMaxLength)
            return FacultyErrors.NameTooLong;

        description = NormalizeDescription(description);

        if (description?.Length > DescriptionMaxLength)
            return FacultyErrors.DescriptionTooLong;

        if (Name == name && Description == description)
            return Result.Success();

        Name = name;
        Description = description;

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

    public Result AssignDean(ProfessorId professorId)
    {
        if (professorId.Value == Guid.Empty)
            return FacultyErrors.DeanProfessorIdEmpty;

        if (!IsActive)
            return FacultyErrors.CannotAssignDeanToInactiveFaculty;

        if (DeanProfessorId == professorId)
            return Result.Success();

        DeanProfessorId = professorId;
        return Result.Success();
    }

    public Result RemoveDean()
    {
        if (DeanProfessorId is null)
            return Result.Success();

        DeanProfessorId = null;
        return Result.Success();
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}