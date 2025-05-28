using MediatR;

namespace SimplifiedDnd.Application.Abstractions.Core;

internal interface IQueryHandler<in TRequest, TResponse> :
  IRequestHandler<TRequest, Result<TResponse>>
  where TRequest : IQuery<TResponse>
  where TResponse : notnull;