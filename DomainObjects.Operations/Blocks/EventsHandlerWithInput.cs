using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class EventsHandler<TEvent, Tin> : EventHandlerBlockBase<Tin>, IEventsHandler<TEvent, TInput, TState, Tin>
            where TEvent : OperationEvent
        {
            readonly IEnumerable<TEvent> events;
            private EventsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<TEvent, bool> filter = null)
                : base(tag, stackInput, state, input, stackEvents)
            {
                Input = input;
                events = stackEvents.FilterErrors(null, filter);
                IsEmptyEventBlock = !events.Any();
            }

            public EventsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IEventsHandler<TEvent, TInput, TState, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
                : this(tag, stackInput, state, input, stackEvents, filter)
            {
                executor = () => func(this);
            }

            public EventsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Action<IEventsHandler<TEvent, TInput, TState, Tin>> action, Func<TEvent, bool> filter = null)
                : this(tag, stackInput, state, input, stackEvents, filter)
            {
                executor = () => { action(this); return Return(input.Value); };
            }

            IEnumerable<TEvent> IEventsHandler<TEvent, TInput, TState, Tin>.Events => events;

            Emptyable<Tin> IEventHandlerWithInput<TInput, TState, Tin>.Result => Input;
        }
    }
}
