using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class ExceptionsHandler<TEvent, TException> : EventHandlerBlockBase, IExceptionsErrorHandler<TEvent, TException, TInput, TState>
            where TException : Exception
            where TEvent : OperationEvent
        {
            readonly IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
            private ExceptionsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
                : base(tag, stackInput, state, stackEvents)
            {
                errors = stackEvents.FilterExceptions(handled, filter);
                if (handle) foreach (var e in errors) e.Error.Handle();
                IsEmptyEventBlock = !errors.Any();
            }

            public ExceptionsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IExceptionsErrorHandler<TEvent, TException, TInput, TState>, BlockResultVoid> func, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, stackEvents, filter, handled, handle)
            {
                executor = () => func(this);
            }

            public ExceptionsHandler(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Action<IExceptionsErrorHandler<TEvent, TException, TInput, TState>> action, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
                : this(tag, stackInput, state, stackEvents, filter, handled, handle)
            {
                executor = () => { action(this); return Return(); };
            }

            IEnumerable<IOperationExceptionError<TEvent, TException>> IExceptionsErrorHandler<TEvent, TException, TInput, TState>.ExceptionErrors => errors;
        }
    }
}
