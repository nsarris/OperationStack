using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ExceptionsHandler<TEvent, TException, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>
            where TException : Exception
            where TOperationEvent : OperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
        private ExceptionsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterExceptions(handled, filter);
            if (handle) foreach (var e in errors) e.Error.Handle();
            IsEmptyEventBlock = !errors.Any();
        }

        public ExceptionsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, state, stackEvents, filter, handled, handle)
        {
            executor = () => func(this);
        }

        public ExceptionsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>> action, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, state, stackEvents, filter, handled, handle)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<IOperationExceptionError<TEvent, TException>> IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>.ExceptionErrors => errors;
    }

}
