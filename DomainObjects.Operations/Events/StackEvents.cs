using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class StackEvents<TOperationEvent> : IStackEvents<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        private readonly List<TOperationEvent> events = new List<TOperationEvent>();
        public StackEvents()
        {

        }

        public StackEvents(IEnumerable<TOperationEvent> events)
        {
            this.events = events.ToList();
        }

        internal void AddRange(IEnumerable<TOperationEvent> events)
        {
            this.events.AddRange(events);
        }

        public IEnumerable<TEvent> Filter<TEvent>(Func<TEvent, bool> filter = null) 
            where TEvent : TOperationEvent
        {
            var filteredEvents = this.OfType<TEvent>();
            if (filter != null)
                filteredEvents = filteredEvents.Where(filter);
            return filteredEvents;
        }

        public IEnumerable<TEvent> FilterErrors<TEvent>(bool? handled, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            var filteredEvents = this.OfType<TEvent>().Where(x => !x.IsSwallowed && (handled == null || x.IsHandled == handled));
            if (filter != null)
                filteredEvents = filteredEvents.Where(filter);
            var r =  filteredEvents.ToList();
            
            return r;
        }

        public IEnumerable<IOperationExceptionError<TEvent, TException>> FilterExceptions<TEvent,TException>(bool? handled, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null) 
            where TException : Exception 
            where TEvent : TOperationEvent
        {
            var filteredEvents = this.OfType<TEvent>()
                .Where(x => x.Exception is TException && !x.IsSwallowed && (handled == null || x.IsHandled == handled))
                .Select(x => new OperationExceptionError<TEvent,TException>(x, x.Exception as TException) as IOperationExceptionError<TEvent,TException>);
            if (filter != null)
                filteredEvents = filteredEvents.Where(filter);
            var r = filteredEvents.ToList();
            
            return r;
        }

        public IEnumerator<TOperationEvent> GetEnumerator()
        {
            return events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
