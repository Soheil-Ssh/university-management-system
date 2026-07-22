namespace Academic.Application.Features.Majors.Queries.GetAll;

public sealed class GetAllMajorsQueryValidator : AbstractValidator<GetAllMajorsQuery>
{
    public GetAllMajorsQueryValidator()
    {
        RuleFor(x => x.DepartmentId).NotEmpty().When(x => x.DepartmentId.HasValue);
        RuleFor(x => x.Code).MaximumLength(15);
        RuleFor(x => x.Name).MaximumLength(150);
        RuleFor(x => x.SortBy).IsInEnum();
        RuleFor(x => x.SortDirection).IsInEnum();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x)
            .Must(x =>
                !x.FromCreatedAt.HasValue ||
                !x.ToCreatedAt.HasValue ||
                x.FromCreatedAt <= x.ToCreatedAt)
            .WithMessage(
                "FromCreatedAt cannot be greater than ToCreatedAt.");
        RuleFor(x => x)
            .Must(x =>
                !x.FromUpdatedAt.HasValue ||
                !x.ToUpdatedAt.HasValue ||
                x.FromUpdatedAt <= x.ToUpdatedAt)
            .WithMessage(
                "FromUpdatedAt cannot be greater than ToUpdatedAt.");
    }
}