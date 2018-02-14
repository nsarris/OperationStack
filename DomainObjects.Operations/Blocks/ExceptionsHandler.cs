using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ExceptionsHandler<TEvent, TException, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>
            where TException : Exception
            where TOperationEvent : IOperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
        private ExceptionsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterUnhandledException(filter);
            IsEmptyEventBlock = !errors.Any();
        }

        public ExceptionsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        public ExceptionsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>> action, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<IOperationExceptionError<TEvent, TException>> IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>.ExceptionErrors => errors;
    }

}
