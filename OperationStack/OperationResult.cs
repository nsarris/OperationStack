using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{



    //public interface IOperationResult
    //{
    //    IEnumerable<OperationEvent> EventLog { get; }
    //    object Input { get; }
    //    object Result { get; }
    //}

    //public interface IOperationResult<T> : IOperationResult
    //{
    //    new T Result { get; }
    //}

    [System.Diagnostics.DebuggerDisplay("{ElapsedMilliseconds}ms")]
    public class OperationStackResultTimer
    {
        public bool Timed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Elapsed { get; set; }
        public long ElapsedMilliseconds => Convert.ToInt64(Elapsed.TotalMilliseconds);
        internal void Set(DateTime start, DateTime end)
        {
            StartTime = start;
            EndTime = end;
            Elapsed = start - end;
            Timed = true;
        }

        internal void Set(DateTime start, DateTime end, TimeSpan elapsed)
        {
            StartTime = start;
            EndTime = end;
            Elapsed = elapsed;
            Timed = true;

        }
    }

    public class OperationStackResult<T>
    {
        public IEnumerable<OperationResult> StackTrace { get; private set; }
        public IEnumerable<OperationEvent> EventLog => StackTrace.SelectMany(x => x.EventLog);
        public Emptyable<T> Result { get; private set; }

        public OperationStackResultTimer Timer { get; private set; } = new OperationStackResultTimer();
        public OperationStackResult(IEnumerable<OperationResult> stackTrace, Emptyable<T> result)
        {
            StackTrace = stackTrace;
            Result = result;
        }
    }

    public class OperationStackResult : OperationStackResult<object>
    {
        public OperationStackResult(IEnumerable<OperationResult> stackTrace, Emptyable<object> result)
            : base(stackTrace, result)
        {

        }
    }


    public class OperationResult
    {
        public IOperationEventLog EventLog { get; private set; }

        internal OperationResult(Operation operation, IEmptyable input)
        {
            Input = input;
            EventLog = operation.EventLog;
            Result = Emptyable.Empty;
            Tag = operation.Name;
        }

        internal OperationResult(Operation operation, IEmptyable input, IEmptyable result)
            : this(operation, input)
        {
            Result = result;
        }

        public string Tag { get; set; }
        public IEmptyable Result { get; private set; }

        public IEmptyable Input { get; private set; }

        public OperationStackResultTimer Timer { get; private set; } = new OperationStackResultTimer();
    }


}
