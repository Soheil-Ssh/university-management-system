namespace Identity.Api.Domain.User;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> IsExistUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<bool> IsExistEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsExistMobileAsync(MobileNumber mobile, CancellationToken cancellationToken = default);
}