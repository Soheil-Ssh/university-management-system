using SharedKernel.Domain.Error;

namespace SharedKernel.Domain.ValueObjects.Errors;

public class AddressErrors
{

    // Province errors
    public static readonly Error.Error ProvinceEmpty = new("Address.Province.Empty", "Province cannot be empty.", ErrorType.Validation);
    public static readonly Error.Error ProvinceTooLong =
        new("Address.Province.TooLong", "Province name is too long.", ErrorType.Validation);

    // City errors
    public static readonly Error.Error CityEmpty = new("Address.City.Empty", "City .", ErrorType.Validation);
    public static readonly Error.Error CityTooLong = 
        new("Address.City.TooLong", "City name is too long.", ErrorType.Validation);

    // Street errors
    public static readonly Error.Error StreetEmpty=
        new("Address.Street.Empty", "Street cannot be empty.", ErrorType.Validation);
    public static readonly Error.Error StreetTooLong = 
        new("Address.Street.TooLong", "Street name is too long.", ErrorType.Validation);

    public static readonly Error.Error BuildingNumberTooLong =
        new("Address.BuildingNumber.TooLong", "Building number is too long.", ErrorType.Validation);

    public static readonly Error.Error UnitTooLong =
        new("Address.Unit.TooLong", "Unit is too long.", ErrorType.Validation);

    public static readonly Error.Error AdditionalInfoTooLong =
        new("Address.AdditionalInfo.TooLong", "Additional info name is too long.", ErrorType.Validation);
}