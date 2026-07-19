namespace Academic.Domain.Curriculum.Errors;

public static class CurriculumCodeErrors
{
    public static readonly Error Empty = new("CurriculumCode.Empty", "Curriculum code cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = 
        new("CurriculumCode.InvalidFormat", "Curriculum code format is invalid. Expected format is UMS_AC_MAJ_0001_CUR_0001.", ErrorType.Validation);
    public static readonly Error NumberOutOfRange = new("CurriculumCode.NumberOutOfRange", "Curriculum code number must be between 1 and 9999.", ErrorType.Validation);
}