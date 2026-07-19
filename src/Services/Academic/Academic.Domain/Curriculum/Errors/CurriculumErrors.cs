namespace Academic.Domain.Curriculum.Errors;

public static class CurriculumErrors
{
    // General errors
    public static readonly Error NotEditable = new("Curriculum.NotEditable", "Only a draft curriculum can be modified.", ErrorType.Conflict);
    public static readonly Error CannotRetire = new("Curriculum.CannotRetire", "Only an active curriculum can be retired.", ErrorType.Validation);
    public static readonly Error CannotActivateWithoutCourses = 
        new("Curriculum.CannotActivateWithoutCourses", "A curriculum must contain at least one course before activation.", ErrorType.Validation);
    public static readonly Error CannotActivate = new("Curriculum.CannotActivate", "Only a draft curriculum can be activated.", ErrorType.Conflict);

    // Major id errors
    public static readonly Error MajorIdEmpty = new("Curriculum.MajorId.Empty", "Major id is required.", ErrorType.Validation);

    // Title errors
    public static readonly Error TitleEmpty = new("Curriculum.Title.Empty", "Curriculum title is required.", ErrorType.Validation);
    public static readonly Error TitleTooLong = new("Curriculum.Title.TooLong", "Curriculum title is too long.", ErrorType.Validation);

    // Version errors
    public static readonly Error VersionEmpty = new("Curriculum.Version.Empty", "Curriculum version is required.", ErrorType.Validation);
    public static readonly Error VersionTooLong = new("Curriculum.Version.TooLong", "Curriculum version is too long.", ErrorType.Validation);

    // EffectiveFrom errors
    public static readonly Error EffectiveFromInvalid = new("Curriculum.EffectiveFrom.Invalid", "Curriculum effective-from date is invalid.", ErrorType.Validation);

    // EffectiveTo
    public static readonly Error EffectiveToBeforeEffectiveFrom = 
        new("Curriculum.EffectiveTo.BeforeEffectiveFrom", "Curriculum effective-to date cannot be before the effective-from date.", ErrorType.Validation);

    // MinimumRequiredCredits errors
    public static readonly Error MinimumRequiredCreditsInvalid = 
        new("Curriculum.MinimumRequiredCredits.Invalid", "Minimum required credits must be greater than zero.", ErrorType.Validation);

    // Description errors
    public static readonly Error DescriptionTooLong = new("Curriculum.Description.TooLong", "Curriculum description is too long.", ErrorType.Validation);

    // Course errors
    public static readonly Error CourseAlreadyAdded = new("Curriculum.Course.AlreadyAdded", "The course has already been added to this curriculum.", ErrorType.Conflict);
    public static readonly Error CourseNotFound = new("Curriculum.Course.NotFound", "The course was not found in this curriculum.", ErrorType.Validation);
}