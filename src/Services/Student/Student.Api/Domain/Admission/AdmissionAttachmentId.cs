namespace Student.Api.Domain.Admission;

public sealed record AdmissionAttachmentId(Guid Value)
{
    public static AdmissionAttachmentId New() => new(Guid.NewGuid());
}