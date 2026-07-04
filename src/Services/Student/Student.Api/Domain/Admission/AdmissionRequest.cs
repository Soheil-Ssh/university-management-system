using SharedKernel.Domain.Identifiers;

namespace Student.Api.Domain.Admission;

public sealed class AdmissionRequest : AggregateRoot<AdmissionRequestId>
{
    public TrackingCode TrackingCode { get; private set; }
    public string RegistrationToken { get; set; }

    public ApplicantPersonalInfo? ApplicantPersonalInfo { get; private set; }
    public ApplicantContactInfo? ApplicantContactInfo { get; private set; }
    public ParentInfo? FatherInfo { get; private set; }
    public ParentInfo? MotherInfo { get; private set; }
    public EmergencyContact? EmergencyContact { get; private set; }
    public DiplomaInfo? DiplomaInfo { get; private set; }
    public EntranceInfo? EntranceInfo { get; private set; }

    public AdmissionRequestStatus Status { get; private set; }
    public AdmissionRequestStep Step { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public UserId? ReviewerId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private AdmissionRequest() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private AdmissionRequest(AdmissionRequestId id, TrackingCode trackingCode, string registrationToken)
        : base(id)
    {
        TrackingCode = trackingCode;
        RegistrationToken = registrationToken;
        Status = AdmissionRequestStatus.Draft;
        Step = AdmissionRequestStep.NotStarted;
    }

    public static Result<AdmissionRequest> StartRegistration(TrackingCode trackingCode, string registrationToken)
        => new AdmissionRequest(AdmissionRequestId.New(), trackingCode, registrationToken);

    public Result SaveApplicantPersonalInfo(ApplicantPersonalInfo applicantPersonalInfo,
        string currentRegistrationToken,
        string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotModifySubmittedRequest;

        if (Step != AdmissionRequestStep.NotStarted && Step != AdmissionRequestStep.PersonalInfoCompleted)
            return AdmissionRequestErrors.InvalidStep;

        ApplicantPersonalInfo = applicantPersonalInfo;
        Step = AdmissionRequestStep.PersonalInfoCompleted;
        RegistrationToken = nextRegistrationToken;

        return Result.Success();
    }

    public Result CompleteApplicantContactInfo(ApplicantContactInfo applicantContactInfo,
        string currentRegistrationToken,
        string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotModifySubmittedRequest;

        if (ApplicantPersonalInfo is null)
            return AdmissionRequestErrors.ApplicantPersonalInfoRequired;

        if (Step != AdmissionRequestStep.PersonalInfoCompleted && Step != AdmissionRequestStep.ContactInfoCompleted)
            return AdmissionRequestErrors.InvalidStep;

        ApplicantContactInfo = applicantContactInfo;
        Step = AdmissionRequestStep.ContactInfoCompleted;
        RegistrationToken = nextRegistrationToken;

        return Result.Success();
    }

    public Result CompleteParentsInfo(ParentInfo fatherInfo,
        ParentInfo motherInfo,
        string currentRegistrationToken,
        string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotModifySubmittedRequest;

        if (ApplicantContactInfo is null)
            return AdmissionRequestErrors.ApplicantContactInfoRequired;

        if (Step != AdmissionRequestStep.ContactInfoCompleted && Step != AdmissionRequestStep.ParentsInfoCompleted)
            return AdmissionRequestErrors.InvalidStep;

        if (fatherInfo.NationalCode == motherInfo.NationalCode)
            return AdmissionRequestErrors.ParentsNationalCodesMustBeDifferent;

        FatherInfo = fatherInfo;
        MotherInfo = motherInfo;
        Step = AdmissionRequestStep.ParentsInfoCompleted;
        RegistrationToken = nextRegistrationToken;

        return Result.Success();
    }

    public Result CompleteEmergencyContact(EmergencyContact emergencyContact,
        string currentRegistrationToken,
        string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotModifySubmittedRequest;

        if (FatherInfo is null || MotherInfo is null)
            return AdmissionRequestErrors.ParentsInfoRequired;

        if (Step != AdmissionRequestStep.ParentsInfoCompleted && Step != AdmissionRequestStep.EmergencyContactInfoCompleted)
            return AdmissionRequestErrors.InvalidStep;

        EmergencyContact = emergencyContact;
        Step = AdmissionRequestStep.EmergencyContactInfoCompleted;
        RegistrationToken = nextRegistrationToken;

        return Result.Success();
    }

    public Result CompleteDiplomaInfo(DiplomaInfo diplomaInfo,
        string currentRegistrationToken,
        string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotModifySubmittedRequest;

        if (EmergencyContact is null)
            return AdmissionRequestErrors.EmergencyContactRequired;

        if (Step != AdmissionRequestStep.EmergencyContactInfoCompleted &&
            Step != AdmissionRequestStep.DiplomaInfoCompleted)
            return AdmissionRequestErrors.InvalidStep;

        DiplomaInfo = diplomaInfo;
        Step = AdmissionRequestStep.DiplomaInfoCompleted;
        RegistrationToken = nextRegistrationToken;

        return Result.Success();
    }

    public Result CompleteEntranceInfo(EntranceInfo entranceInfo,
        string currentRegistrationToken,
        string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotModifySubmittedRequest;

        if (DiplomaInfo is null)
            return AdmissionRequestErrors.DiplomaInfoRequired;

        if (Step != AdmissionRequestStep.DiplomaInfoCompleted &&
            Step != AdmissionRequestStep.EntranceInfoCompleted)
            return AdmissionRequestErrors.InvalidStep;

        EntranceInfo = entranceInfo;
        Step = AdmissionRequestStep.EntranceInfoCompleted;
        RegistrationToken = nextRegistrationToken;

        return Result.Success();
    }

    public Result Submit(string currentRegistrationToken, string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotSubmitRequest;

        if (Step != AdmissionRequestStep.EntranceInfoCompleted)
            return AdmissionRequestErrors.AdmissionRequestIsNotComplete;

        Status = AdmissionRequestStatus.Pending;
        RegistrationToken = nextRegistrationToken;
        SubmittedAt = DateTime.UtcNow;

        return Result.Success();
    }
}