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

        public interface IQueryOperationWithInput<TInput, TOperationEvent, TResult> : IQueryOperation<TOperationEvent, TResult>
            where TOperationEvent : OperationEvent
        {
            IQueryResult<TOperationEvent, TResult> Execute(TInput input);
            Task<IQueryResult<TOperationEvent, TResult>> ExecuteAsync(TInput input);
        }

        public interface ICommandOperationWithInput<TInput, TOperationEvent> : ICommandOperation<TOperationEvent>
            where TOperationEvent : OperationEvent
        {
            ICommandResult<TOperationEvent> Execute(TInput input);
            Task<ICommandResult<TOperationEvent>> ExecuteAsync(TInput input);
        }

        public interface IQueryOperationWithState<TState, TOperationEvent, TResult> : IQueryOperation<TOperationEvent, TResult>
            where TOperationEvent : OperationEvent
        {
            IQueryResult<TOperationEvent, TResult> Execute(TState initialState);
            Task<IQueryResult<TOperationEvent, TResult>> ExecuteAsync(TState initialState);
        }

        public interface ICommandOperationWithState<TState, TOperationEvent> : ICommandOperation<TOperationEvent>
            where TOperationEvent : OperationEvent
        {
            ICommandResult<TOperationEvent> Execute(TState initialState);
            Task<ICommandResult<TOperationEvent>> ExecuteAsync(TState initialState);
        }

        public interface IQueryOperation<TInput, TState, TOperationEvent, TResult> : IQueryOperationWithInput<TInput, TOperationEvent, TResult>, IQueryOperationWithState<TState, TOperationEvent, TResult>
            where TOperationEvent : OperationEvent
        {
            IQueryResult<TInput, TState, TOperationEvent, TResult> Execute(TInput input, TState initialState);
            Task<IQueryResult<TInput, TState, TOperationEvent, TResult>> ExecuteAsync(TInput input, TState initialState);

            OperationStack<TInput, TState, TOperationEvent, IVoid> CreateNew(StackBlockSpecBase<TInput, TState, TOperationEvent> block);
            OperationStack<TInput, TState, TOperationEvent, TOutput> CreateNew<TOutput>(StackBlockSpecBase<TInput, TState, TOperationEvent> block);

            int NextIndex { get; }
        }

        public interface ICommandOperation<TInput, TState, TOperationEvent> : ICommandOperationWithInput<TInput, TOperationEvent>, ICommandOperationWithState<TState, TOperationEvent>
            where TOperationEvent : OperationEvent
        {
            ICommandResult<TInput, TState, TOperationEvent> Execute(TInput input, TState initialState);
            Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync(TInput input, TState initialState);
        }

    



}
