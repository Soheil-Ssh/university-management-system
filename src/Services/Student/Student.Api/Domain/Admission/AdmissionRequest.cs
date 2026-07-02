using SharedKernel.Domain.Identifiers;
using Student.Api.Domain.Admission.Enums;
using Student.Api.Domain.Admission.ValueObjects;

namespace Student.Api.Domain.Admission;

public sealed class AdmissionRequest : AggregateRoot<AdmissionRequestId>
{
    public TrackingCode TrackingCode { get; private set; }

    public ApplicantPersonalInfo ApplicantPersonalInfo { get; private set; }
    public ApplicantContactInfo ApplicantContactInfo { get; private set; }
    public ParentInfo FatherInfo { get; private set; }
    public ParentInfo MotherInfo { get; private set; }
    public EmergencyContact EmergencyContact { get; private set; }
    public DiplomaInfo DiplomaInfo { get; private set; }
    public EntranceInfo EntranceInfo { get; private set; }

    private readonly List<AdmissionAttachment> _attachments = [];
    public IReadOnlyCollection<AdmissionAttachment> Attachments => _attachments;


    public AdmissionRequestStatus Status { get; private set; }
    public AdmissionRequestStep Step { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public Guid? ReviewerId { get; private set; }

    public UserId UpdatedBy { get; private set; }
}