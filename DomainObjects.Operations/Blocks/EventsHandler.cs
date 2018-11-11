using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class EventsHandler<TEvent> : EventHandlerBlockBase, IEventsHandler<TEvent, TInput, TState>
                where TEvent : OperationEvent
        {
            readonly IEnumerable<TEvent> events;

            private EventsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<TEvent, bool> filter = null)
                : base(tag, stackInput, state, stackEvents)
            {
                events = stackEvents.FilterErrors(null, filter);
                IsEmptyEventBlock = !events.Any();
            }

            public EventsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IEventsHandler<TEvent, TInput, TState>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
                : this(tag, stackInput, state, stackEvents, filter)
            {
                executor = () => func(this);
            }

            public EventsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Action<IEventsHandler<TEvent, TInput, TState>> action, Func<TEvent, bool> filter = null)
                : this(tag, stackInput, state, stackEvents, filter)
            {
                executor = () => { action(this); return Return(); };
            }

            IEnumerable<TEvent> IEventsHandler<TEvent, TInput, TState>.Events => events;
        }
    }
}
