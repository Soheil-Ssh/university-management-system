using Grpc.Core;
using SharedKernel.Contracts.Grpc.Identity.v1;

namespace Identity.Api.Infrastructure.Grpc;

public class IdentityUserGrpcService(IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IdentityUserService.IdentityUserServiceBase
{
    public override async Task<CreateCentralOrganizationEmployeeUserResponse> CreateCentralOrganizationEmployeeUser(
        CreateCentralOrganizationEmployeeUserRequest request, ServerCallContext context)
    {
        var cancellationToken = context.CancellationToken;

        // Check if username already exists
        var usernameExists = await userRepository.IsExistUserName(request.Username, cancellationToken);
        if (usernameExists)
            return Failure("Identity.CentralOrganization.Username.AlreadyExists", "User name is already exists.");

        // Check if email already exists
        var emailExists = await userRepository.IsExistEmail(request.Email, cancellationToken);
        if (emailExists)
            return Failure("Identity.CentralOrganization.Email.AlreadyExists", "User name is already exists.");

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Failure($"Identity.CentralOrganization.{emailResult.Error.Code}", emailResult.Error.Description ?? "Email is not valid.");

        var passwordHash = passwordHasher.Hash(request.Password);
        var userResult = User.Create(request.Username, emailResult.Data, passwordHash);
        if (userResult.IsFailure)
            return Failure($"Identity.CentralOrganization.{userResult.Error.Code}", userResult.Error.Description ?? "User was not created.");

        await userRepository.AddAsync(userResult.Data, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return new CreateCentralOrganizationEmployeeUserResponse
        {
            IsSuccess = true,
            UserId = userResult.Data.Id.Value.ToString()
        };
    }

    private static CreateCentralOrganizationEmployeeUserResponse Failure(string code, string message)
        => new()
        {
            IsSuccess = false,
            ErrorCode = code,
            ErrorMessage = message
        };
}