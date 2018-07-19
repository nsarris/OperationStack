using System;
using System.Linq;
using System.Collections.Generic;

namespace DomainObjects.Operations
{
    internal class ExceptionsHandler<TEvent, TException, TInput, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TInput, TState, TOperationEvent, Tin>, IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent, Tin>
            where TException : Exception
            where TOperationEvent : OperationEvent
            where TEvent : TOperationEvent
    {
        readonly IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
        public ExceptionsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : base(tag, stackInput, state, input, stackEvents)
        {
            Input = input;
            errors = stackEvents.FilterExceptions(handled, filter);
            if (handle) foreach (var e in errors) e.Error.Handle();
            IsEmptyEventBlock = !errors.Any();
        }

        public ExceptionsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, stackInput, state,input, stackEvents, filter, handled, handle)
        {
            executor = () => func(this);
        }

        public ExceptionsHandler(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Action<IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent, Tin>> action, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null, bool? handled = null, bool handle = false)
            : this(tag, stackInput, state, input, stackEvents, filter, handled)
        {
            executor = () => { action(this); return Return(input.Value); };
        }

        IEnumerable<IOperationExceptionError<TEvent, TException>> IExceptionsErrorHandler<TEvent, TException, TInput, TState, TOperationEvent, Tin>.ExceptionErrors => errors;

        Emptyable<Tin> IEventHandlerWithInput<TInput,TState, TOperationEvent, Tin>.Result => Input;
    }

}
