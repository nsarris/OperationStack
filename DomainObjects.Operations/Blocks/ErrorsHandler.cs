using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ErrorsHandler<TError, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IErrorsHandler<TError, TState, TOperationEvent>
            where TOperationEvent : OperationEvent
            where TError : TOperationEvent
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<TError, bool> filter = null,bool? handled = null, bool handle = false)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterErrors(handled, filter);
            if (handle) foreach (var e in errors) e.Handle();
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, state, stackEvents, filter, handled, handle)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IErrorsHandler<TError, TState, TOperationEvent>> action, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, state, stackEvents, filter, handled, handle)
        {
            executor = () => { action(this); return Return(); };
        }
        IEnumerable<TError> IErrorsHandler<TError, TState, TOperationEvent>.Errors => errors;
    }

}
