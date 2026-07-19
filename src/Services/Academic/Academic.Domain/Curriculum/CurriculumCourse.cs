using Academic.Domain.Course;
using Academic.Domain.Curriculum.Enums;
using Academic.Domain.Curriculum.Errors;

namespace Academic.Domain.Curriculum;

public sealed class CurriculumCourse : Entity<CurriculumCourseId>
{
    public CurriculumId CurriculumId { get; private set; }
    public CourseId CourseId { get; private set; }
    public CurriculumCourseCategory Category { get; private set; }
    public CurriculumCourseRequirementType RequirementType { get; private set; }
    public int SuggestedSemester { get; private set; }
    public int? DisplayOrder { get; private set; }

#pragma warning disable CS8618
    private CurriculumCourse() { }
#pragma warning restore CS8618

    private CurriculumCourse(CurriculumId curriculumId, CourseId courseId,
        CurriculumCourseCategory category,
        CurriculumCourseRequirementType requirementType,
        int suggestedSemester,
        int? displayOrder)
    {
        CurriculumId = curriculumId;
        CourseId = courseId;
        Category = category;
        RequirementType = requirementType;
        SuggestedSemester = suggestedSemester;
        DisplayOrder = displayOrder;
    }

    internal static Result<CurriculumCourse> Create(CurriculumId curriculumId,
        CourseId courseId,
        CurriculumCourseCategory category,
        CurriculumCourseRequirementType requirementType,
        int suggestedSemester,
        int? displayOrder)
    {
        var validationResult = Validate(curriculumId, courseId, category, requirementType, suggestedSemester, displayOrder);
        if (validationResult.IsFailure)
            return validationResult.Error;

        return new CurriculumCourse(curriculumId, courseId, category, requirementType, suggestedSemester, displayOrder);
    }

    internal Result Update(CurriculumCourseCategory category, CurriculumCourseRequirementType requirementType, int suggestedSemester, int? displayOrder)
    {
        var validationResult = Validate(CurriculumId, CourseId, category, requirementType, suggestedSemester, displayOrder);
        if (validationResult.IsFailure)
            return validationResult.Error;

        Category = category;
        RequirementType = requirementType;
        SuggestedSemester = suggestedSemester;
        DisplayOrder = displayOrder;

        return Result.Success();
    }

    private static Result Validate(CurriculumId curriculumId,
        CourseId courseId,
        CurriculumCourseCategory category,
        CurriculumCourseRequirementType requirementType,
        int suggestedSemester,
        int? displayOrder)
    {
        if (curriculumId.Value == Guid.Empty)
            return CurriculumCourseErrors.CurriculumIdEmpty;

        if (courseId.Value == Guid.Empty)
            return CurriculumCourseErrors.CourseIdEmpty;

        if (!Enum.IsDefined(category))
            return CurriculumCourseErrors.CategoryInvalid;

        if (!Enum.IsDefined(requirementType))
            return CurriculumCourseErrors.RequirementTypeInvalid;

        if (suggestedSemester <= 0)
            return CurriculumCourseErrors.SuggestedSemesterInvalid;

        if (displayOrder is <= 0)
            return CurriculumCourseErrors.DisplayOrderInvalid;

        return Result.Success();
    }
}