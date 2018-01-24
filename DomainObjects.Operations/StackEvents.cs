using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class StackEvents<TOperationEvent> : IStackEvents<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        IEnumerable<TOperationEvent> events;
        public StackEvents(IEnumerable<TOperationEvent> events)
        {
            this.events = events;
        }

        public IEnumerable<TEvent> Filter<TEvent>(Func<TEvent, bool> filter = null) 
            where TEvent : TOperationEvent
        {
            var events = this.OfType<TEvent>();
            if (filter != null)
                events = events.Where(filter);
            return events;
        }

        public IEnumerable<TEvent> FilterUnhandled<TEvent>(Func<TEvent, bool> filter = null, bool markAsHandled = false)
            where TEvent : TOperationEvent
        {
            var events = this.OfType<TEvent>().Where(x => !x.Handled);
            if (filter != null)
                events = events.Where(filter);
            var r =  events.ToList();
            r.ForEach(x => x.Handled = true);
            return r;
        }

        public IEnumerable<IOperationExceptionError<TEvent, TException>> FilterUnhandledException<TEvent,TException>(Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool markAsHandled = false) where TException : Exception where TEvent : TOperationEvent
        {
            var events = this.OfType<TEvent>()
                .Where(x => x.Exception is TException && !x.Handled)
                .Select(x => new OperationExceptionError<TEvent,TException>(x, x.Exception as TException) as IOperationExceptionError<TEvent,TException>);
            if (filter != null)
                events = events.Where(filter);
            var r = events.ToList();
            r.ForEach(x => x.Error.Handled = true);
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
