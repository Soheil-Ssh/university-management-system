namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record ParentInfo(Name FirstName,
    Name LastName,
    NationalCode NationalCode,
    MobileNumber Mobile);