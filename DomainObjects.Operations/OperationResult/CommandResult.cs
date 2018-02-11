using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class CommandResult<TState, TOperationEvent> : OperationResult<TState, TOperationEvent>, ICommandResult<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public CommandResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace, TState stackState)
            : base(success, stackTrace, stackState)
        {


        }
    }
}
