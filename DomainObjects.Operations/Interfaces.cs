using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    


    public interface IResultDispatcher<T>
    {
        //BlockResult<T> Return();
        BlockResult<T> Return(T result, bool success);
        BlockResult<T> Return(T result);
        BlockResult<T> End(bool success);
        BlockResult<T> End(bool success, object overrideResult);
        BlockResult<T> Break(bool success);
        BlockResult<T> Reset();
        BlockResult<T> Reset(object state);
        BlockResult<T> Restart();
        BlockResult<T> Goto(string tag, bool success);
        BlockResult<T> Goto(string tag);
        BlockResult<T> Goto(string tag, object overrideInput, bool success);
        BlockResult<T> Goto(string tag, object overrideInput);
        BlockResult<T> Skip(int i, bool success);
        BlockResult<T> Skip(int i);
        BlockResult<T> Skip(int i, object overrideInput, bool success);
        BlockResult<T> Skip(int i, object overrideInput);
    }

    public interface IResultVoidDispatcher
    {
        BlockResultVoid Return(bool success = true);
        BlockResultVoid Return();
        BlockResultVoid End(bool success);
        BlockResultVoid End(bool success, object overrideResult);
        BlockResultVoid Break(bool success);
        BlockResultVoid Reset();
        BlockResultVoid Reset(object state);
        BlockResultVoid Restart();
        BlockResultVoid Goto(string tag, bool success = true);
        BlockResultVoid Goto(string tag);
        BlockResultVoid Goto(string tag, object overrideInput, bool success = true);
        BlockResultVoid Goto(string tag, object overrideInput);
        BlockResultVoid Skip(int i, bool success = true);
        BlockResultVoid Skip(int i);
        BlockResultVoid Skip(int i, object overrideInput, bool success = true);
        BlockResultVoid Skip(int i, object overrideInput);
    }

    public interface IResultDispatcher
    {
        //BlockResult<T> Return<T>();
        BlockResult<T> Return<T>(T result, bool success = true);
        BlockResult<T> Return<T>(T result);
        BlockResult<T> End<T>(bool success);
        BlockResult<T> End<T>(bool success, object overrideResult);
        BlockResult<T> Break<T>(bool success);
        BlockResult<T> Reset<T>();
        BlockResult<T> Reset<T>(object state);
        BlockResult<T> Restart<T>();
        BlockResult<T> Goto<T>(string tag, bool success = true);
        BlockResult<T> Goto<T>(string tag);
        BlockResult<T> Goto<T>(string tag, object overrideInput, bool success = true);
        BlockResult<T> Goto<T>(string tag, object overrideInput);
        BlockResult<T> Skip<T>(int i, bool success = true);
        BlockResult<T> Skip<T>(int i);
        BlockResult<T> Skip<T>(int i, object overrideInput, bool success = true);
        BlockResult<T> Skip<T>(int i, object overrideInput);
    }

    public interface IStackEvents : IEnumerable<IOperationEvent>
    {
        IEnumerable<TEvent> Filter<TEvent>(Func<TEvent, bool> filter = null) where TEvent : IOperationEvent;
        IEnumerable<TEvent> FilterUnhandled<TEvent>(Func<TEvent, bool> filter = null, bool markAsHandled = false) where TEvent : IOperationEvent;
    }

    public interface IOperationEvents : IEnumerable<IOperationEvent>
    {
        void Add(IOperationEvent @event);
        void Append(IEnumerable<IOperationEvent> events);
        bool HasUnhandledException { get; }
    }

    public interface IStackBlock
    {
        IStackEvents StackEvents { get; }
    }

    public interface IStackBlock<TState> : IStackBlock
    {
        //bool PreviousBlockSuccess { get; }
        TState StackState { get; }
    }

    public interface IStackBlock<TState, T> : IStackBlock<TState>
    {
        Emptyable<T> Input { get; }
    }

    public interface IOperationBlock<TState> : IStackBlock<TState>
    {
        IOperationEvents Events { get; }
        void Append(IOperationResult res);
    }

    public interface IOperationBlock<TState, Tin> : IOperationBlock<TState>, IStackBlock<TState, Tin>
    {

    }

    public interface IQuery<TState> : IOperationBlock<TState>, IResultDispatcher
    {
        IQueryResultProxy<T> DefineResult<T>();
        IQueryResultProxy<T> DefineResult<T>(T result);
        IQueryResultProxy<T> DefineResult<T>(Expression<Func<T>> expression);
    }

    public interface ICommand<TState> : IOperationBlock<TState>, IResultVoidDispatcher
    {


    }




    

    public interface ICommand<TState, T> : ICommand<TState>, IOperationBlock<TState, T>
    {

    }

    public interface IQuery<TState, T> : IQuery<TState>, IOperationBlock<TState, T>
    {

    }



    public interface ITypedQuery<TState, T> : IOperationBlock<TState>, IResultDispatcher<T>
    {

    }

    public interface ITypedQuery<TState, Tin, Tout> : ITypedQuery<TState, Tout>, IOperationBlock<TState, Tin>
    {

    }



    public interface IEventHandler<TState> : IStackBlock<TState>, IResultVoidDispatcher
    {

    }

    public interface IEventHandlerWithInput<TState, T> : IStackBlock<TState>, IResultDispatcher<T>
    {
        Emptyable<T> Result { get; }
        BlockResult<T> Return(bool success);
        BlockResult<T> Return();
    }

   


    public interface IEventsHandler<TEvent, TState> : IEventHandler<TState>
        where TEvent : IOperationEvent
    {
        IEnumerable<TEvent> Events { get; }
    }

    public interface IEventsHandler<TEvent, TState, Tin> : IEventHandlerWithInput<TState, Tin>
        where TEvent : IOperationEvent
    {
        IEnumerable<TEvent> Events { get; }
    }

    public interface IErrorsHandler<TError, TState> : IEventHandler<TState>
        where TError : IOperationError
    {
        IEnumerable<TError> Errors { get; }
    }

    public interface IErrorsHandler<TError, TState, Tin> : IEventHandlerWithInput<TState, Tin>
        where TError : IOperationError
    {
        IEnumerable<TError> Errors { get; }
    }

    public interface IExceptionsErrorHandler<TException, TState> : IEventHandler<TState>
        where TException : Exception
    {
        IEnumerable<IOperationExceptionError<TException>> ExceptionErrors { get; }
    }

    public interface IExceptionsErrorHandler<TException, TState, Tin> : IEventHandlerWithInput<TState, Tin>
        where TException : Exception
    {
        IEnumerable<IOperationExceptionError<TException>> ExceptionErrors { get; }
    }


    public interface IQueryResultProxy<T> : IResultDispatcher<T>
    {
        T Result { get; set; }
    }


    public interface IOperationResult
    {
        bool Success { get; }
        IEnumerable<IOperationEvent> Events { get; }

        IReadOnlyList<BlockTraceResult> StackTrace { get; }
    }

    public interface ICommandResult : IOperationResult
    {

    }

    public interface IQueryResult<T> : IOperationResult
    {
        Emptyable<T> Result { get; }
    }

    public interface IOperationEvent
    {
        bool Handled { get; set; }
    }

    public interface IOperationError : IOperationEvent
    {
        Exception Exception { get; }
        bool UnhandledException { get; }
    }

    public interface IOperationExceptionError : IOperationError
    {

    }

    public interface IOperationExceptionError<TException> : IOperationExceptionError, IOperationError
        where TException : Exception
    {
        new TException Exception { get; }
    }



    

    
}
