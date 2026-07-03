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

    private readonly List<AdmissionAttachment> _attachments = [];
    public IReadOnlyCollection<AdmissionAttachment> Attachments => _attachments;

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

    public static Result<AdmissionRequest> StartRegistration(TrackingCode trackingCode,
        string registrationToken)
        => new AdmissionRequest(AdmissionRequestId.New(), trackingCode, registrationToken);

    public Result SaveApplicantPersonalInfo(ApplicantPersonalInfo applicantPersonalInfo,
        string currentRegistrationToken,
        string nextRegistrationToken)
    {
        if (RegistrationToken != currentRegistrationToken)
            return AdmissionRequestErrors.InvalidRegistrationToken;

        if (Status != AdmissionRequestStatus.Draft)
            return AdmissionRequestErrors.CannotModifySubmittedRequest;

        if (Step != AdmissionRequestStep.NotStarted)
            return AdmissionRequestErrors.InvalidStep;

        ApplicantPersonalInfo = applicantPersonalInfo;
        Step = AdmissionRequestStep.PersonalInfoCompleted;
        RegistrationToken = nextRegistrationToken;

        return Result.Success();
    }
}