using System;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public interface IOperationEvents<TOperationEvent> : IEnumerable<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        void Add(TOperationEvent @event);
        void Add(Exception exception);
        void Throw(TOperationEvent @event);
        void Throw(Exception exception);
        void Append(IEnumerable<TOperationEvent> events);
        bool HasUnhandledErrors { get; }
    }
}
