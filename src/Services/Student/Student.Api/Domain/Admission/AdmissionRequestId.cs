namespace Student.Api.Domain.Admission;

public sealed record AdmissionRequestId(Guid Value)
{
    public static AdmissionRequestId New() => new(Guid.NewGuid());
}