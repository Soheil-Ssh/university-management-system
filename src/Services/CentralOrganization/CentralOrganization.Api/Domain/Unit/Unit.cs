namespace CentralOrganization.Api.Domain.Unit;

public sealed class Unit : AggregateRoot<UnitId>
{
    private const int MaxNameLength = 150;
    private const int MaxDescriptionLength = 500;

    public string Name { get; private set; }
    public UnitCode Code { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

#pragma warning disable CS8618
    private Unit() { }
#pragma warning restore CS8618

    private Unit(UnitId id, string name, UnitCode code, string? description)
        : base(id)
    {
        Name = name;
        Code = code;
        Description = description;
        IsActive = true;
    }

    public static Result<Unit> Create(UnitCode code, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return UnitErrors.NameEmpty;

        name = name.Trim();
        if (name.Length > MaxNameLength)
            return UnitErrors.NameTooLong;

        description = description?.Trim();
        if (description is not null && description.Length > MaxDescriptionLength)
            return UnitErrors.DescriptionTooLong;

        return new Unit(UnitId.New(), name, code, description);
    }

    public Result Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return UnitErrors.NameEmpty;

        name = name.Trim();
        if (name.Length > MaxNameLength)
            return UnitErrors.NameTooLong;

        description = description?.Trim();
        if (description is not null && description.Length > MaxDescriptionLength)
            return UnitErrors.DescriptionTooLong;

        Name = name;
        Description = description;

        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
            return Result.Success();

        IsActive = true;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Success();

        IsActive = false;
        return Result.Success();
    }
}