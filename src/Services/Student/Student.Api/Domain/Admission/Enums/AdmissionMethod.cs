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
    /// پذیرش بر اساس سابقه تحصیلی و سهمیه (روش ترکیبی)
    /// </summary>
    AcademicRecordWithQuota = 2,

    /// <summary>
    /// پذیرش دانشجویان بین‌المللی (غیر ایرانی) از طریق بورسیه
    /// </summary>
    InternationalScholarship = 3,

    /// <summary>
    /// پذیرش آزاد دانشجویان بین‌المللی (غیر ایرانی)
    /// </summary>
    InternationalFree = 4,

    /// <summary>
    /// پذیرش دانشجویان استعداد درخشان (ویژه کارشناسی ارشد و دکترا، در کارشناسی استفاده نمی‌شود)
    /// </summary>
    Talented = 5,

    /// <summary>
    /// پذیرش انتقالی از دانشگاه‌های خارج از کشور
    /// </summary>
    TransferFromAbroad = 6
}
