namespace Identity.Api.Infrastructure.Persistence.Repositories;

public class UserRepository(IdentityDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower(), cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<bool> IsExistUserNameAsync(string userName, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower(), cancellationToken);

    public async Task<bool> IsExistEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(u => u.Email.Value.ToLower() == email.ToLower(), cancellationToken);

    public async Task<bool> IsExistMobileAsync(MobileNumber mobile, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(u => u.Mobile == mobile, cancellationToken);
}