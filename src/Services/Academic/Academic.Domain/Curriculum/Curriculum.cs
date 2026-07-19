using Academic.Domain.Course;
using Academic.Domain.Curriculum.Enums;
using Academic.Domain.Curriculum.Errors;
using Academic.Domain.Curriculum.ValueObjects;
using Academic.Domain.Major;

namespace Academic.Domain.Curriculum;

public sealed class Curriculum : AggregateRoot<CurriculumId>
{
    private const int TitleMaxLength = 200;
    private const int VersionMaxLength = 50;
    private const int DescriptionMaxLength = 1000;

    public MajorId MajorId { get; private set; }
    public CurriculumCode Code { get; private set; }
    public string Title { get; private set; }
    public string Version { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public int MinimumRequiredCredits { get; private set; }
    public CurriculumStatus Status { get; private set; }
    public string? Description { get; private set; }

    private readonly List<CurriculumCourse> _courses = [];
    public IReadOnlyCollection<CurriculumCourse> Courses => _courses.AsReadOnly();

#pragma warning disable CS8618
    private Curriculum() { }
#pragma warning restore CS8618

    private Curriculum(CurriculumId id,
        MajorId majorId,
        CurriculumCode code,
        string title,
        string version,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        int minimumRequiredCredits,
        string? description) : base(id)
    {
        MajorId = majorId;
        Code = code;
        Title = title;
        Version = version;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        MinimumRequiredCredits = minimumRequiredCredits;
        Description = description;
        Status = CurriculumStatus.Draft;
    }

    public static Result<Curriculum> Create(MajorId majorId,
        CurriculumCode code,
        string title,
        string version,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        int minimumRequiredCredits,
        string? description)
    {
        var validationResult = ValidateDetails(majorId, title, version, effectiveFrom, effectiveTo, minimumRequiredCredits, description);
        if (validationResult.IsFailure)
            return validationResult.Error;

        return new Curriculum(CurriculumId.New(), majorId, code, title.Trim(), version.Trim(), effectiveFrom, effectiveTo, minimumRequiredCredits, TrimOrNull(description));
    }

    public Result Update(string title, string version, DateOnly effectiveFrom, DateOnly? effectiveTo, int minimumRequiredCredits, string? description)
    {
        if (Status != CurriculumStatus.Draft)
            return CurriculumErrors.NotEditable;

        var validationResult = ValidateDetails(MajorId, title, version, effectiveFrom, effectiveTo, minimumRequiredCredits, description);
        if (validationResult.IsFailure)
            return validationResult.Error;

        Title = title.Trim();
        Version = version.Trim();
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        MinimumRequiredCredits = minimumRequiredCredits;
        Description = TrimOrNull(description);

        return Result.Success();
    }

    public Result AddCourse(CourseId courseId, CurriculumCourseCategory category, CurriculumCourseRequirementType requirementType, int suggestedSemester, int? displayOrder)
    {
        if (Status != CurriculumStatus.Draft)
            return CurriculumErrors.NotEditable;

        if (_courses.Any(x => x.CourseId == courseId))
            return CurriculumErrors.CourseAlreadyAdded;

        var courseResult = CurriculumCourse.Create(Id, courseId, category, requirementType, suggestedSemester, displayOrder);
        if (courseResult.IsFailure)
            return courseResult.Error;

        _courses.Add(courseResult.Data);
        return Result.Success();
    }

    public Result UpdateCourse(CourseId courseId, CurriculumCourseCategory category, CurriculumCourseRequirementType requirementType, int suggestedSemester, int? displayOrder)
    {
        if (Status != CurriculumStatus.Draft)
            return CurriculumErrors.NotEditable;

        var curriculumCourse = _courses.FirstOrDefault(x => x.CourseId == courseId);
        if (curriculumCourse is null)
            return CurriculumErrors.CourseNotFound;

        return curriculumCourse.Update(category, requirementType, suggestedSemester, displayOrder);
    }

    public Result RemoveCourse(CourseId courseId)
    {
        if (Status != CurriculumStatus.Draft)
            return CurriculumErrors.NotEditable;

        var curriculumCourse = _courses.FirstOrDefault(x => x.CourseId == courseId);
        if (curriculumCourse is null)
            return CurriculumErrors.CourseNotFound;

        _courses.Remove(curriculumCourse);
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == CurriculumStatus.Active)
            return Result.Success();

        if (Status != CurriculumStatus.Draft)
            return CurriculumErrors.CannotActivate;

        if (_courses.Count == 0)
            return CurriculumErrors.CannotActivateWithoutCourses;

        Status = CurriculumStatus.Active;
        return Result.Success();
    }

    public Result Retire(DateOnly effectiveTo)
    {
        if (Status == CurriculumStatus.Retired)
            return Result.Success();

        if (Status != CurriculumStatus.Active)
            return CurriculumErrors.CannotRetire;

        if (effectiveTo < EffectiveFrom)
            return CurriculumErrors.EffectiveToBeforeEffectiveFrom;

        EffectiveTo = effectiveTo;
        Status = CurriculumStatus.Retired;

        return Result.Success();
    }

    private static Result ValidateDetails(MajorId majorId,
        string title,
        string version,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo,
        int minimumRequiredCredits,
        string? description)
    {
        if (majorId.Value == Guid.Empty)
            return CurriculumErrors.MajorIdEmpty;

        if (string.IsNullOrWhiteSpace(title))
            return CurriculumErrors.TitleEmpty;

        if (title.Trim().Length > TitleMaxLength)
            return CurriculumErrors.TitleTooLong;

        if (string.IsNullOrWhiteSpace(version))
            return CurriculumErrors.VersionEmpty;

        if (version.Trim().Length > VersionMaxLength)
            return CurriculumErrors.VersionTooLong;

        if (effectiveFrom == default)
            return CurriculumErrors.EffectiveFromInvalid;

        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
            return CurriculumErrors.EffectiveToBeforeEffectiveFrom;

        if (minimumRequiredCredits <= 0)
            return CurriculumErrors.MinimumRequiredCreditsInvalid;

        if (!string.IsNullOrWhiteSpace(description) &&
            description.Trim().Length > DescriptionMaxLength)
            return CurriculumErrors.DescriptionTooLong;

        return Result.Success();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}