namespace Student.Api.Domain.Admission.Errors;

public static class ApplicantPersonalInfoErrors
{
    // General errors
    public static readonly Error Required = new("ApplicantPersonalInfo.Required", "Personal information is required.", ErrorType.Validation);
    public static readonly Error NotFound = new("ApplicantPersonalInfo.NotFound", "Personal information not found.", ErrorType.NotFound);

    // birthplace errors
    public static readonly Error BirthPlaceEmpty = new("ApplicantPersonalInfo.BirthPlace.Empty", "Birth place is required.", ErrorType.Validation);
    public static readonly Error BirthPlaceTooLong = new("ApplicantPersonalInfo.BirthPlace.TooLong", $"Birth place is too long.", ErrorType.Validation);

    // Issue place errors
    public static readonly Error IssuePlaceEmpty = new("ApplicantPersonalInfo.IssuePlace.Empty", "Issue place is required.", ErrorType.Validation);
    public static readonly Error IssuePlaceTooLong = new("ApplicantPersonalInfo.IssuePlace.TooLong", $"Issue place is too long.", ErrorType.Validation);

    // Birthdate errors
    private static readonly DateTime MinBirthDate = new(1900, 1, 1);
    public static readonly Error BirthDateFuture = new("ApplicantPersonalInfo.BirthDate.Future", "Birth date cannot be in the future.", ErrorType.Validation);
    public static readonly Error BirthDateTooOld = new("ApplicantPersonalInfo.BirthDate.TooOld", $"Birth date must be after {MinBirthDate:yyyy-MM-dd}.", ErrorType.Validation);
}