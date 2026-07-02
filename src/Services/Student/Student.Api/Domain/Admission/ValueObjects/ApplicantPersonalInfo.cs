using SharedKernel.Domain.Enums;

namespace Student.Api.Domain.Admission.ValueObjects;

public record ApplicantPersonalInfo(
    Name FirstName,
    Name LastName,
    NationalCode NationalCode,
    string BirthPlace,
    string IssuePlace,
    DateTime BirthDate,
    Gender Gender,
    MaritalStatus MaritalStatus,
    string? PhotoId = null);