using Academic.Application.Abstractions.Services;
using Academic.Application.Abstractions.Services.Errors;
using Academic.Domain.Major.Errors;
using Grpc.Core;
using SharedKernel.Contracts.Grpc.Faculty.v1;
using SharedKernel.Domain.Result;

namespace Academic.Infrastructure.Grpc;

public sealed class DepartmentValidationGrpcClient(DepartmentValidationService.DepartmentValidationServiceClient client) : IDepartmentValidationService
{
    public async Task<Result> ValidateForMajorAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new ValidateDepartmentForMajorRequest
            {
                DepartmentId = departmentId.ToString()
            };

            var response = await client.ValidateForMajorAsync(request, cancellationToken: cancellationToken);

            if (response.IsEligible)
                return Result.Success();

            return response.FailureReason switch
            {
                DepartmentMajorEligibilityFailureReason.NotFound =>
                    MajorErrors.DepartmentNotFound,

                DepartmentMajorEligibilityFailureReason.Inactive =>
                    MajorErrors.DepartmentInactive,

                _ => MajorErrors.DepartmentValidationFailed
            };
        }
        catch (RpcException exception) when (exception.StatusCode is StatusCode.Unavailable
                                                 or StatusCode.DeadlineExceeded
                                                 or StatusCode.Internal)
        {
            return DepartmentValidationErrors.Unavailable;
        }
    }
}