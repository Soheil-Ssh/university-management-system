using Student.Api.Domain.Admission.Enums;
using Student.Api.Domain.Admission.Errors;

namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record EntranceInfo
{
    public Quota Quota { get; }
    public int? EntranceExamRank { get; }
    public double? EntranceScore { get; }
    public AdmissionMethod AdmissionMethod { get; }
    public AdmissionType AdmissionType { get; }

    private EntranceInfo(Quota quota, int? entranceExamRank, double? entranceScore, AdmissionMethod admissionMethod, AdmissionType admissionType)
    {
        Quota = quota;
        EntranceExamRank = entranceExamRank;
        EntranceScore = entranceScore;
        AdmissionMethod = admissionMethod;
        AdmissionType = admissionType;
    }

    public static Result<EntranceInfo> Create(Quota quota, int? entranceExamRank, 
        double? entranceScore,
        AdmissionMethod admissionMethod,
        AdmissionType admissionType)
    {
        if (!Enum.IsDefined(typeof(Quota), quota))
            return EntranceInfoErrors.QuotaInvalid;

        if (!Enum.IsDefined(typeof(AdmissionMethod), admissionMethod))
            return EntranceInfoErrors.AdmissionMethodInvalid;

        if (!Enum.IsDefined(typeof(AdmissionType), admissionType))
            return EntranceInfoErrors.AdmissionTypeInvalid;

        if (entranceScore is < 0 or > 13000)
            return EntranceInfoErrors.EntranceScoreInvalid;

        if (entranceExamRank is <= 0)
            return EntranceInfoErrors.EntranceExamRankInvalid;

        switch (admissionMethod)
        {
            case AdmissionMethod.NationalExam:
                if (!entranceExamRank.HasValue)
                    return EntranceInfoErrors.EntranceExamRankRequired;
                if (!entranceScore.HasValue)
                    return EntranceInfoErrors.EntranceScoreRequired;

                if (admissionType is not AdmissionType.Daytime
                    and not AdmissionType.Nighttime
                    and not AdmissionType.NonGovernmental)
                    return EntranceInfoErrors.AdmissionTypeInvalidForMethod;
                break;

            case AdmissionMethod.AcademicRecord:
            case AdmissionMethod.AcademicRecordWithQuota:
            case AdmissionMethod.Talented:
                if (entranceExamRank.HasValue)
                    return EntranceInfoErrors.EntranceExamRankMustBeNull;

                if (!entranceScore.HasValue)
                    return EntranceInfoErrors.EntranceScoreRequired;

                if (admissionType is not AdmissionType.Daytime
                    and not AdmissionType.Nighttime
                    and not AdmissionType.NonGovernmental)
                    return EntranceInfoErrors.AdmissionTypeInvalidForMethod;
                break;

            case AdmissionMethod.InternationalScholarship:
            case AdmissionMethod.InternationalFree:
                if (entranceExamRank.HasValue)
                    return EntranceInfoErrors.EntranceExamRankMustBeNull;

                if (quota != Quota.Free)
                    return EntranceInfoErrors.QuotaMustBeFree;

                if (admissionType != AdmissionType.International)
                    return EntranceInfoErrors.AdmissionTypeMustBeInternational;

                break;

            case AdmissionMethod.TransferFromAbroad:
                if (entranceExamRank.HasValue)
                    return EntranceInfoErrors.EntranceExamRankMustBeNull;

                if (quota != Quota.Free)
                    return EntranceInfoErrors.QuotaMustBeFree;

                if (admissionType is not AdmissionType.Transfer and not AdmissionType.Exchange)
                    return EntranceInfoErrors.AdmissionTypeInvalidForMethod;

                break;
        }

        if ((quota is Quota.Region1 or Quota.Region2 or Quota.Region3) &&
            admissionMethod != AdmissionMethod.NationalExam)
            return EntranceInfoErrors.RegionalQuotaOnlyForNationalExam;

        return new EntranceInfo(quota, entranceExamRank, entranceScore, admissionMethod, admissionType);
    }
}