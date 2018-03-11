using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class CommandResult<TInput, TState, TOperationEvent> : OperationResult<TInput, TState, TOperationEvent>, ICommandResult<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        public CommandResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace, TInput stackInput, TState stackState)
            : base(success, stackTrace, stackInput, stackState)
        {


        }
    }
}
