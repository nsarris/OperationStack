using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class EventHandlerBlockBase<TState, TOperationEvent> : StackBlockBase<TState, TOperationEvent>, IResultVoidDispatcher
        where TOperationEvent : IOperationEvent
    {
        ResultVoidDispatcher resultDispather = new ResultVoidDispatcher();
        public EventHandlerBlockBase(string tag, TState state, IStackEvents<TOperationEvent> stackEvents) : base(tag, state, stackEvents)
        {

        }

        public BlockResultVoid Break(bool success)
        {
            return resultDispather.Break(success);
        }

        public BlockResultVoid End(bool success = true)
        {
            return resultDispather.End(success);
        }

        public BlockResultVoid End(bool success, object overrideResult)
        {
            return resultDispather.End(success, overrideResult);
        }

        public BlockResultVoid Reset()
        {
            return resultDispather.Reset();
        }

        public BlockResultVoid Reset(object state)
        {
            return resultDispather.Reset(state);
        }

        public BlockResultVoid Restart()
        {
            return resultDispather.Restart();
        }

        public BlockResultVoid Return(bool success)
        {
            return resultDispather.Return(success);
        }

        public BlockResultVoid Return()
        {
            return resultDispather.Return();
        }

        public BlockResultVoid Goto(string tag, bool success)
        {
            return resultDispather.Goto(tag, success);
        }

        public BlockResultVoid Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        public BlockResultVoid Goto(string tag, object overrideInput, bool success)
        {
            return resultDispather.Goto(tag, overrideInput, success);
        }

        public BlockResultVoid Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
        }

        public BlockResultVoid Skip(int i, bool success)
        {
            return resultDispather.Skip(i, success);
        }

        public BlockResultVoid Skip(int i)
        {
            return resultDispather.Skip(i);
        }

        public BlockResultVoid Skip(int i, object overrideInput, bool success)
        {
            return resultDispather.Skip(i, overrideInput, success);
        }

        public BlockResultVoid Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i, overrideInput);
        }

    }

    public class EventHandlerBlockBase<TState, TOperationEvent, Tin> : StackBlockBase<TState, TOperationEvent>, IStackBlock<TState, TOperationEvent, Tin>, IResultDispatcher<Tin>
        where TOperationEvent : IOperationEvent
    {
        ResultDispatcher<Tin> resultDispather = new ResultDispatcher<Tin>();
        internal EventHandlerBlockBase(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents) : base(tag, state, stackEvents)
        {
            Input = input;
        }

        public Emptyable<Tin> Input { get; protected set; }

        public BlockResult<Tin> Break(bool success)
        {
            return resultDispather.Break(success);
        }

        public BlockResult<Tin> End(bool success)
        {
            return resultDispather.End(success);
        }

        public BlockResult<Tin> End(bool success, object overrideResult)
        {
            return resultDispather.End(success, overrideResult);
        }



        public BlockResult<Tin> Reset()
        {
            return resultDispather.Reset();
        }

        public BlockResult<Tin> Reset(object state)
        {
            return resultDispather.Reset(state);
        }

        public BlockResult<Tin> Restart()
        {
            return resultDispather.Restart();
        }

        public BlockResult<Tin> Return(bool success)
        {
            return resultDispather.Return(Input.Value, success);
        }

        public BlockResult<Tin> Return()
        {
            return resultDispather.Return(Input.Value);
        }

        public BlockResult<Tin> Return(Tin result, bool success)
        {
            return resultDispather.Return(result, success);
        }

        public BlockResult<Tin> Return(Tin result)
        {
            return resultDispather.Return(result);
        }

        public BlockResult<Tin> Goto(string tag, bool success)
        {
            return resultDispather.Goto(tag, success);
        }

        public BlockResult<Tin> Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        public BlockResult<Tin> Goto(string tag, object overrideInput, bool success)
        {
            return resultDispather.Goto(tag, overrideInput, success);
        }

        public BlockResult<Tin> Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
        }

        public BlockResult<Tin> Skip(int i, bool success = true)
        {
            return resultDispather.Skip(i, success);
        }

        public BlockResult<Tin> Skip(int i)
        {
            return resultDispather.Skip(i);
        }

        public BlockResult<Tin> Skip(int i, object overrideInput, bool success = true)
        {
            return resultDispather.Skip(i, overrideInput, success);
        }

        public BlockResult<Tin> Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i, overrideInput);
        }
    }

    public class EventsHandler<TEvent, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IEventsHandler<TEvent, TState, TOperationEvent>
            where TOperationEvent : IOperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> events;

        private EventsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            events = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IEventsHandler<TEvent, TState, TOperationEvent>> action, Func<TEvent, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TState, TOperationEvent>.Events => events;
    }


    public class EventsHandler<TEvent, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TState, TOperationEvent, Tin>, IEventsHandler<TEvent, TState, TOperationEvent, Tin>
            where TOperationEvent : IOperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> events;
        private EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            events = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IEventsHandler<TEvent, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Action<IEventsHandler<TEvent, TState, TOperationEvent, Tin>> action, Func<TEvent, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TState, TOperationEvent, Tin>.Events => events;

        Emptyable<Tin> IEventHandlerWithInput<TState, TOperationEvent, Tin>.Result => Input;
    }


    public class ErrorsHandler<TError, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IErrorsHandler<TError, TState, TOperationEvent>
            where TOperationEvent : IOperationEvent
            where TError : TOperationEvent
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<TError, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<IErrorsHandler<TError, TState, TOperationEvent>> action, Func<TError, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }
        IEnumerable<TError> IErrorsHandler<TError, TState, TOperationEvent>.Errors => errors;
    }

    public class ErrorsHandler<TError, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TState, TOperationEvent, Tin>, IErrorsHandler<TError, TState, TOperationEvent, Tin>
            where TOperationEvent : IOperationEvent
            where TError : TOperationEvent
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<TError, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            errors = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Func<IErrorsHandler<TError, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents, Action<IErrorsHandler<TError, TState, TOperationEvent, Tin>> action, Func<TError, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TError> IErrorsHandler<TError, TState, TOperationEvent, Tin>.Errors => errors;

        Emptyable<Tin> IEventHandlerWithInput<TState, TOperationEvent, Tin>.Result => Input;
    }

    public class ExceptionsHandler<TEvent, TException, TState, TOperationEvent> : EventHandlerBlockBase<TState, TOperationEvent>, IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent>
            where TException : Exception
            where TOperationEvent : IOperationEvent
            where TEvent : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TEvent, TException>> errors;
        private ExceptionsHandler(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationExceptionError<TEvent, TException>, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterUnhandledException(filter, true);
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

    public class ExceptionsHandler<TEvent, TException, TState, TOperationEvent, Tin> : EventHandlerBlockBase<TState, TOperationEvent, Tin>, IExceptionsErrorHandler<TEvent, TException, TState, TOperationEvent, Tin>
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
