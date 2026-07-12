using Identity.Api.Infrastructure.Authorization.Roles;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;
using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1;
using SharedKernel.Contracts.Integration.Events.Identity.User.v1;

namespace Identity.Api.Features.Users.v1.ProvisionProfessorUser;

public static class ProvisionProfessorUser
{
    public sealed class IntegrationEventHandler(ISender sender) : IIntegrationEventHandler<CreateProfessorIdentityUserRequestedIntegrationEvent>
    {
        public async Task HandleAsync(CreateProfessorIdentityUserRequestedIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
        {
            var command = new Command(
                integrationEvent.ProfessorId,
                integrationEvent.NationalCode,
                integrationEvent.FirstName,
                integrationEvent.LastName,
                integrationEvent.Email,
                integrationEvent.MobileNumber,
                integrationEvent.CorrelationId);
            await sender.Send(command, cancellationToken);
        }
    }

    public sealed record Command(
        Guid ProfessorId,
        string NationalCode,
        string FirstName,
        string LastName,
        string Email,
        string MobileNumber,
        Guid CorrelationId) : ICommand<Result>;

    public sealed class Handler(IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IIntegrationEventPublisher integrationEventPublisher,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await userRepository.GetByUserNameAsync(request.NationalCode, cancellationToken);
            if (existingUser is not null)
            {
                await integrationEventPublisher.PublishAsync(
                    new ProfessorIdentityProvisioningSucceededIntegrationEvent
                    {
                        ProfessorId = request.ProfessorId,
                        IdentityUserId = existingUser.Id.Value,
                        CorrelationId = request.CorrelationId
                    },
                    cancellationToken);
                return Result.Success();
            }

            var nationalCodeResult = NationalCode.Create(request.NationalCode);
            if (nationalCodeResult.IsFailure)
            {
                await PublishFailedAsync(request, nationalCodeResult.Error.Description, cancellationToken);
                return Result.Success();
            }

            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure)
            {
                await PublishFailedAsync(request, emailResult.Error.Description, cancellationToken);
                return Result.Success();
            }

            var emailExists = await userRepository.IsExistEmailAsync(emailResult.Data.Value, cancellationToken);
            if (emailExists)
            {
                await PublishFailedAsync(request, "A user with this email already exists.", cancellationToken);
                return Result.Success();
            }

            var mobileResult = MobileNumber.Create(request.MobileNumber);
            if (mobileResult.IsFailure)
            {
                await PublishFailedAsync(request, mobileResult.Error.Description, cancellationToken);
                return Result.Success();
            }

            var firstNameResult = Name.Create(request.FirstName).WithPath(nameof(request.FirstName));
            if (firstNameResult.IsFailure)
            {
                await PublishFailedAsync(request, firstNameResult.Error.Description, cancellationToken);
                return Result.Success();
            }

            var lastNameResult = Name.Create(request.LastName).WithPath(nameof(request.LastName));
            if (lastNameResult.IsFailure)
            {
                await PublishFailedAsync(request, lastNameResult.Error.Description, cancellationToken);
                return Result.Success();
            }

            var mobileExists = await userRepository.IsExistMobileAsync(mobileResult.Data, cancellationToken);
            if (mobileExists)
            {
                await PublishFailedAsync(request, "A user with this mobile number already exists.", cancellationToken);
                return Result.Success();
            }

            var passwordHash = passwordHasher.Hash(request.NationalCode);
            var userResult = User.Create(request.NationalCode,
                firstNameResult.Data,
                lastNameResult.Data,
                mobileResult.Data,
                emailResult.Data,
                passwordHash);
            if (userResult.IsFailure)
            {
                await PublishFailedAsync(request, userResult.Error.Description, cancellationToken);
                return Result.Success();
            }

            var user = userResult.Data;

            var role = await roleRepository.GetByNameAsync(SystemRolesCatalog.Professor.Name, cancellationToken);
            if (role is null)
            {
                await PublishFailedAsync(request, "Professor system rule was not found.", cancellationToken);
                return Result.Success();
            }

            var assignResult = user.AssignRole(role.Id);
            if (assignResult.IsFailure)
            {
                await PublishFailedAsync(request, assignResult.Error.Description, cancellationToken);
                return Result.Success();
            }

            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            await integrationEventPublisher.PublishAsync(
                new ProfessorIdentityProvisioningSucceededIntegrationEvent()
                {
                    ProfessorId = request.ProfessorId,
                    IdentityUserId = user.Id.Value,
                    CorrelationId = request.CorrelationId
                },
                cancellationToken);

            return Result.Success();
        }

        private async Task PublishFailedAsync(Command request, string? reason, CancellationToken cancellationToken)
        {
            await integrationEventPublisher.PublishAsync(
                new EmployeeIdentityProvisioningFailedIntegrationEvent
                {
                    EmployeeId = request.ProfessorId,
                    Reason = reason ?? string.Empty,
                    CorrelationId = request.CorrelationId
                },
                cancellationToken);
        }
    }
}