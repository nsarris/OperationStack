﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public interface IOperation
    {
        bool SupportsSync { get; }
        bool SupportsAsync { get; }
        bool PreferAsync { get; }
    }
    public interface IQueryOperation<TOperationEvent, TResult> : IOperation
        where TOperationEvent : IOperationEvent
    {
        IQueryResult<TOperationEvent, TResult> ToResult();
        Task<IQueryResult<TOperationEvent, TResult>> ToResultAsync();
    }

    public interface ICommandOperation<TOperationEvent> : IOperation
        where TOperationEvent : IOperationEvent
    {
        ICommandResult<TOperationEvent> ToResult();
        Task<ICommandResult<TOperationEvent>> ToResultAsync();
    }

    public interface IQueryOperation<TState, TOperationEvent, TResult> : IQueryOperation<TOperationEvent, TResult>
        where TOperationEvent : IOperationEvent
    {
        IQueryResult<TState, TOperationEvent, TResult> ToResult(TState initialState);
        Task<IQueryResult<TState, TOperationEvent, TResult>> ToResultAsync(TState initialState);
    }

    public interface ICommandOperation<TState, TOperationEvent> : ICommandOperation<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        ICommandResult<TState, TOperationEvent> ToResult(TState initialState);
        Task<ICommandResult<TState, TOperationEvent>> ToResultAsync(TState initialState);
    }

    

    public interface IStackEvents<TOperationEvent> : IEnumerable<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        IEnumerable<TEvent> Filter<TEvent>(Func<TEvent, bool> filter = null) where TEvent : TOperationEvent;
        IEnumerable<TEvent> FilterUnhandled<TEvent>(Func<TEvent, bool> filter = null) where TEvent : TOperationEvent;
        IEnumerable<IOperationExceptionError<TEvent, TException>> FilterUnhandledException<TEvent,TException>(Func<IOperationExceptionError<TEvent, TException>, bool> filter = null) where TException : Exception where TEvent : TOperationEvent;
    }

    public interface IOperationEvents<TOperationEvent> : IEnumerable<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        void Add(TOperationEvent @event);
        void Add(Exception exception);
        void Throw(TOperationEvent @event);
        void Throw(Exception exception);
        void Append(IEnumerable<TOperationEvent> events);
        bool HasUnhandledErrors { get; }
    }

    public interface IStackBlock<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        IStackEvents<TOperationEvent> StackEvents { get; }
    }

    public interface IStackBlock<TState, TOperationEvent> : IStackBlock<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        //bool PreviousBlockSuccess { get; }
        TState StackState { get; set; }
    }

    public interface IStackBlock<TState, TOperationEvent, T> : IStackBlock<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        Emptyable<T> Input { get; }
    }

    public interface IOperationBlock<TState, TOperationEvent> : IStackBlock<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        IOperationEvents<TOperationEvent> Events { get; }
        void Append(IOperationResult<TOperationEvent> res);
    }

    public interface IOperationBlock<TState, TOperationEvent, Tin> : IOperationBlock<TState, TOperationEvent>, IStackBlock<TState, TOperationEvent,Tin>
        where TOperationEvent : IOperationEvent
    {

    }

    public interface IQuery<TState, TOperationEvent> : IOperationBlock<TState, TOperationEvent>, IResultDispatcher<TState>
        where TOperationEvent : IOperationEvent
    {
        IQueryResultProxy<T,TState> DefineResult<T>();
        IQueryResultProxy<T, TState> DefineResult<T>(T result);
        IQueryResultProxy<T, TState> DefineResult<T>(Expression<Func<T>> expression);
    }

    public interface ICommand<TState, TOperationEvent> : IOperationBlock<TState, TOperationEvent>, IResultVoidDispatcher<TState>
        where TOperationEvent : IOperationEvent
    {


    }




    

    public interface ICommand<TState, TOperationEvent, T> : ICommand<TState, TOperationEvent>, IOperationBlock<TState, TOperationEvent, T>
        where TOperationEvent : IOperationEvent
    {

    }

    public interface IQuery<TState, TOperationEvent, T> : IQuery<TState, TOperationEvent>, IOperationBlock<TState, TOperationEvent, T>
        where TOperationEvent : IOperationEvent
    {

    }



    public interface ITypedQuery<TState, TOperationEvent, T> : IOperationBlock<TState, TOperationEvent>, IResultDispatcher<TState>, IResultDispatcher<T,TState>
        where TOperationEvent : IOperationEvent
    {

    }

    public interface ITypedQuery<TState, TOperationEvent, Tin, Tout> : ITypedQuery<TState, TOperationEvent, Tout>, IOperationBlock<TState, TOperationEvent, Tin>
        where TOperationEvent : IOperationEvent
    {

    }



    public interface IEventHandler<TState, TOperationEvent> : IStackBlock<TState, TOperationEvent>, IResultVoidDispatcher<TState>
        where TOperationEvent : IOperationEvent
    {

    }

    public interface IEventHandlerWithInput<TState, TOperationEvent, T> : IStackBlock<TState, TOperationEvent, T>, IResultDispatcher<T,TState>
        where TOperationEvent : IOperationEvent
    {
        Emptyable<T> Result { get; }
        BlockResult<T> Return();
    }

   


    public interface IEventsHandler<TEvent, TState, TOperationEvent> : IEventHandler<TState,TOperationEvent>
        where TOperationEvent : IOperationEvent
        where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> Events { get; }
    }

    public interface IEventsHandler<TEvent, TState, TOperationEvent, Tin> : IEventHandlerWithInput<TState, TOperationEvent, Tin>
        where TOperationEvent : IOperationEvent
        where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> Events { get; }
    }

    public interface IErrorsHandler<TError, TState, TOperationEvent> : IEventHandler<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<TError> Errors { get; }
    }

    public interface IErrorsHandler<TError, TState, TOperationEvent, Tin> : IEventHandlerWithInput<TState, TOperationEvent, Tin>
        where TOperationEvent : IOperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<TError> Errors { get; }
    }

    public interface IExceptionsErrorHandler<TError,TException, TState, TOperationEvent> : IEventHandler<TState, TOperationEvent>
        where TException : Exception
        where TOperationEvent : IOperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TError, TException>> ExceptionErrors { get; }
    }

    public interface IExceptionsErrorHandler<TError, TException, TState, TOperationEvent, Tin> : IEventHandlerWithInput<TState, TOperationEvent, Tin>
        where TException : Exception
        where TOperationEvent : IOperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TError, TException>> ExceptionErrors { get; }
    }


    public interface IQueryResultProxy<T,TState> : IResultDispatcher<T,TState>
    {
        T Result { get; set; }
    }


    public interface IOperationResult
    {
        bool Success { get; }
        object StackState { get; }
        IEnumerable<IOperationEvent> Events { get; }
    }

    public interface ICommandResult : IOperationResult
    {
    }

    public interface IQueryResult<T> : IOperationResult
    {
        Emptyable<T> Result { get; }
    }

    public interface IOperationResult<TOperationEvent> : IOperationResult
        where TOperationEvent : IOperationEvent
    {
        new IEnumerable<TOperationEvent> Events { get; }
        IReadOnlyList<BlockTraceResult<TOperationEvent>> StackTrace { get; }
    }

    public interface ICommandResult<TOperationEvent> : ICommandResult,IOperationResult<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {

    }

    public interface IQueryResult<TOperationEvent,T> : IQueryResult<T>,IOperationResult<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        
    }

    public interface IOperationResult<TState, TOperationEvent> : IOperationResult
        where TOperationEvent : IOperationEvent
    {
        new IEnumerable<TOperationEvent> Events { get; }
        IReadOnlyList<BlockTraceResult<TOperationEvent>> StackTrace { get; }
        new TState StackState { get; }
    }

    public interface ICommandResult<TState, TOperationEvent> : ICommandResult, ICommandResult<TOperationEvent>,  IOperationResult<TState,TOperationEvent>
        where TOperationEvent : IOperationEvent
    {

    }

    public interface IQueryResult<TState, TOperationEvent, T> : IQueryResult<T>, IQueryResult<TOperationEvent, T>, IOperationResult<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {

    }

    public interface IOperationEvent
    {
        string Message { get; }
        string UserMessage { get; set; }
        bool IsHandled { get; }
        bool IsError { get; }
        bool IsException { get; }
        Exception Exception { get; }
        void Handle();
        void Throw();
        void FromException(Exception e);
    }

    
    public interface IOperationExceptionError<TEvent, TException>
        where TException : Exception
        where TEvent : IOperationEvent
    {
        TEvent Error { get; }
        TException Exception { get; }
    }




}