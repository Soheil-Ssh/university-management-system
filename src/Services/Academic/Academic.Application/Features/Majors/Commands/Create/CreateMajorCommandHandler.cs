namespace Academic.Application.Features.Majors.Commands.Create;

public sealed class CreateMajorCommandHandler(IMajorRepository majorRepository, IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateMajorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMajorCommand request, CancellationToken cancellationToken)
    {
        // TODO Check department exits with grpc

        var departmentId = new DepartmentId(request.DepartmentId);

        var majorExists = await majorRepository.ExistsByDepartmentIdAsync(departmentId, cancellationToken);

        if (majorExists)
            return MajorErrors.DepartmentAlreadyHasMajor;

        var nextCodeNumber = await majorRepository.GetNextMajorCodeAsync(cancellationToken);
        var codeResult = MajorCode.Create(nextCodeNumber);
        if (codeResult.IsFailure)
            return codeResult.Error;

        var majorResult = Major.Create(departmentId, codeResult.Data, request.Name, request.Description);
        if (majorResult.IsFailure)
            return majorResult.Error;

        await majorRepository.AddAsync(
            majorResult.Data,
            cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);

        return majorResult.Data.Id.Value;
    }
}