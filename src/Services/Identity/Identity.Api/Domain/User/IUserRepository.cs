namespace Identity.Api.Domain.User;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> IsExistUserName(string userName, CancellationToken cancellationToken = default);
    Task<bool> IsExistEmail(string email, CancellationToken cancellationToken = default);
}