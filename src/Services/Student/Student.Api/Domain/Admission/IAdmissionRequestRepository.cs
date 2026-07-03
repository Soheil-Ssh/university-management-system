namespace Student.Api.Domain.Admission;

public interface IAdmissionRequestRepository
{
    Task AddAsync(AdmissionRequest admissionRequest, CancellationToken cancellationToken);
    Task<int> GetNextTrackingCodeSequenceAsync(int year, CancellationToken cancellationToken);
    Task<bool> IsTrackingCodeExistsAsync(TrackingCode trackingCode, CancellationToken cancellationToken);
}