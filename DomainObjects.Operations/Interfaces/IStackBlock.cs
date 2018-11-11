using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public interface IStackBlock
    {
        string Tag { get; }
        IBlockResult Execute(bool timeMeasurment);
        Task<IBlockResult> ExecuteAsync(bool timeMeasurment);
        IEmptyable Input { get; }
        object StackState { get; }
        IStackEvents StackEvents { get; }
        void Throw(OperationEvent error);
        IOperationEvents Events { get; }
        IEnumerable<BlockTraceResult> InnerStackTrace { get; }
    }

    public interface IStackBlock<TInput, TState> : IStackBlock
    {
        new TState StackState { get; set; }
        TInput StackInput { get; }
    }

    public interface IStackBlock<TInput, TState, T> : IStackBlock<TInput, TState>
    {
        new Emptyable<T> Input { get; }
    }

    public interface IOperationBlock<TInput, TState> : IStackBlock<TInput, TState>
    {
        void Append(IOperationResult res);
    }

    public interface IOperationBlock<TInput, TState, Tin> : IOperationBlock<TInput, TState>, IStackBlock<TInput, TState,Tin>
    {

    }

    public interface IQuery<TInput, TState> : IOperationBlock<TInput, TState>, IResultDispatcher<TState>
    {
        IQueryResultProxy<T,TState> DefineResult<T>();
        IQueryResultProxy<T, TState> DefineResult<T>(T result);
        IQueryResultProxy<T, TState> DefineResult<T>(Expression<Func<T>> expression);
    }

    public interface ICommand<TInput, TState> : IOperationBlock<TInput, TState>, IResultVoidDispatcher<TState>
    {


    }

    public interface ICommand<TInput, TState, T> : ICommand<TInput, TState>, IOperationBlock<TInput, TState, T>
    {

    }

    public interface IQuery<TInput, TState, T> : IQuery<TInput, TState>, IOperationBlock<TInput, TState, T>
    {

    }

    public interface ITypedQuery<TInput, TState, T> : IOperationBlock<TInput, TState>, IResultDispatcher<TState>, IResultDispatcher<T,TState>
    {

    }

    public interface ITypedQuery<TInput, TState, Tin, Tout> : ITypedQuery<TInput, TState, Tout>, IOperationBlock<TInput, TState, Tin>
    {

    }

    public interface IEventHandler<TInput, TState> : IStackBlock<TInput, TState>, IResultVoidDispatcher<TState>
    {

    }

    public interface IEventHandlerWithInput<TInput, TState, T> : IStackBlock<TInput, TState, T>, IResultDispatcher<T,TState>
    {
        Emptyable<T> Result { get; }
        BlockResult<T> Return();
    }


    public interface IEventsHandler<TEvent, TInput, TState> : IEventHandler<TInput, TState>
        where TEvent : OperationEvent
    {
        new IEnumerable<TEvent> Events { get; }
    }

    public interface IEventsHandler<TEvent, TInput, TState, Tin> : IEventHandlerWithInput<TInput, TState, Tin>
        where TEvent : OperationEvent
    {
        new IEnumerable<TEvent> Events { get; }
    }

    public interface IErrorsHandler<TInput, TState> : IEventHandler<TInput, TState>
    {
        IEnumerable<OperationEvent> Errors { get; }
    }

    public interface IErrorsHandler<TError, TInput, TState> : IErrorsHandler<TInput, TState>
        where TError : OperationEvent
    {
        new IEnumerable<TError> Errors { get; }
    }

    public interface IErrorsHandler<TError, TInput, TState, Tin> : IEventHandlerWithInput<TInput,TState, Tin>
        where TError : OperationEvent
    {
        IEnumerable<TError> Errors { get; }
    }

    public interface IExceptionsErrorHandler<TError,TException, TInput, TState> : IEventHandler<TInput,TState>
        where TException : Exception
        where TError : OperationEvent
    {
        IEnumerable<IOperationExceptionError<TError, TException>> ExceptionErrors { get; }
    }

    public interface IExceptionsErrorHandler<TError, TException, TInput, TState, Tin> : IEventHandlerWithInput<TInput, TState, Tin>
        where TException : Exception
        where TError : OperationEvent
    {
        IEnumerable<IOperationExceptionError<TError, TException>> ExceptionErrors { get; }
    }
}
