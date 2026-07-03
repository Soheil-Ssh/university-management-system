namespace Student.Api.Domain.Admission.Errors;

public static class EntranceInfoErrors
{
    public static readonly Error Required = new("EntranceInfo.Required", "Entrance information is required.", ErrorType.Validation);

    public static readonly Error QuotaInvalid = new("EntranceInfo.Quota.Invalid", "Invalid quota value.", ErrorType.Validation);
    public static readonly Error QuotaMustBeFree = new("EntranceInfo.Quota.MustBeFree", "Quota must be Free for international and transfer admissions.", ErrorType.Validation);
    public static readonly Error RegionalQuotaOnlyForNationalExam = new("EntranceInfo.Quota.RegionalQuotaOnlyForNationalExam", "Regional quotas (Region1, Region2, Region3) are only valid for NationalExam admission method.", ErrorType.Validation);

    public static readonly Error EntranceExamRankRequired = new("EntranceInfo.EntranceExamRank.Required", "Entrance exam rank is required for quota-based admissions.", ErrorType.Validation);
    public static readonly Error EntranceExamRankInvalid = new("EntranceInfo.EntranceExamRank.Invalid", "Entrance exam rank must be a positive number.", ErrorType.Validation);
    public static readonly Error EntranceExamRankMustBeNull = new("EntranceInfo.EntranceExamRank.MustBeNull", "Entrance exam rank must not be provided for this admission method.", ErrorType.Validation);

    public static readonly Error EntranceScoreRequired = new("EntranceInfo.EntranceScore.Required", "Entrance score is required for exam-based admissions.", ErrorType.Validation);
    public static readonly Error EntranceScoreInvalid = new("EntranceInfo.EntranceScore.Invalid", "Entrance score must be between 0 and 13000.", ErrorType.Validation);

    public static readonly Error AdmissionMethodInvalid = new("EntranceInfo.AdmissionMethod.Invalid", "Invalid admission method.", ErrorType.Validation);
    public static readonly Error AdmissionTypeInvalid = new("EntranceInfo.AdmissionType.Invalid", "Invalid admission type.", ErrorType.Validation);
    public static readonly Error AdmissionTypeInvalidForMethod = new("EntranceInfo.AdmissionType.InvalidForMethod", "Admission type is not compatible with the selected admission method.", ErrorType.Validation);
    public static readonly Error AdmissionTypeMustBeInternational = new("EntranceInfo.AdmissionType.MustBeInternational", "Admission type must be International for international student admissions.", ErrorType.Validation);

    public static readonly Error IncompatibleMethodAndQuota = new("EntranceInfo.IncompatibleMethodAndQuota", "Exam rank and score are required for NationalExam admission method.", ErrorType.Validation);
}