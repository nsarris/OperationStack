using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class EventHandlerBlockBase<TState> : StackBlockBase<TState>, IResultVoidDispatcher
    {
        ResultVoidDispatcher resultDispather = new ResultVoidDispatcher();
        public EventHandlerBlockBase(string tag, TState state, IStackEvents stackEvents) : base(tag, state, stackEvents)
        {
            
        }

        public BlockResultVoid Break()
        {
            return resultDispather.Break();
        }

        public BlockResultVoid End()
        {
            return resultDispather.End();
        }

        public BlockResultVoid End(object overrideResult)
        {
            return resultDispather.End(overrideResult);
        }

        public BlockResultVoid Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        public BlockResultVoid Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
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

        public BlockResultVoid Return()
        {
            return resultDispather.Return();
        }

        public BlockResultVoid Skip(int i)
        {
            return resultDispather.Skip(i); 
        }

        public BlockResultVoid Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i,overrideInput);
        }
        
    }

    public class EventHandlerBlockBase<TState, Tin> : StackBlockBase<TState>, IStackBlock<TState, Tin>, IResultDispatcher<Tin>
    {
        ResultDispatcher<Tin> resultDispather = new ResultDispatcher<Tin>();
        internal EventHandlerBlockBase(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents) : base(tag, state, stackEvents)
        {
            Input = input;
        }

        public Emptyable<Tin> Input {get;protected set;}

        public BlockResult<Tin> Break()
        {
            return resultDispather.Break();
        }

        public BlockResult<Tin> End()
        {
            return resultDispather.End();
        }

        public BlockResult<Tin> End(object overrideResult)
        {
            return resultDispather.End(overrideResult);
        }

        public BlockResult<Tin> Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        public BlockResult<Tin> Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
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

        public BlockResult<Tin> Return()
        {
            return resultDispather.Return(Input.Value);
        }

        public BlockResult<Tin> Return(Tin result)
        {
            return resultDispather.Return(result);
        }

        public BlockResult<Tin> Skip(int i)
        {
            return resultDispather.Skip(i);
        }

        public BlockResult<Tin> Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i, overrideInput);
        }
    }

    public class EventsHandler<TEvent, TState> : EventHandlerBlockBase<TState>, IEventsHandler<TEvent, TState>
            where TEvent : IOperationEvent
    {
        IEnumerable<TEvent> events;
        
        private EventsHandler(string tag, TState state, IStackEvents stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            events = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TState state, IStackEvents stackEvents, Func<IEventsHandler<TEvent, TState>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TState state, IStackEvents stackEvents, Action<IEventsHandler<TEvent, TState>> action, Func<TEvent, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TState>.Events => events;
    }
    

    public class EventsHandler<TEvent, TState, Tin> : EventHandlerBlockBase<TState, Tin>, IEventsHandler<TEvent, TState, Tin>
            where TEvent : IOperationEvent
    {
        IEnumerable<TEvent> events;
        private EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<TEvent, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            events = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !events.Any();
        }

        public EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IEventsHandler<TEvent, TState,Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            : this(tag, state,input, stackEvents)
        {
            executor = () => func(this);
        }

        public EventsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Action<IEventsHandler<TEvent, TState,Tin>> action, Func<TEvent, bool> filter = null)
            : this(tag, state, input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TEvent> IEventsHandler<TEvent, TState, Tin>.Events => events;

        Emptyable<Tin> IEventHandlerWithInput<TState, Tin>.Result => Input;
    }

   
    public class ErrorsHandler<TError, TState> : EventHandlerBlockBase<TState>, IErrorsHandler<TError, TState>
            where TError : IOperationError
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TState state, IStackEvents stackEvents,  Func<TError, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag, TState state, IStackEvents stackEvents, Func<IErrorsHandler<TError, TState>, BlockResultVoid> func, Func<TError, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TState state, IStackEvents stackEvents, Action<IErrorsHandler<TError, TState>> action, Func<TError, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }
        IEnumerable<TError> IErrorsHandler<TError, TState>.Errors => errors;
    }

    public class ErrorsHandler<TError, TState, Tin> : EventHandlerBlockBase<TState, Tin>, IErrorsHandler<TError, TState, Tin>
            where TError : IOperationError
    {
        IEnumerable<TError> errors;
        private ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<TError, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            errors = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IErrorsHandler<TError, TState,Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            : this(tag, state, input,stackEvents)
        {
            executor = () => func(this);
        }

        public ErrorsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Action<IErrorsHandler<TError, TState,Tin>> action, Func<TError, bool> filter = null)
            : this(tag, state,input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<TError> IErrorsHandler<TError, TState, Tin>.Errors => errors;

        Emptyable<Tin> IEventHandlerWithInput<TState, Tin>.Result => Input;
    }

    public class ExceptionsHandler<TException, TState> : EventHandlerBlockBase<TState>, IExceptionsErrorHandler<TException, TState>
            where TException : Exception
    {
        IEnumerable<IOperationExceptionError<TException>> errors;
        private ExceptionsHandler(string tag, TState state, IStackEvents stackEvents, Func<IOperationExceptionError<TException>, bool> filter = null)
            : base(tag, state, stackEvents)
        {
            errors = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ExceptionsHandler(string tag, TState state, IStackEvents stackEvents, Func<IExceptionsErrorHandler<TException, TState>, BlockResultVoid> func, Func<IOperationExceptionError<TException>, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        public ExceptionsHandler(string tag, TState state, IStackEvents stackEvents, Action<IExceptionsErrorHandler<TException, TState>> action, Func<IOperationExceptionError<TException>, bool> filter = null)
            : this(tag, state, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<IOperationExceptionError<TException>> IExceptionsErrorHandler<TException, TState>.ExceptionErrors => errors;
    }

    public class ExceptionsHandler<TException, TState, Tin> : EventHandlerBlockBase<TState, Tin>, IExceptionsErrorHandler<TException, TState, Tin>
            where TException : Exception
    {
        IEnumerable<IOperationExceptionError<TException>> errors;
        public ExceptionsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IOperationExceptionError<TException>, bool> filter = null)
            : base(tag, state, input, stackEvents)
        {
            Input = input;
            errors = stackEvents.FilterUnhandled(filter, true);
            IsEmptyEventBlock = !errors.Any();
        }

        public ExceptionsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Func<IExceptionsErrorHandler<TException, TState,Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TException>, bool> filter = null)
            : this(tag, state,input, stackEvents)
        {
            executor = () => func(this);
        }

        public ExceptionsHandler(string tag, TState state, Emptyable<Tin> input, IStackEvents stackEvents, Action<IExceptionsErrorHandler<TException, TState,Tin>> action, Func<IOperationExceptionError<TException>, bool> filter = null)
            : this(tag, state,input, stackEvents)
        {
            executor = () => { action(this); return Return(); };
        }

        IEnumerable<IOperationExceptionError<TException>> IExceptionsErrorHandler<TException, TState, Tin>.ExceptionErrors => errors;

        Emptyable<Tin> IEventHandlerWithInput<TState, Tin>.Result => Input;
    }

}
