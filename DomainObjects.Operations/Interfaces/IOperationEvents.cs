using System;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public interface IOperationEvents : IEnumerable<OperationEvent>
        
    {
        void Add(OperationEvent @event);
        void Add(Exception exception);
        void Throw(OperationEvent @event);
        void Throw(Exception exception);
        void Append(IEnumerable<OperationEvent> events);
        bool HasUnhandledErrors { get; }
    }
}
