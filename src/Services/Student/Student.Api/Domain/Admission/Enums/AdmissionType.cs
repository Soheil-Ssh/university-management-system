namespace Student.Api.Domain.Admission.Enums;

public enum AdmissionType
{
    /// <summary>
    /// روزانه (دولتی) - با آزمون سراسری، شهریه یارانه‌ای، تعهد به خدمت (طرح نیروی انسانی)
    /// </summary>
    Daytime = 0,

    /// <summary>
    /// شبانه (نوبت دوم) - شهریه‌پرداز، بدون تعهد خدمتی، مدرک معادل روزانه
    /// </summary>
    Nighttime = 1,

    /// <summary>
    /// دانشجوی بین‌المللی (غیرایرانی) - شهریه ویژه، نیاز به ویزای تحصیلی
    /// </summary>
    International = 2,
}