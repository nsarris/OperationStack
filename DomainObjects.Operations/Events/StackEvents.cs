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
        private readonly List<OperationEvent> events = new List<OperationEvent>();
        public StackEvents()
        {

        }

        public StackEvents(IEnumerable<OperationEvent> events)
        {
            this.events = events.ToList();
        }

        internal void AddRange(IEnumerable<OperationEvent> events)
        {
            this.events.AddRange(events);
        }

        public IEnumerable<TEvent> Filter<TEvent>(Func<TEvent, bool> filter = null) 
            where TEvent : OperationEvent
        {
            var filteredEvents = this.OfType<TEvent>();
            if (filter != null)
                filteredEvents = filteredEvents.Where(filter);
            return filteredEvents;
        }

        public IEnumerable<TEvent> FilterErrors<TEvent>(bool? handled, Func<TEvent, bool> filter = null)
            where TEvent : OperationEvent
        {
            var filteredEvents = this.OfType<TEvent>().Where(x => !x.IsSwallowed && (handled == null || x.IsHandled == handled));
            if (filter != null)
                filteredEvents = filteredEvents.Where(filter);
            var r =  filteredEvents.ToList();
            
            return r;
        }

        public IEnumerable<IOperationExceptionError<TEvent, TException>> FilterExceptions<TEvent,TException>(bool? handled, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null) 
            where TException : Exception 
            where TEvent : OperationEvent
        {
            var filteredEvents = this.OfType<TEvent>()
                .Where(x => x.Exception is TException && !x.IsSwallowed && (handled == null || x.IsHandled == handled))
                .Select(x => new OperationExceptionError<TEvent,TException>(x, x.Exception as TException) as IOperationExceptionError<TEvent,TException>);
            if (filter != null)
                filteredEvents = filteredEvents.Where(filter);
            var r = filteredEvents.ToList();
            
            return r;
        }

        public IEnumerator<OperationEvent> GetEnumerator()
        {
            return events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
