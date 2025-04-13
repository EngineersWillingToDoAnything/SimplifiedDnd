using MediatR;
using SimplifiedDnd.Application.Characters.CreateCharacter;

namespace SimplifiedDnd.Application.Abstractions.Core;

internal interface ICommandHandler<in TRequest, TResponse> :
  IRequestHandler<TRequest, Result<TResponse>>
  where TRequest : ICommand<TResponse>
  where TResponse : notnull;