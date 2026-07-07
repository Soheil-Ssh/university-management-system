using CentralOrganization.Api.Application.Abstractions.Identity;
using CentralOrganization.Api.Application.Abstractions.Identity.Errors;
using Grpc.Core;
using SharedKernel.Contracts.Grpc.Identity.v1;

namespace CentralOrganization.Api.Infrastructure.Grpc;

public class IdentityUserGrpcClient(IdentityUserService.IdentityUserServiceClient client) : IIdentityUserClient
{
    public async Task<Result<Guid>> CreateEmployeeUserAsync(CreateEmployeeUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.CreateCentralOrganizationEmployeeUserAsync(
                new CreateCentralOrganizationEmployeeUserRequest
                {
                    Username = request.UserName,
                    Email = request.Email,
                    Password = request.Password
                },
                deadline: DateTime.UtcNow.AddSeconds(3),
                cancellationToken: cancellationToken);

            if (!response.IsSuccess)
                return new Error(response.ErrorCode, response.ErrorMessage, ErrorType.Failure);

            if (!Guid.TryParse(response.UserId, out var userId))
                return IdentityUserClientErrors.InvalidResponse;

            return userId;
        }
        catch (RpcException exception) when (exception.StatusCode is StatusCode.Unavailable or StatusCode.Internal)
        {
            return IdentityUserClientErrors.ServiceUnavailable;
        }
        catch (RpcException exception) when (exception.StatusCode is StatusCode.DeadlineExceeded)
        {
            return IdentityUserClientErrors.DeadlineExceeded;
        }
    }
}