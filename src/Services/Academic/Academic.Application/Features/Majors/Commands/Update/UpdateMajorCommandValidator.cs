namespace Academic.Application.Features.Majors.Commands.Update;

public sealed class UpdateMajorCommandValidator : AbstractValidator<UpdateMajorCommand>
{
    public UpdateMajorCommandValidator()
    {
        RuleFor(x => x.MajorId).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}