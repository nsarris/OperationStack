using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ErrorsHandler<TError,TInput, TState, TOperationEvent> : EventHandlerBlockBase<TInput, TState, TOperationEvent>, IErrorsHandler<TError, TInput, TState, TOperationEvent>
            where TOperationEvent : OperationEvent
            where TError : TOperationEvent
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<TError, bool> filter = null,bool? handled = null, bool handle = false)
            : base(tag, stackInput, state, stackEvents)
        {
            errors = stackEvents.FilterErrors(handled, filter);
            if (handle) foreach (var e in errors) e.Handle();
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag,TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, stackInput, state, stackEvents, filter, handled, handle)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent>> action, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, stackInput,state, stackEvents, filter, handled, handle)
        {
            executor = () => { action(this); return Return(); };
        }
        IEnumerable<TError> IErrorsHandler<TError, TInput, TState, TOperationEvent>.Errors => errors;
    }

}
