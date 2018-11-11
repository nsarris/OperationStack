using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class ErrorsHandler<TError, Tin> : EventHandlerBlockBase<Tin>, IErrorsHandler<TError, TInput, TState, Tin>
                where TError : OperationEvent
        {
            readonly IEnumerable<TError> errors;
            private ErrorsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
                : base(tag, stackInput, state, input, stackEvents)
            {
                Input = input;
                errors = stackEvents.FilterErrors(handled, filter);
                if (handle) foreach (var e in errors) e.Handle();
                IsEmptyEventBlock = !errors.Any();
            }

            public ErrorsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IErrorsHandler<TError, TInput, TState, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, input, stackEvents, filter, handled, handle)
            {
                executor = () => func(this);
            }

            public ErrorsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Action<IErrorsHandler<TError, TInput, TState, Tin>> action, Func<TError, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, input, stackEvents, filter, handled, handle)
            {
                executor = () => { action(this); return Return(input.Value); };
            }

            IEnumerable<TError> IErrorsHandler<TError, TInput, TState, Tin>.Errors => errors;

            Emptyable<Tin> IEventHandlerWithInput<TInput, TState, Tin>.Result => Input;
        }
    }
}
