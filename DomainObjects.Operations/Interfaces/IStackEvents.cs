using System;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public interface IStackEvents<TOperationEvent> : IEnumerable<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        IEnumerable<TEvent> Filter<TEvent>(Func<TEvent, bool> filter = null) where TEvent : TOperationEvent;
        IEnumerable<TEvent> FilterErrors<TEvent>(bool? handled, Func<TEvent, bool> filter = null) where TEvent : TOperationEvent;
        IEnumerable<IOperationExceptionError<TEvent, TException>> FilterExceptions<TEvent,TException>(bool? handled, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null) where TException : Exception where TEvent : TOperationEvent;
    }
}
