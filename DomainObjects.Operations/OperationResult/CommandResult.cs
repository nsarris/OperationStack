using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class CommandResult<TInput, TState> : OperationResult<TInput, TState>, ICommandResult<TInput, TState>
        
    {
        public CommandResult(bool success, IEnumerable<BlockTraceResult> stackTrace, TInput stackInput, TState stackState)
            : base(success, stackTrace, stackInput, stackState)
        {


        }
    }
}
