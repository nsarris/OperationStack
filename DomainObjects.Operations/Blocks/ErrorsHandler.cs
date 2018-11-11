using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class ErrorsHandler<TError> : EventHandlerBlockBase, IErrorsHandler<TError, TInput, TState>
            where TError : OperationEvent
        {
            readonly IEnumerable<TError> errors;
            private ErrorsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
                : base(tag, stackInput, state, stackEvents)
            {
                errors = stackEvents.FilterErrors(handled, filter);
                if (handle) foreach (var e in errors) e.Handle();
                IsEmptyEventBlock = !errors.Any();
            }

            public ErrorsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IErrorsHandler<TError, TInput, TState>, BlockResultVoid> func, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, stackEvents, filter, handled, handle)
            {
                executor = () => func(this);
            }

            public ErrorsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Action<IErrorsHandler<TError, TInput, TState>> action, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, stackEvents, filter, handled, handle)
            {
                executor = () => { action(this); return Return(); };
            }
            IEnumerable<TError> IErrorsHandler<TError, TInput, TState>.Errors => errors;

            public IEnumerable<OperationEvent> Errors => errors;
        }
    }
}
