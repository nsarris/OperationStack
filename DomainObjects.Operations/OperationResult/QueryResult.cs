using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class QueryResult<TOperationEvent,T> : CommandResult<TOperationEvent>, IQueryResult<TOperationEvent,T>
        where TOperationEvent : IOperationEvent
    {
        public Emptyable<T> Result { get; private set; }

        public QueryResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace, Emptyable<T> result)
            : base(success, stackTrace)
        {
            Result = result;
        }
    }
}
