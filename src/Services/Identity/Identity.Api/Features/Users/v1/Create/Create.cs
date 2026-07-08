using SharedKernel.Domain.Extensions;

namespace Identity.Api.Features.Users.v1.Create;

public static class Create
{
    public sealed record CreateUserRequest(string UserName,
        string FirstName,
        string LastName,
        string Mobile,
        string Email,
        string Password,
        List<Guid> Roles);

    public sealed record Command(string UserName,
        string FirstName,
        string LastName,
        string Mobile,
        string Email,
        string Password,
        List<Guid> Roles) : ICommand<Result<Guid>>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Mobile).NotEmpty();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(32)
                .Must(pass => pass.All(char.IsLetterOrDigit))
                .Must(password => password.Any(char.IsLetter))
                .Must(password => password.Any(char.IsDigit));
            RuleForEach(x => x.Roles).NotEmpty();
        }
    }

    public class Handler(IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork) : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Check if username already exists
            var usernameExists = await userRepository.IsExistUserName(request.UserName, cancellationToken);
            if (usernameExists)
                return UserErrors.UsernameAlreadyExists;

            // Check if email already exists
            var emailExists = await userRepository.IsExistEmail(request.Email, cancellationToken);
            if (emailExists)
                return UserErrors.EmailAlreadyExists;

            // Validate that all roles exist
            foreach (var roleId in request.Roles)
            {
                var roleExists = await roleRepository.IsExistRole(new RoleId(roleId), cancellationToken);
                if (!roleExists)
                    return RoleErrors.NotFound;
            }

            // Create Email
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
                return emailResult.Error;

            // Create Mobile
            var mobileResult = MobileNumber.Create(request.Mobile);
            if (mobileResult.IsFailure)
                return mobileResult.Error;

            // Create FirstName
            var firstNameResult = Name.Create(request.FirstName).WithPath(nameof(request.FirstName));
            if (firstNameResult.IsFailure)
                return firstNameResult.Error;

            // Create LastName
            var lastNameResult = Name.Create(request.LastName).WithPath(nameof(request.LastName));
            if (lastNameResult.IsFailure)
                return lastNameResult.Error;

            // Hash password
            string passwordHash = passwordHasher.Hash(request.Password);

            // Create the user
            var userResult = User.Create(request.UserName,
                firstNameResult.Data,
                lastNameResult.Data,
                mobileResult.Data,
                emailResult.Data,
                passwordHash);
            if (userResult.IsFailure)
                return userResult.Error;

            var user = userResult.Data;

            // Assign roles to the user
            foreach (var roleId in request.Roles)
            {
                var assignResult = user.AssignRole(new RoleId(roleId));
                if (assignResult.IsFailure)
                    return assignResult.Error;
            }

            // Save the user
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return user.Id.Value;
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/users", async (ISender sender, CreateUserRequest request) =>
            {
                var command = request.Adapt<Command>();
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("CreateUser")
            .WithTags("Users");
        }
    }
}