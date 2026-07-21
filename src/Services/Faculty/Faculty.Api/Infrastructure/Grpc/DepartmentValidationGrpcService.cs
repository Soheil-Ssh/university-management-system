using Grpc.Core;
using SharedKernel.Contracts.Grpc.Faculty.v1;

namespace Faculty.Api.Infrastructure.Grpc;

public sealed class DepartmentValidationGrpcService(FacultyDbContext dbContext) : DepartmentValidationService.DepartmentValidationServiceBase
{
    public override async Task<ValidateDepartmentForMajorResponse> ValidateForMajor(ValidateDepartmentForMajorRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.DepartmentId, out var departmentId) || departmentId == Guid.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "DepartmentId is invalid."));

        var department = await dbContext.Departments
            .AsNoTracking()
            .Where(x => x.Id == new DepartmentId(departmentId))
            .Select(x => new { x.IsActive })
            .SingleOrDefaultAsync(context.CancellationToken);

        if (department is null)
        {
            return new ValidateDepartmentForMajorResponse
            {
                IsEligible = false,
                FailureReason = DepartmentMajorEligibilityFailureReason.NotFound
            };
        }

        if (!department.IsActive)
        {
            return new ValidateDepartmentForMajorResponse
            {
                IsEligible = false,
                FailureReason = DepartmentMajorEligibilityFailureReason.Inactive
            };
        }

        return new ValidateDepartmentForMajorResponse
        {
            IsEligible = true,
            FailureReason = DepartmentMajorEligibilityFailureReason.Unspecified
        };
    }
}