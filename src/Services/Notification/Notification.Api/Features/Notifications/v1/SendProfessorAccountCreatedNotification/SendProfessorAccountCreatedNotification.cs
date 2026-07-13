using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1;

namespace Notification.Api.Features.Notifications.v1.SendProfessorAccountCreatedNotification;

public static class SendProfessorAccountCreatedNotification
{
    public sealed class IntegrationEventHandler(ISender sender) : IIntegrationEventHandler<ProfessorAccountCreatedIntegrationEvent>
    {
        public async Task HandleAsync(ProfessorAccountCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            var command = new Command(
                integrationEvent.EventId,
                integrationEvent.ProfessorId,
                integrationEvent.IdentityUserId,
                integrationEvent.FirstName,
                integrationEvent.LastName,
                integrationEvent.MobileNumber,
                integrationEvent.UserName,
                integrationEvent.TemporaryPassword,
                integrationEvent.CorrelationId);

            var result = await sender.Send(command, cancellationToken);
            if (result.IsFailure)
                throw new InvalidOperationException(result.Error.ToString());
        }
    }

    public sealed record Command(Guid EventId,
        Guid ProfessorId,
        Guid IdentityUserId,
        string FirstName,
        string LastName,
        string MobileNumber,
        string UserName,
        string TemporaryPassword,
        Guid CorrelationId) : ICommand<Result>;

    public sealed class Handler(INotificationDispatcher notificationDispatcher,
       INotificationRepository notificationRepository,
       IUnitOfWork unitOfWork,
       ILogger<Handler> logger)
       : ICommandHandler<Command, Result>
    {
        private const string SourceService = "Faculty.Api";
        private const string SourceEventType = nameof(ProfessorAccountCreatedIntegrationEvent);

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var alreadyProcessed = await notificationRepository.ExistsAsync(request.EventId, NotificationChannel.Sms, cancellationToken);
            if (alreadyProcessed)
            {
                logger.LogInformation(
                    "Duplicate professor notification event ignored. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "SourceEventId={SourceEventId} SourceEventType={SourceEventType} " +
                    "Channel={Channel} ProfessorId={ProfessorId}",
                    "DuplicateProfessorNotificationEventIgnored",
                    true,
                    request.EventId,
                    SourceEventType,
                    NotificationChannel.Sms,
                    request.ProfessorId);

                return Result.Success();
            }

            var notificationResult = NotificationMessage.CreateSms(
                recipientMobile: request.MobileNumber,
                body: CreateSmsBody(request),
                priority: NotificationPriority.High,
                recipientUserId: request.IdentityUserId,
                correlationId: request.CorrelationId.ToString(),
                sourceService: SourceService,
                sourceEventId: request.EventId,
                sourceEventType: SourceEventType);

            if (notificationResult.IsFailure)
            {
                logger.LogError(
                    "Professor account notification creation failed. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "SourceEventId={SourceEventId} ProfessorId={ProfessorId} " +
                    "ErrorCode={ErrorCode} ErrorDescription={ErrorDescription}",
                    "ProfessorAccountNotificationCreationFailed",
                    true,
                    request.EventId,
                    request.ProfessorId,
                    notificationResult.Error.Code,
                    notificationResult.Error.Description);

                return notificationResult.Error;
            }

            var notification = notificationResult.Data;

            await notificationRepository.AddAsync(notification, cancellationToken);

            var dispatchResult = await notificationDispatcher.DispatchAsync(notification, cancellationToken);
            if (dispatchResult.IsFailure)
            {
                logger.LogError(
                    "Professor account notification dispatch could not be processed. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "NotificationMessageId={NotificationMessageId} SourceEventId={SourceEventId} " +
                    "ProfessorId={ProfessorId} ErrorCode={ErrorCode} " +
                    "ErrorDescription={ErrorDescription}",
                    "ProfessorAccountNotificationDispatchError",
                    true,
                    notification.Id.Value,
                    request.EventId,
                    request.ProfessorId,
                    dispatchResult.Error.Code,
                    dispatchResult.Error.Description);

                return dispatchResult.Error;
            }

            await unitOfWork.SaveAsync(cancellationToken);

            if (dispatchResult.Data.Status == NotificationDispatchStatus.Failed)
            {
                logger.LogWarning(
                    "Professor account notification saved as failed. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "NotificationMessageId={NotificationMessageId} " +
                    "DeliveryAttemptId={DeliveryAttemptId} SourceEventId={SourceEventId} " +
                    "ProfessorId={ProfessorId} IdentityUserId={IdentityUserId} " +
                    "Channel={Channel} Provider={Provider} DispatchStatus={DispatchStatus} " +
                    "ErrorCode={ErrorCode} ErrorMessage={ErrorMessage}",
                    "ProfessorAccountNotificationFailed",
                    true,
                    dispatchResult.Data.NotificationMessageId.Value,
                    dispatchResult.Data.DeliveryAttemptId.Value,
                    request.EventId,
                    request.ProfessorId,
                    request.IdentityUserId,
                    dispatchResult.Data.Channel,
                    dispatchResult.Data.Provider,
                    dispatchResult.Data.Status,
                    dispatchResult.Data.ErrorCode,
                    dispatchResult.Data.ErrorMessage);

                return Result.Success();
            }

            logger.LogInformation(
                "Professor account notification processed successfully. " +
                "EventName={EventName} NotificationLog={NotificationLog} " +
                "NotificationMessageId={NotificationMessageId} " +
                "DeliveryAttemptId={DeliveryAttemptId} SourceEventId={SourceEventId} " +
                "ProfessorId={ProfessorId} IdentityUserId={IdentityUserId} " +
                "Channel={Channel} Provider={Provider} DispatchStatus={DispatchStatus} " +
                "ProviderMessageId={ProviderMessageId}",
                "ProfessorAccountNotificationProcessed",
                true,
                dispatchResult.Data.NotificationMessageId.Value,
                dispatchResult.Data.DeliveryAttemptId.Value,
                request.EventId,
                request.ProfessorId,
                request.IdentityUserId,
                dispatchResult.Data.Channel,
                dispatchResult.Data.Provider,
                dispatchResult.Data.Status,
                dispatchResult.Data.ProviderMessageId);

            return Result.Success();
        }

        private static string CreateSmsBody(Command request)
            => $"استاد گرامی {request.FirstName} {request.LastName}، حساب کاربری شما در سامانه UMS ایجاد شد. " +
               $"نام کاربری: {request.UserName}، رمز عبور موقت: {request.TemporaryPassword}. " +
               "پس از اولین ورود، تغییر رمز عبور الزامی است.";
    }
}