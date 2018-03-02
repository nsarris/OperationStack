using System;

namespace DomainObjects.Operations
{
    public class OperationExceptionError<TEvent, TException> : IOperationExceptionError<TEvent,TException>
        where TEvent : OperationEvent
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
