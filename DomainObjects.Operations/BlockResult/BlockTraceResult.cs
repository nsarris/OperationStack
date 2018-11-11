using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Operations
{
    public class BlockTraceResult
    {
        public string Tag { get; private set; }
        public IEmptyable Result { get; private set; }
        public IEmptyable Input { get; private set; }
        public IEnumerable<OperationEvent> Events { get; private set; }
        public ExecutionTime Time { get; private set; }

        public IEnumerable<BlockTraceResult> InnerStackTrace { get; private set; }
        public IEnumerable<OperationEvent> FlattenedEvents => Events.Concat(InnerStackTrace.SelectMany(x => x.FlattenedEvents));
        
        internal BlockTraceResult(IStackBlock stackBlock, IBlockResult blockResult)
        {
            Tag = stackBlock.Tag;
            Input = stackBlock.Input;
            Result = blockResult.Result;
            InnerStackTrace = stackBlock.InnerStackTrace ?? Enumerable.Empty<BlockTraceResult>();
            Events = stackBlock.StackEvents.ToList();

            if (blockResult.ExecutionTime == null)
                Time = new ExecutionTime();
            else
                Time = blockResult.ExecutionTime;
        }

        internal BlockTraceResult(string tag, OperationEvent error, IEmptyable input)
        {
            Tag = tag;
            Events = new[] { error };
            Input = input;
            InnerStackTrace = Enumerable.Empty<BlockTraceResult>();
            Time = new ExecutionTime();
        }
    }
}
