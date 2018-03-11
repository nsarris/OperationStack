using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class EventsHandler<TEvent, TInput, TState, TOperationEvent> : EventHandlerBlockBase<TInput, TState, TOperationEvent>, IEventsHandler<TEvent, TInput, TState, TOperationEvent>
            where TOperationEvent : OperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> events;

        private EventsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, stackInput, state, stackEvents)
        {
            events = stackEvents.FilterErrors(null, filter);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            : this(tag, stackInput, state, stackEvents, filter)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent>> action, Func<TEvent, bool> filter = null)
            : this(tag, stackInput, state, stackEvents, filter)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TInput, TState, TOperationEvent>.Events => events;
    }




}
