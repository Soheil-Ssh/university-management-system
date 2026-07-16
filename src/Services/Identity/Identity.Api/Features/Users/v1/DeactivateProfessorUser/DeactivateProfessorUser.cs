using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;
using SharedKernel.Contracts.Integration.Events.Identity.User.v1;

namespace Identity.Api.Features.Users.v1.DeactivateProfessorUser;

public class DeactivateProfessorUser
{
    public sealed class IntegrationEventHandler(ISender sender) : IIntegrationEventHandler<DeactivateProfessorIdentityUserRequestedIntegrationEvent>
    {
        public async Task HandleAsync(DeactivateProfessorIdentityUserRequestedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            var command = new Command(integrationEvent.ProfessorId,
                integrationEvent.IdentityUserId,
                integrationEvent.CorrelationId);

            var result = await sender.Send(command, cancellationToken);
            if (result.IsFailure)
                throw new InvalidOperationException(result.Error.ToString());
        }
    }

    public sealed record Command(Guid ProfessorId, Guid IdentityUserId, Guid CorrelationId) : ICommand<Result>;

    public sealed class Handler(IUserRepository userRepository, IIntegrationEventPublisher integrationEventPublisher, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(new UserId(request.IdentityUserId), cancellationToken);
            if (user is null)
            {
                await PublishFailureAsync(request, "The corresponding identity user was not found.", cancellationToken);
                return Result.Success();
            }

            var deactivateResult = user.Deactivate();
            if (deactivateResult.IsFailure)
            {
                await PublishFailureAsync(request, deactivateResult.Error.ToString(), cancellationToken);
                return Result.Success();
            }

            var succeededEvent = new ProfessorIdentityDeactivationSucceededIntegrationEvent
            {
                CorrelationId = request.CorrelationId,
                ProfessorId = request.ProfessorId,
                IdentityUserId = request.IdentityUserId
            };

            await integrationEventPublisher.PublishAsync(succeededEvent, cancellationToken);

            await unitOfWork.SaveAsync(cancellationToken);

            return Result.Success();
        }


        private async Task PublishFailureAsync(Command request, string reason, CancellationToken cancellationToken)
        {
            var failedEvent = new ProfessorIdentityDeactivationFailedIntegrationEvent
            {
                CorrelationId = request.CorrelationId,
                ProfessorId = request.ProfessorId,
                IdentityUserId = request.IdentityUserId,
                Reason = reason
            };

            await integrationEventPublisher.PublishAsync(failedEvent, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);
        }
    }
}