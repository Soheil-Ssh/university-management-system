using Student.Api.Domain.Admission.Errors;

namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record TrackingCode
{
    private const string Prefix = "AR";
    private const int YearLength = 4;
    private const int SequenceLength = 8;

    public string Value { get; }

    private TrackingCode(string value)
    {
        Value = value;
    }

    public static TrackingCode Generate(int sequenceNumber)
    {
        var year = DateTime.Now.ToString("yyyy");
        var sequence = sequenceNumber.ToString($"D{SequenceLength}");
        var raw = $"{Prefix}{year}{sequence}";

        return new TrackingCode(raw);
    }

    public static Result<TrackingCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return TrackingCodeErrors.Empty;

        if (!IsValidFormat(value))
            return TrackingCodeErrors.InvalidFormat;

        return new TrackingCode(value);
    }

    private static bool IsValidFormat(string code)
    {
        if (code.Length != Prefix.Length + YearLength + SequenceLength)
            return false;

        if (!code.StartsWith(Prefix))
            return false;

        var yearPart = code.Substring(Prefix.Length, YearLength);
        if (!int.TryParse(yearPart, out _))
            return false;

        var seqPart = code.Substring(Prefix.Length + YearLength);
        if (!int.TryParse(seqPart, out _))
            return false;

        return true;
    }

    public override string ToString() => Value;

    public static implicit operator string(TrackingCode code) => code.Value;
}