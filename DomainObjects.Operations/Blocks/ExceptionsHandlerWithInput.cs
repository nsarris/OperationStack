using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class ExceptionsHandler<TEvent, TException, Tin> : EventHandlerBlockBase<Tin>, IExceptionsErrorHandler<TEvent, TException, TInput, TState, Tin>
            where TException : Exception
            where TEvent : OperationEvent
        {
            readonly IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
            public ExceptionsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
                : base(tag, stackInput, state, input, stackEvents)
            {
                Input = input;
                errors = stackEvents.FilterExceptions(handled, filter);
                if (handle) foreach (var e in errors) e.Error.Handle();
                IsEmptyEventBlock = !errors.Any();
            }

            public ExceptionsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IExceptionsErrorHandler<TEvent, TException, TInput, TState, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, input, stackEvents, filter, handled, handle)
            {
                executor = () => func(this);
            }

            public ExceptionsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Action<IExceptionsErrorHandler<TEvent, TException, TInput, TState, Tin>> action, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, input, stackEvents, filter, handled)
            {
                executor = () => { action(this); return Return(input.Value); };
            }

            IEnumerable<IOperationExceptionError<TEvent, TException>> IExceptionsErrorHandler<TEvent, TException, TInput, TState, Tin>.ExceptionErrors => errors;

            Emptyable<Tin> IEventHandlerWithInput<TInput, TState, Tin>.Result => Input;
        }
    }
}
