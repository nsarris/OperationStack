using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ErrorsHandler<TError, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TState, TOperationEvent, Tin>, IErrorsHandler<TError, TState, TOperationEvent, Tin>
            where TOperationEvent : IOperationEvent
            where TError : TOperationEvent
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<TError, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            errors = stackEvents.FilterUnhandled(filter);
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IErrorsHandler<TError, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Action<IErrorsHandler<TError, TState, TOperationEvent, Tin>> action, Func<TError, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TError> IErrorsHandler<TError, TState, TOperationEvent, Tin>.Errors => errors;

        Emptyable<Tin> IEventHandlerWithInput<TState, TOperationEvent, Tin>.Result => Input;
    }

}
