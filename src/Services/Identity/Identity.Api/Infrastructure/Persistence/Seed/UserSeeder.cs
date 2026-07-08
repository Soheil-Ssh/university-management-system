using Identity.Api.Infrastructure.Authorization.Roles;
using Identity.Api.Infrastructure.Persistence.Options;
using Microsoft.Extensions.Options;
using SharedKernel.Domain.Extensions;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Infrastructure.Persistence.Seed;

public class UserSeeder(IdentityDbContext context,
    IPasswordHasher passwordHasher,
    IOptions<SuperAdminOptions> options) : IDataSeeder
{
    public int Order => 3;
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var config = options.Value;

        var existingUser = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserName == config.UserName, cancellationToken);

        var superAdminRole = await context.Roles
            .FirstOrDefaultAsync(u => u.Name == SystemRolesCatalog.SuperAdmin.Name, cancellationToken);

        if (superAdminRole is null)
            throw new Exception("SuperAdmin role was not found.");

        if (existingUser is null)
        {
            var emailResult = Email.Create(config.Email);
            if (emailResult.IsFailure)
                throw new Exception(emailResult.Error.ToString());

            var firstNameResult = Name.Create(config.FirstName).WithPath(nameof(config.FirstName));
            if (firstNameResult.IsFailure)
                throw new Exception(firstNameResult.Error.ToString());

            var lastNameResult = Name.Create(config.LastName).WithPath(nameof(config.LastName));
            if (lastNameResult.IsFailure)
                throw new Exception(lastNameResult.Error.ToString());

            var mobileResult = MobileNumber.Create(config.Mobile);
            if (mobileResult.IsFailure)
                throw new Exception(mobileResult.Error.ToString());

            string passwordHash = passwordHasher.Hash(config.Password);

            var userResult = User.Create(config.UserName, firstNameResult.Data, lastNameResult.Data, mobileResult.Data, emailResult.Data, passwordHash);
            if (userResult.IsFailure)
                throw new Exception(userResult.Error.ToString());

            var user = userResult.Data;

            var assignResult = user.AssignRole(superAdminRole.Id);

            if (assignResult.IsFailure)
                throw new Exception(assignResult.Error.ToString());

            context.Users.Add(user);

            await context.SaveChangesAsync(cancellationToken);
            return;
        }


        if (existingUser.UserRoles.All(x => x.RoleId != superAdminRole.Id))
        {
            var assignResult = existingUser.AssignRole(superAdminRole.Id);

            if (assignResult.IsFailure)
                throw new Exception(assignResult.Error.ToString());

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}