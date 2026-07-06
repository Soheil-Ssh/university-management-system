namespace Student.Api.Domain.Admission.Enums;

public enum AdmissionMethod
{
    /// <summary>
    /// پذیرش از طریق کنکور سراسری (آزمون سراسری سازمان سنجش)
    /// </summary>
    NationalExam = 0,

    /// <summary>
    /// پذیرش صرفاً بر اساس سوابق تحصیلی (بدون کنکور)
    /// </summary>
    AcademicRecord = 1,

    /// <summary>
    /// پذیرش دانشجویان بین‌المللی (غیر ایرانی) از طریق بورسیه
    /// </summary>
    InternationalScholarship = 2,
}
