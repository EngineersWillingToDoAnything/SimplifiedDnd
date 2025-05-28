using MediatR;

namespace SimplifiedDnd.Application.Abstractions.Core;

internal interface IQuery<T> : IRequest<Result<T>> where T : notnull;