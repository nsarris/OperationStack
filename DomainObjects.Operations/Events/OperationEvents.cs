using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class OperationEvents<TOperationEvent> : IOperationEvents<TOperationEvent>
        where TOperationEvent :OperationEvent
    {
        readonly List<TOperationEvent> events = new List<TOperationEvent>();

        public OperationEvents()
        {

        }
        public OperationEvents(IEnumerable<TOperationEvent> events)
        {
            this.events.AddRange(events);
        }

        public void Append(IEnumerable<TOperationEvent> events)
        {
            this.events.AddRange(events);
        }

        public void Append(params TOperationEvent[] events)
        {
            this.events.AddRange(events);
        }

        public void Add(TOperationEvent @event)
        {
            @event.Handle();
            this.events.Add(@event);
        }

        public void Throw(TOperationEvent @event)
        {
            @event.Throw();
            this.events.Add(@event);
        }

        public IEnumerator<TOperationEvent> GetEnumerator()
        {
            return this.events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.events.GetEnumerator();
        }

        public void Add(Exception exception)
        {
            Add(ExceptionErrorBuilder<TOperationEvent>.Build(exception));
        }

        public void Throw(Exception exception)
        {
            Throw(ExceptionErrorBuilder<TOperationEvent>.Build(exception));
        }

        public bool HasUnhandledErrors
        {
            get
            {
                return this.Any(x => x.IsException && !x.IsHandled && !x.IsSwallowed);
            }
        }
    }
}
