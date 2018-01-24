using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class OperationEvents<TOperationEvent> : List<TOperationEvent>, IOperationEvents<TOperationEvent>
        where TOperationEvent :IOperationEvent
    {
        public OperationEvents()
        {

        }
        public OperationEvents(IEnumerable<TOperationEvent> events)
        {
            this.Append(events);
        }
        public void Append(IEnumerable<TOperationEvent> events)
        {
            this.AddRange(events);
        }

        public bool HasUnhandledException
        {
            get
            {
                return this.Any(x => x.IsException && x.UnhandledException);
            }
        }
    }

    public class OperationEvent : IOperationEvent
    {
        public bool Handled { get; set; }

        public bool IsError { get; protected set; }

        public bool IsException => Exception != null;

        public Exception Exception { get; private set; }

        public bool UnhandledException { get; private set; }

        public OperationEvent(Exception exception, bool unhandled = false)
        {
            Exception = exception;
            IsError = true;
            UnhandledException = unhandled;
        }
    }

    //public class OperationError : OperationEvent, IOperationError
    //{
    //    public OperationError(Exception exception, bool unhandled = false)
    //    {
    //        Exception = exception;
    //        UnhandledException = unhandled;
    //    }
    //    public Exception Exception { get; private set; }
    //    public bool UnhandledException { get; set; }

    //    //public static OperationError FromException(Exception e, bool unhandled = false)
    //    //{
    //    //    var type = typeof(OperationExceptionError<>).MakeGenericType(new[] { e.GetType() });
    //    //    var error = (OperationError)(Activator.CreateInstance(type, new object[] { e }));
    //    //    error.UnhandledException = unhandled;
    //    //    return error;
    //    //}
    //}

    public class OperationExceptionError<TEvent, TException> : IOperationExceptionError<TEvent,TException>
        where TEvent : IOperationEvent
        where TException : Exception
    {
        
        public OperationExceptionError(TEvent error, TException exception)
        {
            Error = error;
        }

        public TEvent Error { get; private set; }
        
        public TException Exception => Error?.Exception as TException;
    }
}
