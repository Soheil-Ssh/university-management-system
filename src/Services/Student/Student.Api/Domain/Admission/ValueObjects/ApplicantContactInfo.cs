namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record ApplicantContactInfo
{
    public MobileNumber Mobile { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public Email Email { get; private set; }
    public Address Address { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ApplicantContactInfo() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private ApplicantContactInfo(MobileNumber mobile,
        PhoneNumber phone,
        Email email,
        Address address)
    {
        Mobile = mobile;
        Phone = phone;
        Email = email;
        Address = address;
    }

    public static Result<ApplicantContactInfo> Create(string mobile,
        string phone,
        string email,
        string province,
        string city,
        string street,
        string buildingNumber,
        string postalCode,
        string? unit,
        string? additionalInfo)
    {
        var mobileResult = MobileNumber.Create(mobile);
        if (mobileResult.IsFailure)
            return mobileResult.Error;

        var phoneResult = PhoneNumber.Create(phone);
        if (phoneResult.IsFailure)
            return phoneResult.Error;

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var addressResult = Address.Create(province, city, street, buildingNumber, postalCode, unit, additionalInfo);
        if (addressResult.IsFailure)
            return addressResult.Error;

        return new ApplicantContactInfo(mobileResult.Data, phoneResult.Data, emailResult.Data, addressResult.Data);
    }
}