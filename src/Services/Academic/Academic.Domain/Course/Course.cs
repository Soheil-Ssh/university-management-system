using Academic.Domain.Course.Errors;
using Academic.Domain.Course.ValueObjects;

namespace Academic.Domain.Course;

public sealed class Course : AggregateRoot<CourseId>
{
    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 1000;

    public DepartmentId DepartmentId { get; private set; }
    public CourseCode Code { get; private set; }
    public string Title { get; private set; }
    public int TheoreticalCredits { get; private set; }
    public int PracticalCredits { get; private set; }
    public int TotalCredits => TheoreticalCredits + PracticalCredits;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<CoursePrerequisite> _prerequisites = [];
    public IReadOnlyCollection<CoursePrerequisite> Prerequisites => _prerequisites.AsReadOnly();

#pragma warning disable CS8618
    private Course() { }
#pragma warning restore CS8618

    private Course(CourseId id, DepartmentId departmentId, CourseCode code, string title, int theoreticalCredits, int practicalCredits, string? description) 
        : base(id)
    {
        DepartmentId = departmentId;
        Code = code;
        Title = title;
        TheoreticalCredits = theoreticalCredits;
        PracticalCredits = practicalCredits;
        Description = description;
        IsActive = true;
    }

    public static Result<Course> Create(DepartmentId departmentId, CourseCode code, string title, int theoreticalCredits, int practicalCredits, string? description)
    {
        var validationResult = ValidateDetails(departmentId, title, theoreticalCredits, practicalCredits, description);
        if (validationResult.IsFailure)
            return validationResult.Error;

        return new Course(CourseId.New(), departmentId, code, title.Trim(), theoreticalCredits, practicalCredits, TrimOrNull(description));
    }

    public Result Update(DepartmentId departmentId, string title, int theoreticalCredits, int practicalCredits, string? description)
    {
        var validationResult = ValidateDetails(departmentId, title, theoreticalCredits, practicalCredits, description);

        if (validationResult.IsFailure)
            return validationResult.Error;

        DepartmentId = departmentId;
        Title = title.Trim();
        TheoreticalCredits = theoreticalCredits;
        PracticalCredits = practicalCredits;
        Description = TrimOrNull(description);

        return Result.Success();
    }

    public Result AddPrerequisite(CourseId prerequisiteCourseId)
    {
        if (_prerequisites.Any(x => x.PrerequisiteCourseId == prerequisiteCourseId))
            return CourseErrors.PrerequisiteAlreadyAdded;

        var prerequisiteResult = CoursePrerequisite.Create(Id, prerequisiteCourseId);
        if (prerequisiteResult.IsFailure)
            return prerequisiteResult.Error;

        _prerequisites.Add(prerequisiteResult.Data);
        return Result.Success();
    }

    public Result RemovePrerequisite(CourseId prerequisiteCourseId)
    {
        var prerequisite = _prerequisites
            .FirstOrDefault(x => x.PrerequisiteCourseId == prerequisiteCourseId);

        if (prerequisite is null)
            return CourseErrors.PrerequisiteNotFound;

        _prerequisites.Remove(prerequisite);
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

    private static Result ValidateDetails(DepartmentId departmentId, string title, int theoreticalCredits, int practicalCredits, string? description)
    {
        if (departmentId.Value == Guid.Empty)
            return CourseErrors.DepartmentIdEmpty;

        if (string.IsNullOrWhiteSpace(title))
            return CourseErrors.TitleEmpty;

        if (title.Trim().Length > TitleMaxLength)
            return CourseErrors.TitleTooLong;

        if (theoreticalCredits < 0)
            return CourseErrors.TheoreticalCreditsInvalid;

        if (practicalCredits < 0)
            return CourseErrors.PracticalCreditsInvalid;

        if (theoreticalCredits + practicalCredits <= 0)
            return CourseErrors.TotalCreditsInvalid;

        if (!string.IsNullOrWhiteSpace(description) && description.Trim().Length > DescriptionMaxLength)
            return CourseErrors.DescriptionTooLong;

        return Result.Success();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}