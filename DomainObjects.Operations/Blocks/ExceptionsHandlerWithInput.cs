using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ExceptionsHandler<TEvent, TException, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TState, TOperationEvent, Tin>, IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent, Tin>
            where TException : Exception
            where TOperationEvent : IOperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
        public ExceptionsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            errors = stackEvents.FilterUnhandledException(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ExceptionsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => func(this);
        }

        public ExceptionsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Action<IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent, Tin>> action, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<IOperationExceptionError<TEvent, TException>> IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent, Tin>.ExceptionErrors => errors;

        Emptyable<Tin> IEventHandlerWithInput<TState, TOperationEvent, Tin>.Result => Input;
    }

}
