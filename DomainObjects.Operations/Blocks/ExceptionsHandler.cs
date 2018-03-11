using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ExceptionsHandler<TEvent, TException, TInput, TState, TOperationEvent> : EventHandlerBlockBase<TInput, TState, TOperationEvent>, IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent>
            where TException : Exception
            where TOperationEvent : OperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
        private ExceptionsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : base(tag, stackInput, state, stackEvents)
        {
            errors = stackEvents.FilterExceptions(handled, filter);
            if (handle) foreach (var e in errors) e.Error.Handle();
            IsEmptyEventBlock = !errors.Any();
        }

        public ExceptionsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, stackInput, state, stackEvents, filter, handled, handle)
        {
            executor = () => func(this);
        }

        public ExceptionsHandler(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent>> action, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, stackInput, state, stackEvents, filter, handled, handle)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<IOperationExceptionError<TEvent, TException>> IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent>.ExceptionErrors => errors;
    }

}
