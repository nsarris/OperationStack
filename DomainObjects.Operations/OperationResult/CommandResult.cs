using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class CommandResult<TOperationEvent> : OperationResult<TOperationEvent>, ICommandResult<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public CommandResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace)
            : base(success, stackTrace)
        {


        }
    }
}
