using SharedKernel.Domain.Enums;
using SharedKernel.Domain.Extensions;
using SharedKernel.Domain.Identifiers;
using Student.Api.Domain.Admission.Errors;

namespace Student.Api.Domain.Admission.ValueObjects;

public record ApplicantPersonalInfo
{
    private const int MaxPlaceLength = 100;
    private static readonly DateTime MinBirthDate = new(1900, 1, 1);

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EnFirstName { get; private set; }
    public string EnLastName { get; private set; }
    public string NationalCode { get; private set; }
    public string BirthPlace { get; private set; }
    public string IssuePlace { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Gender Gender { get; private set; }
    public MaritalStatus MaritalStatus { get; private set; }
    public FileId PersonalImageFileId { get; private set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ApplicantPersonalInfo() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private ApplicantPersonalInfo(string firstName,
        string lastName,
        string enFirstName,
        string enLastName,
        string nationalCode,
        string birthPlace,
        string issuePlace,
        DateTime birthDate,
        Gender gender,
        MaritalStatus maritalStatus,
        FileId personalImageFileId)
    {
        FirstName = firstName;
        LastName = lastName;
        EnFirstName = enFirstName;
        EnLastName = enLastName;
        NationalCode = nationalCode;
        BirthPlace = birthPlace;
        IssuePlace = issuePlace;
        BirthDate = birthDate;
        Gender = gender;
        MaritalStatus = maritalStatus;
        PersonalImageFileId = personalImageFileId;
    }

    public static Result<ApplicantPersonalInfo> Create(string firstName,
        string lastName,
        string enFirstName,
        string enLastName,
        string nationalCode,
        string birthPlace,
        string issuePlace,
        DateTime birthDate,
        Gender gender,
        MaritalStatus maritalStatus,
        FileId personalImageFileId)
    {
        var firstNameResult = Name.Create(firstName).WithPath(nameof(FirstName));
        if (firstNameResult.IsFailure)
            return firstNameResult.Error;

        var lastNameResult = Name.Create(lastName).WithPath(nameof(LastName));
        if (lastNameResult.IsFailure)
            return lastNameResult.Error;

        var enFirstNameResult = Name.Create(enFirstName).WithPath(nameof(EnFirstName));
        if (enFirstNameResult.IsFailure)
            return enFirstNameResult.Error;

        var enLastNameResult = Name.Create(enLastName).WithPath(nameof(EnLastName));
        if (enLastNameResult.IsFailure)
            return enLastNameResult.Error;

        var nationalCodeResult = SharedKernel.Domain.ValueObjects.NationalCode.Create(nationalCode);
        if (nationalCodeResult.IsFailure)
            return nationalCodeResult.Error;

        if (string.IsNullOrWhiteSpace(birthPlace))
            return ApplicantPersonalInfoErrors.BirthPlaceEmpty;

        birthPlace = birthPlace.Trim();
        if (birthPlace.Length > MaxPlaceLength)
            return ApplicantPersonalInfoErrors.BirthPlaceTooLong;

        if (string.IsNullOrWhiteSpace(issuePlace))
            return ApplicantPersonalInfoErrors.IssuePlaceEmpty;

        issuePlace = issuePlace.Trim();
        if (issuePlace.Length > MaxPlaceLength)
            return ApplicantPersonalInfoErrors.IssuePlaceTooLong;

        if (birthDate > DateTime.UtcNow.Date)
            return ApplicantPersonalInfoErrors.BirthDateFuture;

        if (birthDate < MinBirthDate)
            return ApplicantPersonalInfoErrors.BirthDateTooOld;

        return new ApplicantPersonalInfo(
            firstNameResult.Data.Value,
            lastNameResult.Data.Value,
            enFirstNameResult.Data.Value,
            enLastNameResult.Data.Value,
            nationalCodeResult.Data.Value,
            birthPlace,
            issuePlace,
            birthDate,
            gender,
            maritalStatus,
            personalImageFileId);
    }
}