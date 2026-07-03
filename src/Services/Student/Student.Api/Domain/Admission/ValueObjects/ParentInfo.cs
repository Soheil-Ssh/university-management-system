using Student.Api.Domain.Admission.Errors;

namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record ParentInfo
{
    public Name FirstName { get; }
    public Name LastName { get; }
    public NationalCode NationalCode { get; }
    public MobileNumber Mobile { get; }

    private ParentInfo(Name firstName, Name lastName, NationalCode nationalCode, MobileNumber mobile)
    {
        FirstName = firstName;
        LastName = lastName;
        NationalCode = nationalCode;
        Mobile = mobile;
    }

    public static Result<ParentInfo> Create(string firstName, string lastName, string nationalCode, string mobile)
    {
        var firstNameResult = Name.Create(firstName);
        if (firstNameResult.IsFailure)
            return ParentInfoErrors.FirstNameInvalid;

        var lastNameResult = Name.Create(lastName);
        if (lastNameResult.IsFailure)
            return ParentInfoErrors.LastNameInvalid;

        var nationalCodeResult = NationalCode.Create(nationalCode);
        if (nationalCodeResult.IsFailure)
            return ParentInfoErrors.NationalCodeInvalid;

        var mobileResult = MobileNumber.Create(mobile);
        if (mobileResult.IsFailure)
            return ParentInfoErrors.MobileInvalid;

        return new ParentInfo(firstNameResult.Data, lastNameResult.Data, nationalCodeResult.Data, mobileResult.Data);
    }
}