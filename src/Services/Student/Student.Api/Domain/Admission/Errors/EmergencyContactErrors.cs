namespace Student.Api.Domain.Admission.Errors;

public static class EmergencyContactErrors
{
    public static readonly Error RelationEmpty = new("EmergencyContact.RelationEmpty.Empty", "Emergency contact is required.", ErrorType.Validation);
    public static readonly Error RelationTooLong = new("EmergencyContact.RelationEmpty.TooLong", "Emergency contact is too long.", ErrorType.Validation);
}