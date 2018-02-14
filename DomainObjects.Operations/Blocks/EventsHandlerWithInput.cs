using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class EventsHandler<TEvent, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TState, TOperationEvent, Tin>, IEventsHandler<TEvent, TState, TOperationEvent, Tin>
            where TOperationEvent : IOperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> events;
        private EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            events = stackEvents.FilterUnhandled(filter);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IEventsHandler<TEvent, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Action<IEventsHandler<TEvent, TState, TOperationEvent, Tin>> action, Func<TEvent, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TState, TOperationEvent, Tin>.Events => events;

        Emptyable<Tin> IEventHandlerWithInput<TState, TOperationEvent, Tin>.Result => Input;
    }

}
