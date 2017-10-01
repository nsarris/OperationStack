using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class OperationEvents : List<IOperationEvent>, IOperationEvents
    {
        public OperationEvents()
        {

        }
        public OperationEvents(IEnumerable<IOperationEvent> events)
        {
            this.Append(events);
        }
        public void Append(IEnumerable<IOperationEvent> events)
        {
            this.AddRange(events);
        }

        public bool HasUnhandledException
        {
            get
            {
                return this.OfType<OperationError>().Any(x => (x.UnhandledException));
            }
        }
    }

    public class OperationEvent : IOperationEvent
    {
        public bool Handled { get; set; }
    }

    public class OperationError : OperationEvent, IOperationError
    {
        public OperationError(Exception exception)
        {
            Exception = exception;
        }
        public Exception Exception { get; private set; }
        public bool UnhandledException { get; set; }

        public static OperationError FromException(Exception e, bool Unhandled = false)
        {
            var type = typeof(OperationExceptionEvent<>).MakeGenericType(new[] { e.GetType() });
            var error = (OperationError)(Activator.CreateInstance(type, new object[] { e }));
            error.UnhandledException = Unhandled;
            return error;
        }
    }

    public class OperationExceptionEvent<TException> : OperationError, IOperationExceptionError, IOperationExceptionError<TException>
        where TException : Exception
    {
        public OperationExceptionEvent(TException exception)
            :base(exception)
        {
        }

        internal OperationExceptionEvent(TException exception, bool Unhandled)
            : base(exception)
        {
            UnhandledException = Unhandled;
        }

        public new TException Exception => (TException)(base.Exception);
    }
}
