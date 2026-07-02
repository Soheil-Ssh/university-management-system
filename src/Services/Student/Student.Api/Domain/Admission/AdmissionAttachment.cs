using SharedKernel.Domain.Identifiers;
using Student.Api.Domain.Admission.Enums;

namespace Student.Api.Domain.Admission;

public sealed class AdmissionAttachment : Entity<Guid>
{
    public AdmissionAttachmentType Type { get; private set; }
    public FileId FileId { get; private set; }
    public string? Description { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private AdmissionAttachment() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public AdmissionAttachment(
        Guid id,
        AdmissionAttachmentType type,
        FileId fileId,
        string? description = null) : base(id)
    {
        Type = type;
        FileId = fileId;
        Description = description;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription;
    }
}