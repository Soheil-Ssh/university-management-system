using SharedKernel.Domain.Extensions;
using Student.Api.Domain.Admission.Errors;

namespace Student.Api.Domain.Admission.ValueObjects;

public sealed record EmergencyContact
{
    private const int MaxRelationLength = 100;

    public Name FirstName { get; }
    public Name LastName { get; }
    public string Relation { get; }
    public MobileNumber Mobile { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private EmergencyContact() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private EmergencyContact(Name FirstName,
        Name LastName,
        string Relation,
        MobileNumber Mobile)
    {
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Relation = Relation;
        this.Mobile = Mobile;
    }

    public static Result<EmergencyContact> Create(string firstName,
        string lastName,
        string relation,
        string mobile)
    {
        var firstNameResult = Name.Create(firstName).WithPath(nameof(EmergencyContact) + nameof(FirstName));
        if (firstNameResult.IsFailure)
            return firstNameResult.Error;

        var lastNameResult = Name.Create(lastName).WithPath(nameof(EmergencyContact) + nameof(LastName));
        if (lastNameResult.IsFailure)
            return lastNameResult.Error;

        if (string.IsNullOrWhiteSpace(relation))
            return EmergencyContactErrors.RelationEmpty;

        relation = relation.Trim();
        if(relation.Length > MaxRelationLength)
            return EmergencyContactErrors.RelationTooLong;

        var mobileResult = MobileNumber.Create(mobile).WithPath(nameof(EmergencyContact) + nameof(Mobile));
        if (mobileResult.IsFailure)
            return mobileResult.Error;

        return new EmergencyContact(firstNameResult.Data, lastNameResult.Data, relation, mobileResult.Data);
    }
}