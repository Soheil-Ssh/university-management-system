namespace Student.Api.Domain.Admission;

public interface IAdmissionRequestRepository
{
    Task<AdmissionRequest?> GetByIdAsync(AdmissionRequestId id, CancellationToken cancellationToken);
    Task AddAsync(AdmissionRequest admissionRequest, CancellationToken cancellationToken);
    Task<int> GetNextTrackingCodeSequenceAsync(int year, CancellationToken cancellationToken);
    Task<bool> IsTrackingCodeExistsAsync(TrackingCode trackingCode, CancellationToken cancellationToken);
}