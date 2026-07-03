namespace Student.Api.Infrastructure.Persistence.Repositories;

public class AdmissionRequestRepository(StudentDbContext context) : IAdmissionRequestRepository
{
    public Task<AdmissionRequest?> GetByIdAsync(AdmissionRequestId id, CancellationToken cancellationToken)
        => context.AdmissionRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(AdmissionRequest admissionRequest, CancellationToken cancellationToken)
    {
        await context.AdmissionRequests.AddAsync(admissionRequest, cancellationToken);
    }

    public async Task<int> GetNextTrackingCodeSequenceAsync(int year, CancellationToken cancellationToken)
    {
        var yearText = year.ToString();
        var prefix = $"AR{yearText}";

        var lastTrackingCode = await context.AdmissionRequests
            .Where(x => x.TrackingCode.Value.StartsWith(prefix))
            .OrderByDescending(x => x.TrackingCode.Value)
            .Select(x => x.TrackingCode.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastTrackingCode is null)
            return 1;

        var sequencePart = lastTrackingCode[prefix.Length..];

        if (!int.TryParse(sequencePart, out var lastSequenceNumber))
            return 1;

        return lastSequenceNumber + 1;
    }

    public Task<bool> IsTrackingCodeExistsAsync(TrackingCode trackingCode, CancellationToken cancellationToken)
    {
        return context.AdmissionRequests.AnyAsync(x => x.TrackingCode == trackingCode, cancellationToken);
    }
}