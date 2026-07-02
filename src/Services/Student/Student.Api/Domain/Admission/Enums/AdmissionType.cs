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
    /// غیرانتفاعی (دولتی غیرانتفاعی) - شهریه بالا، بدون تعهد خدمتی
    /// </summary>
    NonGovernmental = 2,

    /// <summary>
    /// دانشجوی بین‌المللی (غیرایرانی) - شهریه ویژه، نیاز به ویزای تحصیلی
    /// </summary>
    International = 3,

    /// <summary>
    /// انتقالی از دانشگاه‌های داخل کشور (با حفظ وضعیت قبلی یا تغییر آن)
    /// </summary>
    Transfer = 4,

    /// <summary>
    /// دانشجوی میهمان/تبادلی (فقط برای یک یا دو ترم)
    /// </summary>
    Exchange = 5
}