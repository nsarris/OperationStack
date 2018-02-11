using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class QueryResult<TState, TOperationEvent,T> : CommandResult<TState, TOperationEvent>, IQueryResult<TState, TOperationEvent,T>
        where TOperationEvent : IOperationEvent
    {
        public Emptyable<T> Result { get; private set; }

        public QueryResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace, TState stackState, Emptyable<T> result)
            : base(success, stackTrace, stackState)
        {
            Result = result;
        }
    }
}
