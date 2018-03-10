using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Operations
{
    public class BlockTraceResult<TOperationEvent>
        where TOperationEvent : OperationEvent
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
            Result = blockResult.Result;
            InnerStackTrace = stackBlock.InnerStackTrace ?? Enumerable.Empty<BlockTraceResult<TOperationEvent>>();
            Events = stackBlock.Events.ToList();

            if (blockResult.ExecutionTime == null)
                Time = new ExecutionTime();
            else
                Time = blockResult.ExecutionTime;
        }

        internal BlockTraceResult(string tag, TOperationEvent error, IEmptyable input)
        {
            Tag = tag;
            Events = new[] { error };
            Input = input;
            InnerStackTrace = Enumerable.Empty<BlockTraceResult<TOperationEvent>>();
            Time = new ExecutionTime();
        }
    }
}
