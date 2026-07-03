using Student.Api.Domain.Admission.Errors;

namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record ParentInfo
{
    public Name FirstName { get; }
    public Name LastName { get; }
    public NationalCode NationalCode { get; }
    public MobileNumber Mobile { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ParentInfo() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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