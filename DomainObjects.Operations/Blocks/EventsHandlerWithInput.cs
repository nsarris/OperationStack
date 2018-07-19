using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class EventsHandler<TEvent, TInput, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TInput, TState, TOperationEvent, Tin>, IEventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>
            where TOperationEvent : OperationEvent
            where TEvent : TOperationEvent
    {
        readonly IEnumerable<TEvent> events;
        private EventsHandler(string tag,TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, stackInput, state, input, stackEvents)
        {
            Input = input;
            events = stackEvents.FilterErrors(null, filter);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            : this(tag, stackInput, state, input, stackEvents, filter)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>> action, Func<TEvent, bool> filter = null)
            : this(tag, stackInput, state, input, stackEvents, filter)
        {
            executor = () => { action(this); return Return(input.Value); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>.Events => events;

        Emptyable<Tin> IEventHandlerWithInput<TInput,TState, TOperationEvent, Tin>.Result => Input;
    }

}
