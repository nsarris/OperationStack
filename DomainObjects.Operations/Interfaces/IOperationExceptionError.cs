using System;

namespace DomainObjects.Operations
{
    public interface IOperationExceptionError<TEvent, TException>
        where TException : Exception
        where TEvent : OperationEvent
    {
        TEvent Error { get; }
        TException Exception { get; }
    }




}
