using MediatR;
using SharedKernel.Abstractions.CQRS;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;
using SharedKernel.Messaging.Abstractions;

namespace Notification.Api.Features.Notifications.SendEmployeeAccountCreatedNotification;

public sealed class SendEmployeeAccountCreatedNotification
{
    public sealed class IntegrationEventHandler(ISender sender) : IIntegrationEventHandler<EmployeeAccountCreatedIntegrationEvent>
    {
        public async Task HandleAsync(EmployeeAccountCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            var command = new Command(
                integrationEvent.EventId,
                integrationEvent.EmployeeId,
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
        Guid EmployeeId,
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
        private const string SourceService = "CentralOrganization.Api";
        private const string SourceEventType = nameof(EmployeeAccountCreatedIntegrationEvent);

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var alreadyProcessed = await notificationRepository.ExistsAsync(request.EventId, NotificationChannel.Sms, cancellationToken);

            if (alreadyProcessed)
            {
                logger.LogInformation(
                    "Duplicate notification event ignored. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "SourceEventId={SourceEventId} SourceEventType={SourceEventType} " +
                    "Channel={Channel} EmployeeId={EmployeeId}",
                    "DuplicateNotificationEventIgnored",
                    true,
                    request.EventId,
                    SourceEventType,
                    NotificationChannel.Sms,
                    request.EmployeeId);

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
                    "Employee account notification creation failed. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "SourceEventId={SourceEventId} EmployeeId={EmployeeId} " +
                    "ErrorCode={ErrorCode} ErrorDescription={ErrorDescription}",
                    "EmployeeAccountNotificationCreationFailed",
                    true,
                    request.EventId,
                    request.EmployeeId,
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
                    "Employee account notification dispatch could not be processed. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "NotificationMessageId={NotificationMessageId} SourceEventId={SourceEventId} " +
                    "EmployeeId={EmployeeId} ErrorCode={ErrorCode} " +
                    "ErrorDescription={ErrorDescription}",
                    "EmployeeAccountNotificationDispatchError",
                    true,
                    notification.Id.Value,
                    request.EventId,
                    request.EmployeeId,
                    dispatchResult.Error.Code,
                    dispatchResult.Error.Description);

                return dispatchResult.Error;
            }

            await unitOfWork.SaveAsync(cancellationToken);

            if (dispatchResult.Data.Status == NotificationDispatchStatus.Failed)
            {
                logger.LogWarning(
                    "Employee account notification saved as failed. " +
                    "EventName={EventName} NotificationLog={NotificationLog} " +
                    "NotificationMessageId={NotificationMessageId} " +
                    "DeliveryAttemptId={DeliveryAttemptId} SourceEventId={SourceEventId} " +
                    "EmployeeId={EmployeeId} IdentityUserId={IdentityUserId} " +
                    "Channel={Channel} Provider={Provider} DispatchStatus={DispatchStatus} " +
                    "ErrorCode={ErrorCode} ErrorMessage={ErrorMessage}",
                    "EmployeeAccountNotificationFailed",
                    true,
                    dispatchResult.Data.NotificationMessageId.Value,
                    dispatchResult.Data.DeliveryAttemptId.Value,
                    request.EventId,
                    request.EmployeeId,
                    request.IdentityUserId,
                    dispatchResult.Data.Channel,
                    dispatchResult.Data.Provider,
                    dispatchResult.Data.Status,
                    dispatchResult.Data.ErrorCode,
                    dispatchResult.Data.ErrorMessage);

                return Result.Success();
            }

            logger.LogInformation(
                "Employee account notification processed successfully. " +
                "EventName={EventName} NotificationLog={NotificationLog} " +
                "NotificationMessageId={NotificationMessageId} " +
                "DeliveryAttemptId={DeliveryAttemptId} SourceEventId={SourceEventId} " +
                "EmployeeId={EmployeeId} IdentityUserId={IdentityUserId} " +
                "Channel={Channel} Provider={Provider} DispatchStatus={DispatchStatus} " +
                "ProviderMessageId={ProviderMessageId}",
                "EmployeeAccountNotificationProcessed",
                true,
                dispatchResult.Data.NotificationMessageId.Value,
                dispatchResult.Data.DeliveryAttemptId.Value,
                request.EventId,
                request.EmployeeId,
                request.IdentityUserId,
                dispatchResult.Data.Channel,
                dispatchResult.Data.Provider,
                dispatchResult.Data.Status,
                dispatchResult.Data.ProviderMessageId);

            return Result.Success();
        }

        private static string CreateSmsBody(Command request)
        {
            return
                $"{request.FirstName} {request.LastName} عزیز، حساب کاربری شما در سامانه UMS ایجاد شد. " +
                $"نام کاربری: {request.UserName}، رمز عبور موقت: {request.TemporaryPassword}. " +
                "پس از اولین ورود، تغییر رمز عبور الزامی است.";
        }
    }
}