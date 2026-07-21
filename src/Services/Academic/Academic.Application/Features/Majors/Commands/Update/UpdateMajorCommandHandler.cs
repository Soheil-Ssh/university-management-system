namespace Academic.Application.Features.Majors.Commands.Update;

public sealed class UpdateMajorCommandHandler(IMajorRepository majorRepository, IUnitOfWork unitOfWork, IDepartmentValidationService departmentValidationService)
    : ICommandHandler<UpdateMajorCommand, Result>
{
    public async Task<Result> Handle(UpdateMajorCommand request, CancellationToken cancellationToken)
    {
        var majorId = new MajorId(request.MajorId);
        var major = await majorRepository.GetByIdAsync(majorId, cancellationToken);
        if (major is null)
            return MajorErrors.NotFound;

        var departmentId = new DepartmentId(request.DepartmentId);

        if (major.DepartmentId != departmentId)
        {
            var majorExists = await majorRepository.ExistsByDepartmentIdAsync(departmentId, majorId, cancellationToken);
            if (majorExists)
                return MajorErrors.DepartmentAlreadyHasMajor;

            var departmentValidationResult = await departmentValidationService.ValidateForMajorAsync(departmentId.Value, cancellationToken);
            if (departmentValidationResult.IsFailure)
                return departmentValidationResult.Error;
        }

        var updateResult = major.Update(departmentId, request.Name, request.Description);
        if (updateResult.IsFailure)
            return updateResult.Error;

        await unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}