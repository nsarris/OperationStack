using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class QueryResult<TInput, TState,T> : CommandResult<TInput, TState>, IQueryResult<TInput, TState,T>
    {
        public Emptyable<T> Result { get; private set; }

        public QueryResult(bool success, IEnumerable<BlockTraceResult> stackTrace, TInput stackInput, TState stackState, Emptyable<T> result)
            : base(success, stackTrace, stackInput, stackState)
        {
            Result = result;
        }
    }
}
