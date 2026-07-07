namespace CentralOrganization.Api.Application.Abstractions.Identity;

public interface IIdentityUserClient
{
    Task<Result<Guid>> CreateEmployeeUserAsync(CreateEmployeeUserRequest request,
        CancellationToken cancellationToken = default);
}