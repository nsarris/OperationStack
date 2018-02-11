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
        public BlockTraceResult(string tag, IEmptyable input, IEmptyable result, IEnumerable<TOperationEvent> events, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace = null, ExecutionTime time = null)
        {
            Tag = tag;
            Input = input;
            Result = result;
            InnerStackTrace = stackTrace ?? Enumerable.Empty<BlockTraceResult<TOperationEvent>>();
            Events = events;
            
            if (time == null)
                Time = new ExecutionTime();
            else
                Time = time;
        }
    }
}
