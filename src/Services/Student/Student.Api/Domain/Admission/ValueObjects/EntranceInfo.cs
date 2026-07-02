using Student.Api.Domain.Admission.Enums;

namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record EntranceInfo(
    Quota Quota,
    int? EntranceExamRank,
    double? EntranceScore,
    AdmissionMethod AdmissionMethod,
    AdmissionType AdmissionType
);