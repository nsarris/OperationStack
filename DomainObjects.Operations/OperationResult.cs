using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class BlockTraceResult
    {
        public string Tag { get; private set; }
        public IEmptyable Result { get; private set; }
        public IEmptyable Input { get; private set; }
        public IEnumerable<IOperationEvent> Events { get; private set; }
        public ExecutionTime Time { get; private set; }

        public IEnumerable<BlockTraceResult> InnerStackTrace { get; private set; }

        public BlockTraceResult(string tag, IEmptyable input, IEmptyable result, IEnumerable<IOperationEvent> events, IEnumerable<BlockTraceResult> stackTrace = null, ExecutionTime time = null)
        {
            Tag = tag;
            Input = input;
            Result = result;
            var e = events.ToList();
            if (InnerStackTrace != null)
                e.AddRange(InnerStackTrace.SelectMany(x => x.Events));
            Events = e;
            InnerStackTrace = stackTrace ?? Enumerable.Empty<BlockTraceResult>();
            if (time == null)
                Time = new ExecutionTime();
            else
                Time = time;
        }
    }

    public abstract class OperationResult : IOperationResult
    {
        public bool Success { get; private set; }
        public IEnumerable<IOperationEvent> Events { get; private set; }
        public IReadOnlyList<BlockTraceResult> StackTrace { get; private set; }

        public OperationResult(bool success, IEnumerable<BlockTraceResult> stackTrace)
        {
            Success = success;

            StackTrace = new System.Collections.ObjectModel.ReadOnlyCollection<BlockTraceResult>(
                stackTrace
                .ToList());

            Events = StackTrace.SelectMany(x => x.Events).ToList();
        }
    }

    public class CommandResult : OperationResult,ICommandResult
    {
        public CommandResult(bool success, IEnumerable<BlockTraceResult> stackTrace)
            : base(success,stackTrace)
        {

            
        }
    }

    public class QueryResult<T> : CommandResult, IQueryResult<T>
    {
        public Emptyable<T> Result { get; private set; }

        public QueryResult(bool success, IEnumerable<BlockTraceResult> stackTrace, Emptyable<T> result)
            :base(success, stackTrace)
        {
            Result = result;
        }
    }
}
