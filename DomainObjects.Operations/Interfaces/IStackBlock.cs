using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public interface IStackBlock<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        IStackEvents<TOperationEvent> StackEvents { get; }
        void Throw(TOperationEvent error);
        string Tag { get; }
    }

    public interface IStackBlock<TInput, TState, TOperationEvent> : IStackBlock<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        TState StackState { get; set; }
        TInput StackInput { get; }
    }

    public interface IStackBlock<TInput, TState, TOperationEvent, T> : IStackBlock<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        Emptyable<T> Input { get; }
    }

    public interface IOperationBlock<TInput, TState, TOperationEvent> : IStackBlock<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        IOperationEvents<TOperationEvent> Events { get; }
        void Append(IOperationResult<TOperationEvent> res);
    }

    public interface IOperationBlock<TInput, TState, TOperationEvent, Tin> : IOperationBlock<TInput, TState, TOperationEvent>, IStackBlock<TInput, TState, TOperationEvent,Tin>
        where TOperationEvent : OperationEvent
    {

    }

    public interface IQuery<TInput, TState, TOperationEvent> : IOperationBlock<TInput, TState, TOperationEvent>, IResultDispatcher<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        IQueryResultProxy<T,TState, TOperationEvent> DefineResult<T>();
        IQueryResultProxy<T, TState, TOperationEvent> DefineResult<T>(T result);
        IQueryResultProxy<T, TState, TOperationEvent> DefineResult<T>(Expression<Func<T>> expression);
    }

    public interface ICommand<TInput, TState, TOperationEvent> : IOperationBlock<TInput, TState, TOperationEvent>, IResultVoidDispatcher<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {


    }




    

    public interface ICommand<TInput, TState, TOperationEvent, T> : ICommand<TInput, TState, TOperationEvent>, IOperationBlock<TInput, TState, TOperationEvent, T>
        where TOperationEvent : OperationEvent
    {

    }

    public interface IQuery<TInput, TState, TOperationEvent, T> : IQuery<TInput, TState, TOperationEvent>, IOperationBlock<TInput, TState, TOperationEvent, T>
        where TOperationEvent : OperationEvent
    {

    }



    public interface ITypedQuery<TInput, TState, TOperationEvent, T> : IOperationBlock<TInput, TState, TOperationEvent>, IResultDispatcher<TState, TOperationEvent>, IResultDispatcher<T,TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {

    }

    public interface ITypedQuery<TInput, TState, TOperationEvent, Tin, Tout> : ITypedQuery<TInput, TState, TOperationEvent, Tout>, IOperationBlock<TInput, TState, TOperationEvent, Tin>
        where TOperationEvent : OperationEvent
    {

    }



    public interface IEventHandler<TInput, TState, TOperationEvent> : IStackBlock<TInput, TState, TOperationEvent>, IResultVoidDispatcher<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {

    }

    public interface IEventHandlerWithInput<TInput, TState, TOperationEvent, T> : IStackBlock<TInput, TState, TOperationEvent, T>, IResultDispatcher<T,TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        Emptyable<T> Result { get; }
        BlockResult<T> Return();
    }

   


    public interface IEventsHandler<TEvent, TInput, TState, TOperationEvent> : IEventHandler<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
        where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> Events { get; }
    }

    public interface IEventsHandler<TEvent, TInput, TState, TOperationEvent, Tin> : IEventHandlerWithInput<TInput, TState, TOperationEvent, Tin>
        where TOperationEvent : OperationEvent
        where TEvent : TOperationEvent
    {
        IEnumerable<TEvent> Events { get; }
    }

    public interface IErrorsHandler<TError, TInput, TState, TOperationEvent> : IEventHandler<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<TError> Errors { get; }
    }

    public interface IErrorsHandler<TError, TInput, TState, TOperationEvent, Tin> : IEventHandlerWithInput<TInput,TState, TOperationEvent, Tin>
        where TOperationEvent : OperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<TError> Errors { get; }
    }

    public interface IExceptionsErrorHandler<TError,TException, TInput, TState, TOperationEvent> : IEventHandler<TInput,TState, TOperationEvent>
        where TException : Exception
        where TOperationEvent : OperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TError, TException>> ExceptionErrors { get; }
    }

    public interface IExceptionsErrorHandler<TError, TException, TInput, TState, TOperationEvent, Tin> : IEventHandlerWithInput<TInput, TState, TOperationEvent, Tin>
        where TException : Exception
        where TOperationEvent : OperationEvent
        where TError : TOperationEvent
    {
        IEnumerable<IOperationExceptionError<TError, TException>> ExceptionErrors { get; }
    }
}
