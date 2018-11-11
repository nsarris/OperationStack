using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public interface IOperationResult
    {
        bool Success { get; }
        object StackState { get; }
        object StackInput { get; }
        IEnumerable<OperationEvent> Events { get; }
        IReadOnlyList<BlockTraceResult> StackTrace { get; }
    }

    public interface ICommandResult : IOperationResult
    {
    }

    public interface IQueryResult<T> : IOperationResult
    {
        Emptyable<T> Result { get; }
    }

   
    public interface IOperationResult<TInput, TState> : IOperationResult
    {
        new TState StackState { get; }
        new TInput StackInput { get; }
    }

    public interface ICommandResult<TInput, TState> : ICommandResult, IOperationResult<TInput, TState>
    {

    }

    public interface IQueryResult<TInput, TState, T> : IQueryResult<T>, IOperationResult<TInput, TState>
    {

    }
}
