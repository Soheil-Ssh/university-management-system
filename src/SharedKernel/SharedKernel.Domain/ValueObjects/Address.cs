using SharedKernel.Domain.Result;
using System.Text.RegularExpressions;
using SharedKernel.Domain.ValueObjects.Errors;
using System.Text;

namespace SharedKernel.Domain.ValueObjects;

public sealed record Address
{
    private static readonly Regex Pattern = new(@"\s+", RegexOptions.Compiled);

    private const int ProvinceMaxLength = 50;
    private const int CityMaxLength = 50;
    private const int StreetMaxLength = 200;
    private const int BuildingNumberMaxLength = 20;
    private const int UnitMaxLength = 20;
    private const int AdditionalInfoMaxLength = 500;

    public string Province { get; }
    public string City { get; }
    public string Street { get; }
    public string BuildingNumber { get; }
    public PostalCode PostalCode { get; }
    public string? Unit { get; }
    public string? AdditionalInfo { get; }

    private Address(
        string province,
        string city,
        string street,
        string buildingNumber,
        PostalCode postalCode,
        string? unit,
        string? additionalInfo)
    {
        Province = province;
        City = city;
        Street = street;
        BuildingNumber = buildingNumber;
        PostalCode = postalCode;
        Unit = unit;
        AdditionalInfo = additionalInfo;
    }

    public static Result<Address> Create(
        string province,
        string city,
        string street,
        string buildingNumber,
        string postalCode,
        string? unit = null,
        string? additionalInfo = null)
    {
        province = Normalize(province) ?? string.Empty;
        city = Normalize(city) ?? string.Empty;
        street = Normalize(street) ?? string.Empty;
        buildingNumber = Normalize(buildingNumber) ?? string.Empty;
        unit = Normalize(unit);
        additionalInfo = Normalize(additionalInfo);

        if (string.IsNullOrWhiteSpace(province))
            return AddressErrors.ProvinceEmpty;

        if (string.IsNullOrWhiteSpace(city))
            return AddressErrors.CityEmpty;

        if (string.IsNullOrWhiteSpace(street))
            return AddressErrors.StreetEmpty;

        if (province.Length > ProvinceMaxLength)
            return AddressErrors.ProvinceTooLong;

        if (city.Length > CityMaxLength)
            return AddressErrors.CityTooLong;

        if (street.Length > StreetMaxLength)
            return AddressErrors.StreetTooLong;

        if (!string.IsNullOrWhiteSpace(buildingNumber) && buildingNumber.Length > BuildingNumberMaxLength)
            return AddressErrors.BuildingNumberTooLong;

        if (!string.IsNullOrWhiteSpace(unit) && unit.Length > UnitMaxLength)
            return AddressErrors.UnitTooLong;

        if (!string.IsNullOrWhiteSpace(additionalInfo) && additionalInfo.Length > AdditionalInfoMaxLength)
            return AddressErrors.AdditionalInfoTooLong;

        var postalCodeResult = PostalCode.Create(postalCode);
        if (postalCodeResult.IsFailure)
            return postalCodeResult.Error;

        return new Address(
            province,
            city,
            street,
            buildingNumber,
            postalCodeResult.Data,
            unit,
            additionalInfo);
    }

    private static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        value = value
            .Normalize(NormalizationForm.FormC)
            .Trim();

        return Pattern.Replace(value, " ");
    }

    public override string ToString()
    {
        var parts = new List<string>
        {
            Province,
            City,
            Street,
            BuildingNumber,
        };

        if (!string.IsNullOrWhiteSpace(Unit))
            parts.Add(Unit);

        parts.Add(PostalCode.Value);

        if (!string.IsNullOrWhiteSpace(AdditionalInfo))
            parts.Add(AdditionalInfo);

        return string.Join(", ", parts);
    }
}