using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public BlockTraceResult(string tag, IEmptyable input, IEmptyable result, IEnumerable<TOperationEvent> events, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace = null, ExecutionTime time = null)
        {
            Tag = tag;
            Input = input;
            Result = result;
            var e = events.ToList();
            if (InnerStackTrace != null)
                e.AddRange(InnerStackTrace.SelectMany(x => x.Events));
            Events = e;
            InnerStackTrace = stackTrace ?? Enumerable.Empty<BlockTraceResult<TOperationEvent>>();
            if (time == null)
                Time = new ExecutionTime();
            else
                Time = time;
        }
    }

    public abstract class OperationResult<TOperationEvent> : IOperationResult<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public bool Success { get; private set; }
        public IEnumerable<TOperationEvent> Events { get; private set; }
        public IReadOnlyList<BlockTraceResult<TOperationEvent>> StackTrace { get; private set; }

        public OperationResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace)
        {
            Success = success;

            StackTrace = new System.Collections.ObjectModel.ReadOnlyCollection<BlockTraceResult<TOperationEvent>>(
                stackTrace
                .ToList());

            Events = StackTrace.SelectMany(x => x.Events).ToList();
        }
    }

    public class CommandResult<TOperationEvent> : OperationResult<TOperationEvent>, ICommandResult<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public CommandResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace)
            : base(success, stackTrace)
        {


        }
    }

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
