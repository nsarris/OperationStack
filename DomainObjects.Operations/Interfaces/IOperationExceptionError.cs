using System;

namespace DomainObjects.Operations
{
    public interface IOperationExceptionError<out TEvent,out TException>
        where TException : Exception
        where TEvent : OperationEvent
    {
        TEvent Error { get; }
        TException Exception { get; }
    }
}
