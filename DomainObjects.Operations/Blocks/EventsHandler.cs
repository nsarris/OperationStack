using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class EventsHandler<TEvent, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IEventsHandler<TEvent, TState, TOperationEvent>
            where TOperationEvent : OperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> events;

        private EventsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            events = stackEvents.FilterErrors(null, filter);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            : this(tag, state, stackEvents, filter)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IEventsHandler<TEvent, TState, TOperationEvent>> action, Func<TEvent, bool> filter = null)
            : this(tag, state, stackEvents, filter)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TState, TOperationEvent>.Events => events;
    }


    

}
