using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public interface IOperationResult
    {
        bool Success { get; }
        object StackState { get; }
        object StackInput { get; }
        IEnumerable<OperationEvent> Events { get; }
    }

    public interface ICommandResult : IOperationResult
    {
    }

    public interface IQueryResult<T> : IOperationResult
    {
        Emptyable<T> Result { get; }
    }

    public interface IOperationResult<TOperationEvent> : IOperationResult
        where TOperationEvent : OperationEvent
    {
        new IEnumerable<TOperationEvent> Events { get; }
        IReadOnlyList<BlockTraceResult<TOperationEvent>> StackTrace { get; }
    }

    public interface ICommandResult<TOperationEvent> : ICommandResult, IOperationResult<TOperationEvent>
        where TOperationEvent : OperationEvent
    {

    }

    public interface IQueryResult<TOperationEvent, T> : IQueryResult<T>, IOperationResult<TOperationEvent>
        where TOperationEvent : OperationEvent
    {

    }

    public interface IOperationResult<TInput, TState, TOperationEvent> : IOperationResult<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        new TState StackState { get; }
        new TInput StackInput { get; }
    }

    public interface ICommandResult<TInput, TState, TOperationEvent> : ICommandResult<TOperationEvent>, IOperationResult<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {

    }

    public interface IQueryResult<TInput, TState, TOperationEvent, T> : IQueryResult<TOperationEvent, T>, IOperationResult<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {

    }
}
