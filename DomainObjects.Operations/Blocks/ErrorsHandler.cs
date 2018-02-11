using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public class ErrorsHandler<TError, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IErrorsHandler<TError, TState, TOperationEvent>
            where TOperationEvent : IOperationEvent
            where TError : TOperationEvent
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<TError, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IErrorsHandler<TError, TState, TOperationEvent>> action, Func<TError, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }
        IEnumerable<TError> IErrorsHandler<TError, TState, TOperationEvent>.Errors => errors;
    }

}
