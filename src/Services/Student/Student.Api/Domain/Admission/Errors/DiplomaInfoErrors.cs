namespace Student.Api.Domain.Admission.Errors;

public static class DiplomaInfoErrors
{
    // General
    public static readonly Error Required = new("DiplomaInfo.Required", "Diploma information is required.", ErrorType.Validation);

    // Average errors
    public static readonly Error AverageBelowMinimum = new("DiplomaInfo.Average.BelowMinimum", "Diploma average must be at least 10.", ErrorType.Validation);
    public static readonly Error AverageAboveMaximum = new("DiplomaInfo.Average.AboveMaximum", "Diploma average cannot exceed 20.", ErrorType.Validation);

    // Field errors
    public static readonly Error FieldEmpty = new("DiplomaInfo.Field.Empty", "Field of study is required.", ErrorType.Validation);
    public static readonly Error FieldTooLong = new("DiplomaInfo.Field.TooLong", "Field of study cannot exceed 200 characters.", ErrorType.Validation);

    // Graduation year errors
    public static readonly Error GraduationYearInFuture = new("DiplomaInfo.GraduationYear.Future", "Graduation year cannot be in the future.", ErrorType.Validation);
    public static readonly Error GraduationYearTooOld = new("DiplomaInfo.GraduationYear.TooOld", "Graduation year must be after 1950.", ErrorType.Validation);
}