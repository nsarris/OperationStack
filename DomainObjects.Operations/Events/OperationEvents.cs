using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class OperationEvents : IOperationEvents
    {
        readonly List<OperationEvent> events = new List<OperationEvent>();

        public OperationEvents()
        {

        }
        public OperationEvents(IEnumerable<OperationEvent> events)
        {
            this.events.AddRange(events);
        }

        public void Append(IEnumerable<OperationEvent> events)
        {
            this.events.AddRange(events);
        }

        public void Append(params OperationEvent[] events)
        {
            this.events.AddRange(events);
        }

        public void Add(OperationEvent @event)
        {
            @event.Handle();
            this.events.Add(@event);
        }

        public void Throw(OperationEvent @event)
        {
            @event.Throw();
            this.events.Add(@event);
        }

        public IEnumerator<OperationEvent> GetEnumerator()
        {
            return this.events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.events.GetEnumerator();
        }

        public void Add(Exception exception)
        {
            Add(ExceptionErrorBuilder.Build(exception));
        }

        public void Throw(Exception exception)
        {
            Throw(ExceptionErrorBuilder.Build(exception));
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
