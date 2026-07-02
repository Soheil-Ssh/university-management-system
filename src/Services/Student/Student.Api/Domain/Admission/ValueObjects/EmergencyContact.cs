namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record EmergencyContact(
    Name FirstName,
    Name LastName,
    string Relation,
    MobileNumber Mobile);