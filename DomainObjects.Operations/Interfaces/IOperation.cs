using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public interface IOperation
    {
        bool SupportsSync { get; }
        bool SupportsAsync { get; }
        bool PreferAsync { get; }
    }


    public interface IQueryOperation<TOperationEvent, TResult> : IOperation
        where TOperationEvent : OperationEvent
    {
        IQueryResult<TOperationEvent, TResult> Execute();
        Task<IQueryResult<TOperationEvent, TResult>> ExecuteAsync();
    }

    public interface ICommandOperation<TOperationEvent> : IOperation
        where TOperationEvent : OperationEvent
    {
        ICommandResult<TOperationEvent> Execute();
        Task<ICommandResult<TOperationEvent>> ExecuteAsync();
    }

    public interface IQueryOperation<TInput, TState, TOperationEvent, TResult> : IQueryOperation<TOperationEvent, TResult>
        where TOperationEvent : OperationEvent
    {
        IQueryResult<TInput, TState, TOperationEvent, TResult> Execute(TState initialState);
        Task<IQueryResult<TInput, TState, TOperationEvent, TResult>> ExecuteAsync(TState initialState);
    }

    public interface ICommandOperation<TInput, TState, TOperationEvent> : ICommandOperation<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        ICommandResult<TInput, TState, TOperationEvent> Execute(TState initialState);
        Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync(TState initialState);
    }





}
