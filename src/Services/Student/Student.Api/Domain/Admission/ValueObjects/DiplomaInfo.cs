namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record DiplomaInfo
{
    private const decimal MinAverage = 10m;
    private const decimal MaxAverage = 20m;
    private const int MinGraduationYear = 1950;
    private const int MaxFieldLength = 200;

    public decimal Average { get; private set; }
    public string Field { get; private set; }
    public int GraduationYear { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private DiplomaInfo() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private DiplomaInfo(decimal average, string field, int graduationYear)
    {
        Average = average;
        Field = field;
        GraduationYear = graduationYear;
    }

    public static Result<DiplomaInfo> Create(decimal average, string field, int graduationYear)
    {
        if (average < MinAverage)
            return DiplomaInfoErrors.AverageBelowMinimum;

        if (average > MaxAverage)
            return DiplomaInfoErrors.AverageAboveMaximum;

        if (string.IsNullOrWhiteSpace(field))
            return DiplomaInfoErrors.FieldEmpty;

        field = field.Trim();
        if (field.Length > MaxFieldLength)
            return DiplomaInfoErrors.FieldTooLong;

        var currentYear = DateTime.Now.Year;
        if (graduationYear > currentYear)
            return DiplomaInfoErrors.GraduationYearInFuture;

        if (graduationYear < MinGraduationYear)
            return DiplomaInfoErrors.GraduationYearTooOld;

        return new DiplomaInfo(average, field, graduationYear);
    }
}