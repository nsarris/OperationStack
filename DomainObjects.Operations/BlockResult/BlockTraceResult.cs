using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Operations
{
    public class BlockTraceResult<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public string Tag { get; private set; }
        public IEmptyable Result { get; private set; }
        public IEmptyable Input { get; private set; }
        public IEnumerable<TOperationEvent> Events { get; private set; }
        public ExecutionTime Time { get; private set; }

        public IEnumerable<BlockTraceResult<TOperationEvent>> InnerStackTrace { get; private set; }
        public IEnumerable<TOperationEvent> FlattenedEvents => Events.Concat(InnerStackTrace.SelectMany(x => x.FlattenedEvents));
        
        internal BlockTraceResult(StackBlockBase<TOperationEvent> stackBlock, IBlockResult blockResult)
        {
            Tag = stackBlock.Tag;
            Input = stackBlock.Input;
            Result = blockResult.GetEffectiveResult();
            InnerStackTrace = stackBlock.InnerStackTrace ?? Enumerable.Empty<BlockTraceResult<TOperationEvent>>();
            Events = stackBlock.FlattenedEvents;

            if (blockResult.ExecutionTime == null)
                Time = new ExecutionTime();
            else
                Time = blockResult.ExecutionTime;
        }
    }
}
