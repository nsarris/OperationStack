using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface IResult
    {

    }
    public class ResultVoid : IResult
    {

    }

    public class Result<T> : IResult
    {
        
    }
  

    public interface IResultDispatcher<T>
    {
        Result<T> Return();
        Result<T> Return(T result);
        Result<T> End();
        Result<T> Break();
        Result<T> Reset();
        Result<T> Reset(object state);
        Result<T> Restart();
        Result<T> Goto(string tag);
        Result<T> Goto(string tag, object overrideInput);
        Result<T> Skip(int i);
        Result<T> Skip(int i, object overrideInput);
    }

    public interface IResultVoidDispatcher
    {
        ResultVoid Return();
        ResultVoid End();
        ResultVoid Break();
        ResultVoid Reset();
        ResultVoid Reset(object state);
        ResultVoid Restart();
        ResultVoid Goto(string tag);
        ResultVoid Goto(string tag, object overrideInput);
        ResultVoid Skip(int i);
        ResultVoid Skip(int i, object overrideInput);
    }

    public interface IResultDispatcher
    {
        Result<T> Return<T>();
        Result<T> Return<T>(T result);
        Result<T> End<T>();
        Result<T> Break<T>();
        Result<T> Reset<T>();
        Result<T> Reset<T>(object state);
        Result<T> Restart<T>();
        Result<T> Goto<T>(string tag);
        Result<T> Goto<T>(string tag, object overrideInput);
        Result<T> Skip<T>(int i);
        Result<T> Skip<T>(int i, object overrideInput);

    }

    public interface IStackEvents : IEnumerable<IOperationEvent>
    {

    }

    public interface IOperationEvents : IEnumerable<IOperationEvent>
    {

    }

    public interface IStackBlock<TState>
    {
        IStackEvents StackEvents { get; }
        TState StackState { get; }
    }

    public interface IOperationBlock<TState> : IStackBlock<TState>
    {
        List<IOperationEvent> Events { get; }
    }

    public interface IQuery<TState> : IOperationBlock<TState>, IResultDispatcher
    {
        IOperationResultProxy<T> DefineResult<T>();
        IOperationResultProxy<T> DefineResult<T>(T result);
        IOperationResultProxy<T> DefineResult<T>(Expression<Func<T>> expression);
    }

    public interface ICommand<TState> : IOperationBlock<TState>, IResultVoidDispatcher
    {


    }




    public interface IStackBlock<TState, T> : IOperationBlock<TState>
    {
        T Input { get; }
    }

    public interface ICommand<TState, T> : ICommand<TState>, IStackBlock<TState, T>
    {

    }

    public interface IQuery<TState, T> : IQuery<TState>, IStackBlock<TState, T>
    {

    }



    public interface ITypedQuery<TState, T> : IOperationBlock<TState>, IResultDispatcher<T>
    {

    }

    public interface ITypedQuery<TState, Tin, Tout> : ITypedQuery<TState, Tout>, IOperationBlock<TState>, IStackBlock<TState, Tin>
    {

    }





    public interface IEventHandler<TEvent, TState> : IStackBlock<TState>, IResultVoidDispatcher
        where TEvent : IOperationEvent
    {
        TEvent Event { get; }
    }



    public interface IEventHandler<TEvent, TState, Tin> : IStackBlock<TState, Tin>, IResultDispatcher<Tin>
        where TEvent : IOperationEvent
    {
        TEvent Event { get; }
    }

    public interface IErrorHandler<TError, TState> : IStackBlock<TState>, IResultVoidDispatcher
        where TError : IOperationError
    {
        TError Error { get; }
    }

    public interface IErrorHandler<TError, TState, Tin> : IStackBlock<TState, Tin>, IResultVoidDispatcher
        where TError : IOperationError
    {
        TError Error { get; }
    }

    public interface IExceptionErrorHandler<TException, TState> : IStackBlock<TState>, IResultVoidDispatcher
        where TException : Exception
    {
        IOperationExceptionError<TException> Error { get; }
    }

    public interface IExceptionErrorHandler<TException, TState, Tin> : IStackBlock<TState, Tin>, IResultVoidDispatcher
        where TException : Exception
    {
        IOperationExceptionError<TException> Error { get; }
    }




    public interface IEventsHandler<TEvent, TState> : IOperationBlock<TState>, IResultVoidDispatcher
        where TEvent : IOperationEvent
    {
        IEnumerable<TEvent> Event { get; }
    }

    public interface IEventsHandler<TEvent, TState, Tin> : IStackBlock<TState, Tin>, IResultDispatcher<Tin>
        where TEvent : IOperationEvent
    {
        IEnumerable<TEvent> Event { get; }
    }

    public interface IErrorsHandler<TError, TState> : IStackBlock<TState>, IResultVoidDispatcher
        where TError : IOperationError
    {
        IEnumerable<TError> Error { get; }
    }

    public interface IErrorsHandler<TError, TState, Tin> : IStackBlock<TState, Tin>, IResultVoidDispatcher
        where TError : IOperationError
    {
        IEnumerable<TError> Error { get; }
    }

    public interface IExceptionsErrorHandler<TException, TState> : IStackBlock<TState>, IResultVoidDispatcher
        where TException : Exception
    {
        IEnumerable<IOperationExceptionError<TException>> Error { get; }
    }

    public interface IExceptionsErrorHandler<TException, TState, Tin> : IStackBlock<TState, Tin>, IResultVoidDispatcher
        where TException : Exception
    {
        IEnumerable<IOperationExceptionError<TException>> Error { get; }
    }


    public interface IOperationResultProxy<T> : IResultDispatcher<T>
    {
        T Result { get; set; }
    }

    
    public interface IOperationResult
    {
        List<string> Events { get; }
    }

    public interface ICommandResult : IOperationResult
    {

    }

    public interface IQueryResult<T> : IOperationResult
    {
        T Result { get; }
    }

    public interface IOperationEvent
    {
        bool Handled { get; }
    }

    public interface IOperationError
    {
        Exception Exception { get; }
    }

    public interface IOperationExceptionError<TException> : IOperationError
        where TException : Exception
    {
        new TException Exception { get; }
    }



    public class StackTest<TState>
    {
        private TState state;

        public StackTest(TState state)
        {
            this.state = state;
        }

        public StackTest<TState> Then(Func<ICommand<TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> Then(Action<ICommand<TState>> res)
        {
            return null;
        }

        

        public StackTest<TState, T> ThenReturn<T>(Func<IQuery<TState>, Result<T>> res)
        {
            return null;
        }

        public StackTest<TState, T> ThenReturnOf<T>(Func<ITypedQuery<TState, T>, Result<T>> res)
        {
            return null;
        }

        public StackTest<TState> ThenAppend(Func<IOperationBlock<TState>, ICommandResult> res)
        {
            return null;
        }

        public StackTest<TState, T> ThenAppend<T>(Func<IOperationBlock<TState>, IQueryResult<T>> res)
        {
            return null;
        }



        public StackTest<TState> Then(string tag, Func<ICommand<TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> Then(string tag, Action<ICommand<TState>> res)
        {
            return null;
        }

        public StackTest<TState, T> ThenReturn<T>(string tag, Func<IQuery<TState>, Result<T>> res)
        {
            return null;
        }

        public StackTest<TState, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TState, T>, Result<T>> res)
        {
            return null;
        }

        public StackTest<TState> ThenAppend(string tag, Func<IOperationBlock<TState>, ICommandResult> res)
        {
            return null;
        }

        public StackTest<TState, T> ThenAppend<T>(string tag, Func<IOperationBlock<TState>, IQueryResult<T>> res)
        {
            return null;
        }



        public StackTest<TState> Finally(Func<ICommand<TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState, T> FinallyReturn<T>(Func<IQuery<TState>, Result<T>> res)
        {
            return null;
        }

        public StackTest<TState, T> FianllyReturnOf<T>(Func<ITypedQuery<TState, T>, Result<T>> res)
        {
            return null;
        }





        public StackTest<TState> OnEvents(Func<IEventsHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEvent(Func<IEventHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOf<TEvent>(Func<IEventHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnError(Func<IErrorHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrors(Func<IErrorsHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOf<TError>(Func<IErrorHandler<TError, TState>, ResultVoid> rhandleres)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState>, ResultVoid> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnException(Func<IErrorHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptions(Func<IErrorsHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOf<TException>(Func<IExceptionErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledException(Func<IErrorHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptions(Func<IErrorsHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOf<TException>(Func<IExceptionErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOf<TException>(Func<IExceptionErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }



        public StackTest<TState> OnEventsWhere(Func<IOperationEvent, bool> filter, Func<IEventsHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEventWhere(Func<IOperationEvent, bool> filter, Func<IEventHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }
        public StackTest<TState> OnErrorWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOfWhere<TError>(Func<TError, bool> filter, Func<IErrorHandler<TError, TState>, ResultVoid> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState>, ResultVoid> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnExceptionWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionsErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }



        public StackTest<TState> OnEvents(Action<IEventsHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEvent(Action<IEventHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOf<TEvent>(Action<IEventHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnError(Action<IErrorHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrors(Action<IErrorsHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOf<TError>(Action<IErrorHandler<TError, TState>> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState>> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnException(Action<IErrorHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptions(Action<IErrorsHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOf<TException>(Action<IExceptionErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledException(Action<IErrorHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptions(Action<IErrorsHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOf<TException>(Action<IExceptionErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOf<TException>(Action<IExceptionErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }



        public StackTest<TState> OnEventsWhere(Func<IOperationEvent, bool> filter, Action<IEventsHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEventWhere(Func<IOperationEvent, bool> filter, Action<IEventHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }
        public StackTest<TState> OnErrorWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOfWhere<TError>(Func<TError, bool> filter, Func<IErrorHandler<TError, TState>> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState>> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnExceptionWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOfWhere<TException>(Func<TException, bool> filter, Func<IExceptionErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOfWhere<TException>(Func<TException, bool> filter, Func<IExceptionsErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return null;
        }




        public ICommandResult ToResult()
        {
            return null;
        }
    }



    public class StackTest<TState, T>
    {
        private TState state;

        public StackTest(TState state)
        {
            this.state = state;
        }
        public StackTest<TState> Then(Func<ICommand<TState, T>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> Then(Action<ICommand<TState, T>> res)
        {
            return null;
        }



        public StackTest<TState, Tout> ThenReturn<Tout>(Func<IQuery<TState, T>, Result<Tout>> res)
        {
            return null;
        }

        public StackTest<TState, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TState, T, Tout>, Result<Tout>> res)
        {
            return null;
        }

        public StackTest<TState> ThenAppend(Func<IOperationBlock<TState>, ICommandResult> res)
        {
            return null;
        }

        public StackTest<TState, Tout> ThenAppend<Tout>(Func<IStackBlock<TState, T>, IQueryResult<Tout>> res)
        {
            return null;
        }



        public StackTest<TState> Then(string tag, Func<ICommand<TState, T>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> Then(string tag, Action<ICommand<TState, T>> res)
        {
            return null;
        }

        public StackTest<TState, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TState, T>, Result<Tout>> res)
        {
            return null;
        }

        public StackTest<TState, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TState, T, Tout>, Result<Tout>> res)
        {
            return null;
        }

        public StackTest<TState> ThenAppend(string tag, Func<IOperationBlock<TState>, ICommandResult> res)
        {
            return null;
        }

        public StackTest<TState, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TState, T>, IQueryResult<Tout>> res)
        {
            return null;
        }


        public StackTest<TState> Finally(Func<ICommand<TState, T>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState, Tout> FinallyReturn<Tout>(Func<IQuery<TState, T>, Result<Tout>> res)
        {
            return null;
        }

        public StackTest<TState, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TState, T, Tout>, Result<Tout>> res)
        {
            return null;
        }





        public StackTest<TState> OnEvents(Func<IEventsHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEvent(Func<IEventHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOf<TEvent>(Func<IEventHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnError(Func<IErrorHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrors(Func<IErrorsHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOf<TError>(Func<IErrorHandler<TError, TState, T>, ResultVoid> rhandleres)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState, T>, ResultVoid> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnException(Func<IErrorHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptions(Func<IErrorsHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOf<TException>(Func<IExceptionErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledException(Func<IErrorHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptions(Func<IErrorsHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOf<TException>(Func<IExceptionErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOf<TException>(Func<IExceptionErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }






        public StackTest<TState> OnEventsWhere(Func<IOperationEvent, bool> filter, Func<IEventsHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEventWhere(Func<IOperationEvent, bool> filter, Func<IEventHandler<IOperationEvent, TState>, ResultVoid> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventHandler<TEvent, TState>, ResultVoid> res)
            where TEvent : IOperationEvent
        {
            return null;
        }
        public StackTest<TState> OnErrorWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOfWhere<TError>(Func<TError, bool> filter, Func<IErrorHandler<TError, TState, T>, ResultVoid> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, T>, ResultVoid> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnExceptionWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionsErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionWhere(Func<IOperationError, bool> filter, Func<IErrorHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState, T>, ResultVoid> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionErrorHandler<TException, TState, T>, ResultVoid> handler)
            where TException : Exception
        {
            return null;
        }



        public StackTest<TState> OnEvents(Action<IEventsHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEvent(Action<IEventHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOf<TEvent>(Action<IEventHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnError(Action<IErrorHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrors(Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOf<TError>(Action<IErrorHandler<TError, TState, T>> rhandleres)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState, T>> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnException(Action<IErrorHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptions(Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOf<TException>(Action<IExceptionErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledException(Action<IErrorHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptions(Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOf<TException>(Action<IExceptionErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOf<TException>(Action<IExceptionErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }





        public StackTest<TState> OnEventsWhere(Func<IOperationEvent, bool> filter, Action<IEventsHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEventWhere(Func<IOperationEvent, bool> filter, Action<IEventHandler<IOperationEvent, TState>> res)
        {
            return null;
        }

        public StackTest<TState> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnEventOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventHandler<TEvent, TState>> res)
            where TEvent : IOperationEvent
        {
            return null;
        }

        public StackTest<TState> OnErrorWhere(Func<IOperationError, bool> filter, Action<IErrorHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorsWhere(Func<IOperationError, bool> filter, Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnErrorOfWhere<TError>(Func<TError, bool> filter, Action<IErrorHandler<TError, TState, T>> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, T>> handler)
            where TError : IOperationError
        {
            return null;
        }

        public StackTest<TState> OnExceptionWhere(Func<IOperationError, bool> filter, Action<IErrorHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionsWhere(Func<IOperationError, bool> filter, Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnExceptionOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionsErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionWhere(Func<IOperationError, bool> filter, Action<IErrorHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsWhere(Func<IOperationError, bool> filter, Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }

        public StackTest<TState> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return null;
        }


        public IQueryResult<T> ToResult()
        {
            return null;
        }
    }

    public class StackTest : StackTest<object>
    {
        public StackTest()
            : base(null)
        {

        }

        public StackTest(object state)
            : base(state)
        {

        }
    }

    public interface IMockService<T>
    {
        T Get();
    }

    public interface IStackTestState
    {
        int Id { get; set; }
        bool First { get; set; }
    }

    public interface ISomeError : IOperationError
    {
    }


    public class Test
    {
        public void Foo()
        {
            IMockService<List<string>> service = null;
            var s = new StackTest<IStackTestState>(null)
                .Then(async op =>
                {
                    await Task.Delay(1000);
                    op.Return();
                })
                .ThenReturn(op =>
                {
                    return op.Return(2);
                })
                .OnErrors(op =>
                {
                    return op.Return();
                })
                .OnErrorsWhere(x => x.Exception != null,
                op =>
                {
                    return op.Return();
                })
                .OnEventOf<IOperationEvent>(op =>
                {
                    return op.Return();
                })
                .OnEventsOf<IOperationEvent>(op =>
                {
                    //return op.Return();
                })
                .Then("SomeStep", (op) =>
                {
                    op.StackState.Id = 0;
                    return op.Return();
                })


                .ThenReturn((op) =>
                {
                    var r = op.DefineResult<int>();
                    return r.Return(2);
                })
                .ThenReturn((op) =>
                {
                    return op.Return(2);
                })
                .ThenReturnOf<int>(op =>
                {
                    return op.End();
                })
                .OnError(h =>
                {
                    //return h.Reset();
                })
                .ThenAppend(op =>
                {
                    return (IQueryResult<int>)null;
                })
                //.OnError(op =>
                //{
                //    return op.End();
                //})
                .ThenReturn((op) =>
                {
                    //var rd = op.DefineResult(() => service.Get());
                    //var res1 = op.DefineResult(new List<string>());
                    //var rd = op.DefineResult(() => new { Name = "" });

                    if (op == null)
                    {
                        //var res = r.GetResult(null);
                        //rd.Result.Name = "Kostas";
                        //rd.Result = new { Name = "Kostas" };
                        //return rd.Result;
                        //return rd.Return(new { Name = "Kostas" });
                        return op.Return(5);
                    }
                    else
                        return op.End<int>();
                    //return r.End();
                })
                .ToResult();


        }
    }
}
