using MediatR;

namespace SimplifiedDnd.Application.Abstractions.Core;

internal interface ICommand<T> : IRequest<Result<T>> where T : notnull;