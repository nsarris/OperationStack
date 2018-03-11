using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class QueryResult<TInput, TState, TOperationEvent,T> : CommandResult<TInput, TState, TOperationEvent>, IQueryResult<TInput, TState, TOperationEvent,T>
        where TOperationEvent : OperationEvent
    {
        public Emptyable<T> Result { get; private set; }

        public QueryResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace, TInput stackInput, TState stackState, Emptyable<T> result)
            : base(success, stackTrace, stackInput, stackState)
        {
            Result = result;
        }
    }
}
