using MediatR;

namespace SharedKernel.Abstractions.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull;