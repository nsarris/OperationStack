using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class StackEvents : IStackEvents
    {
        IEnumerable<IOperationEvent> events;
        public StackEvents(IEnumerable<IOperationEvent> events)
        {
            this.events = events;
        }

        public IEnumerable<TEvent> Filter<TEvent>(Func<TEvent, bool> filter = null) 
            where TEvent : IOperationEvent
        {
            var events = this.OfType<TEvent>();
            if (filter != null)
                events = events.Where(filter);
            return events;
        }

        public IEnumerable<TEvent> FilterUnhandled<TEvent>(Func<TEvent, bool> filter = null, bool markAsHandled = false)
            where TEvent : IOperationEvent
        {
            var events = this.OfType<TEvent>().Where(x => !x.Handled);
            if (filter != null)
                events = events.Where(filter);
            var r =  events.ToList();
            r.ForEach(x => x.Handled = true);
            return r;
        }

        public IEnumerator<IOperationEvent> GetEnumerator()
        {
            return events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
