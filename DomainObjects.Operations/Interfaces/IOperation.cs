using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public interface IOperation
    {
        bool SupportsSync { get; }
        bool SupportsAsync { get; }
        bool PreferAsync { get; }
    }


    public interface IQueryOperation<TResult> : IOperation
    {
        IQueryResult<TResult> Execute();
        Task<IQueryResult<TResult>> ExecuteAsync();
    }

    public interface ICommandOperation : IOperation
    {
        ICommandResult Execute();
        Task<ICommandResult> ExecuteAsync();
    }

    public interface IQueryOperationWithInput<TInput, TResult> : IQueryOperation<TResult>
    {
        IQueryResult<TResult> Execute(TInput input);
        Task<IQueryResult<TResult>> ExecuteAsync(TInput input);
    }

    public interface ICommandOperationWithInput<TInput> : ICommandOperation
    {
        ICommandResult Execute(TInput input);
        Task<ICommandResult> ExecuteAsync(TInput input);
    }

    public interface IQueryOperationWithState<TState, TResult> : IQueryOperation<TResult>
    {
        IQueryResult<TResult> Execute(TState initialState);
        Task<IQueryResult<TResult>> ExecuteAsync(TState initialState);
    }

    public interface ICommandOperationWithState<TState> : ICommandOperation
    {
        ICommandResult Execute(TState initialState);
        Task<ICommandResult> ExecuteAsync(TState initialState);
    }

    public interface IQueryOperation<TInput, TState, TResult> : IQueryOperationWithInput<TInput, TResult>, IQueryOperationWithState<TState, TResult>
    {
        IQueryResult<TInput, TState, TResult> Execute(TInput input, TState initialState);
        Task<IQueryResult<TInput, TState, TResult>> ExecuteAsync(TInput input, TState initialState);
    }

    public interface ICommandOperation<TInput, TState> : ICommandOperationWithInput<TInput>, ICommandOperationWithState<TState>
    {
        ICommandResult<TInput, TState> Execute(TInput input, TState initialState);
        Task<ICommandResult<TInput, TState>> ExecuteAsync(TInput input, TState initialState);
    }
}
