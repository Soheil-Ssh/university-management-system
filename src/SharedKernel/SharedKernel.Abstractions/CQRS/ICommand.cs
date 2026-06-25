using MediatR;

namespace SharedKernel.Abstractions.CQRS;

public interface ICommand : IRequest;

public interface ICommand<out TResult> : IRequest<TResult>;