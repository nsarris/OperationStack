using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TState">Type of the state of this operation stack</typeparam>
    /// <typeparam name="TState">Type of the event of this operation stack</typeparam>
    public class OperationStack<TInput, TState, TOperationEvent> : ICommandOperation<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        #region Fields and Props
        public OperationStackOptions Options => internalStack.Options;

        bool IOperation.SupportsSync => internalStack.Options.SupportsSync;
        bool IOperation.SupportsAsync => internalStack.Options.SupportsAsync;
        bool IOperation.PreferAsync => internalStack.Options.PreferAsync;

        private OperationStackInternal<TInput, TState, TOperationEvent> internalStack;

        private StackBlockSpecBuilder<TInput, TState, TOperationEvent> blockSpecBuilder = new StackBlockSpecBuilder<TInput, TState, TOperationEvent>();
        #endregion Fields and Props

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        //public OperationStack()
        //{

        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">Override the default options</param>
        internal OperationStack(OperationStackOptions options, Func<TState> initialStateBuilder, bool hasInput)
        {
            internalStack = new OperationStackInternal<TInput, TState, TOperationEvent>(options, initialStateBuilder, hasInput);
        }

        internal OperationStack(IEnumerable<StackBlockSpecBase<TInput, TState, TOperationEvent>> blocks, OperationStackOptions options, Func<TState> initialStateBuilder, bool hasInput)
        {
            internalStack = new OperationStackInternal<TInput, TState, TOperationEvent>(options, initialStateBuilder, hasInput, new StackBlocks<TInput, TState, TOperationEvent>(blocks));
        }

        #endregion Ctor

        #region Sync

        /// <summary>
        /// Add a void block of code to be executed in the stack
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="op">A handle to control the flow of the code</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent> Then(Func<ICommand<TInput, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent> Then(Action<ICommand<TInput, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// Add a block of code which returns a value of <see cref="T"/> 
        /// </summary>
        /// <remarks>
        /// A query operation is a function that returns a value
        /// </remarks>
        /// <param name="op">The query to be executed</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturn<T>(Func<IQuery<TInput, TState, TOperationEvent>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturnOf<T>(Func<ITypedQuery<TInput, TState, TOperationEvent, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command stack to append</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TInput, TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query stack to append</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(Func<IOperationBlock<TInput, TState, TOperationEvent>, IQueryResult<TOperationEvent, T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, (op) => res));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(IQueryResult<TOperationEvent, T> res)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(ICommandOperation<TOperationEvent> operation)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, operation));
        }

        //public OperationStack<TInput, TState, TOperationEvent> ThenAppend(ICommandOperation<TInput,TState, TOperationEvent> operation)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, operation));
        //}

        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(IQueryOperation<TOperationEvent, T> operation)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, operation));
        }

        //public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(IQueryOperation<TInput,TState, TOperationEvent, T> operation)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, operation));
        //}

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="tag">Mark the block with a tag for reference</param>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        /// /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="tag">Mark the block with a tag for reference</param>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        /// /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Action<ICommand<TInput, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        /// <summary>
        /// Add a query operation
        /// </summary>
        /// <remarks>
        /// A query operation is a function that returns a value
        /// </remarks>
        /// <param name="op">The query to be executed</param>
        /// <param name="tag">Mark the block with a tag for reference</param>
        /// <returns></returns>
        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturn<T>(string tag, Func<IQuery<TInput, TState, TOperationEvent>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TInput, TState, TOperationEvent, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, IQueryResult<TOperationEvent, T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(string tag, IQueryResult<TOperationEvent, T> res)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, ICommandOperation<TOperationEvent> operation)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, operation));
        }
          
        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(string tag, IQueryOperation<TOperationEvent, T> operation)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, operation));
        }
              
        public OperationStack<TInput, TState, TOperationEvent> Finally(Func<ICommand<TInput, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Finally(Action<ICommand<TInput, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> FinallyReturn<T>(Func<IQuery<TInput, TState, TOperationEvent>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> FinallyReturnOf<T>(Func<ITypedQuery<TInput, TState, TOperationEvent, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Sync

        #region Async

        public OperationStack<TInput, TState, TOperationEvent> Then(Func<ICommand<TInput, TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Then(Func<ICommand<TInput, TState, TOperationEvent>, Task> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturn<T>(Func<IQuery<TInput, TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturnOf<T>(Func<ITypedQuery<TInput, TState, TOperationEvent, T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }



        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent>, Task> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturn<T>(string tag, Func<IQuery<TInput, TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TInput, TState, TOperationEvent, T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> ThenAppend<T>(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }




        public OperationStack<TInput, TState, TOperationEvent> Finally(Func<ICommand<TInput, TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> FinallyReturn<T>(Func<IQuery<TInput, TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> FinallyReturnOf<T>(Func<ITypedQuery<TInput, TState, TOperationEvent, T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Async

        #region OnEvents / Catch

        public OperationStack<TInput, TState, TOperationEvent> OnEvents(Func<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent>, BlockResultVoid> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrors(Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> Catch(Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptions(Func<IExceptionsErrorHandler<TOperationEvent,Exception, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, x => x.Error.IsException && !x.Error.IsHandled));
        //}

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent,TException, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, x => x.Error.IsException && !x.Error.IsHandled));
        //}



        public OperationStack<TInput, TState, TOperationEvent> OnEventsWhere(Func<TOperationEvent, bool> filter, Func<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent>, BlockResultVoid> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrorsWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TInput, TState, TOperationEvent>, BlockResultVoid> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}



        public OperationStack<TInput, TState, TOperationEvent> OnEvents(Action<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrors(Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptions(Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> Catch(Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptions(Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TInput, TState, TOperationEvent>> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler));
        //}

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TInput, TState, TOperationEvent>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler));
        //}



        public OperationStack<TInput, TState, TOperationEvent> OnEventsWhere(Func<TOperationEvent, bool> filter, Action<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrorsWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TInput, TState, TOperationEvent>> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TInput, TState, TOperationEvent> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,TException, TInput, TState, TOperationEvent>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}

        #endregion

        #region Execute
        
        public ICommandResult<TInput, TState, TOperationEvent> Execute(TInput input, TState initialState)
        {
            return internalStack.Execute(input, initialState);
        }

        public Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync(TInput input, TState initialState)
        {
            return internalStack.ExecuteAsync(input, initialState);
        }

        public ICommandResult<TInput, TState, TOperationEvent> Execute(TInput input)
        {
            return internalStack.Execute(input);
        }

        public Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync(TInput input)
        {
            return internalStack.ExecuteAsync(input);
        }

        public ICommandResult<TInput, TState, TOperationEvent> Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput), initialState);
        }

        public Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync(TState initialState)
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput), initialState);
        }

        public ICommandResult<TInput, TState, TOperationEvent> Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        public Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync()
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput));
        }

        ICommandResult<TOperationEvent> ICommandOperation<TOperationEvent>.Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        async Task<ICommandResult<TOperationEvent>> ICommandOperation<TOperationEvent>.ExecuteAsync()
        {
            internalStack.AssertInput();
            return await this.ExecuteAsync(default(TInput)).ConfigureAwait(false);
        }

        ICommandResult<TOperationEvent> ICommandOperationWithInput<TInput, TOperationEvent>.Execute(TInput input)
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        async Task<ICommandResult<TOperationEvent>> ICommandOperationWithInput<TInput, TOperationEvent>.ExecuteAsync(TInput input)
        {
            return await this.ExecuteAsync(input).ConfigureAwait(false);
        }

        ICommandResult<TOperationEvent> ICommandOperationWithState<TState, TOperationEvent>.Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput), initialState);
        }

        async Task<ICommandResult<TOperationEvent>> ICommandOperationWithState<TState, TOperationEvent>.ExecuteAsync(TState initialState)
        {
            internalStack.AssertInput();
            return await this.ExecuteAsync(default(TInput), initialState).ConfigureAwait(false);
        }



        #endregion Result
    }


    public class OperationStack<TInput, TState, TOperationEvent, T> : IQueryOperation<TInput, TState, TOperationEvent, T>
        where TOperationEvent : OperationEvent
    {
        #region Fields and Props

        public OperationStackOptions Options => internalStack.Options;
        bool IOperation.SupportsSync => internalStack.Options.SupportsSync;
        bool IOperation.SupportsAsync => internalStack.Options.SupportsAsync;
        bool IOperation.PreferAsync => internalStack.Options.PreferAsync;

        private OperationStackInternal<TInput, TState, TOperationEvent> internalStack;
        private StackBlockSpecBuilder<TInput, TState, TOperationEvent> blockSpecBuilder = new StackBlockSpecBuilder<TInput, TState, TOperationEvent>();

        #endregion Fields and Props

        #region Ctor

        internal OperationStack(IEnumerable<StackBlockSpecBase<TInput, TState, TOperationEvent>> blocks, OperationStackOptions options, Func<TState> initalStateBuilder, bool hasInput)
        {
            internalStack = new OperationStackInternal<TInput, TState, TOperationEvent>(options, initalStateBuilder, hasInput, new StackBlocks<TInput, TState, TOperationEvent>(blocks));
        }

        #endregion Ctor

        #region Sync

        public OperationStack<TInput, TState, TOperationEvent> Then(Func<ICommand<TInput, TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Then(Action<ICommand<TInput, TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturn<Tout>(Func<IQuery<TInput, TState, TOperationEvent, T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TInput, TState, TOperationEvent, T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TInput, TState, TOperationEvent, T>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(Func<IStackBlock<TInput, TState, TOperationEvent, T>, IQueryResult<TOperationEvent, Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(IQueryResult<TOperationEvent, Tout> res)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(ICommandOperation<TOperationEvent> operation)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand<T>(null, internalStack.NextIndex, operation));
        }

     
        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(IQueryOperation<TOperationEvent, Tout> operation)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery<T,Tout>(null, internalStack.NextIndex, operation));
        }

     
        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Action<ICommand<TInput, TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TInput, TState, TOperationEvent, T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TInput, TState, TOperationEvent, T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TInput, TState, TOperationEvent, T>, IQueryResult<TOperationEvent, Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, IQueryResult<TOperationEvent, Tout> res)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, ICommandOperation<TOperationEvent> operation)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand<T>(tag, internalStack.NextIndex, operation));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, IQueryOperation<TOperationEvent, Tout> operation)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery<T, Tout>(tag, internalStack.NextIndex, operation));
        }

        public OperationStack<TInput, TState, TOperationEvent> Finally(Func<ICommand<TInput, TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Finally(Action<ICommand<TInput, TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> FinallyReturn<Tout>(Func<IQuery<TInput, TState, TOperationEvent, T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TInput, TState, TOperationEvent, T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Sync

        #region Async

        public OperationStack<TInput, TState, TOperationEvent> Then(Func<ICommand<TInput, TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Then(Func<ICommand<TInput, TState, TOperationEvent, T>, Task> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }



        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturn<Tout>(Func<IQuery<TInput, TState, TOperationEvent, T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TInput, TState, TOperationEvent, T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(Func<IStackBlock<TInput, TState, TOperationEvent, T>, Task<IQueryResult<TOperationEvent, Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }



        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent, T>, Task> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TInput, TState, TOperationEvent, T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TInput, TState, TOperationEvent, T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TInput, TState, TOperationEvent, T>, Task<IQueryResult<TOperationEvent, Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }


        public OperationStack<TInput, TState, TOperationEvent> Finally(Func<ICommand<TInput, TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> FinallyReturn<Tout>(Func<IQuery<TInput, TState, TOperationEvent, T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TInput, TState, TOperationEvent, T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Sync

        #region OnEvents / Catch


        public OperationStack<TInput, TState, TOperationEvent, T> OnEvents(Func<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrors(Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> Catch(Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}






        public OperationStack<TInput, TState, TOperationEvent, T> OnEventsWhere(Func<TOperationEvent, bool> filter, Func<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrorsWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TInput, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TInput, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}



        public OperationStack<TInput, TState, TOperationEvent, T> OnEvents(Action<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent, T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrors(Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptions(Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> Catch(Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptions(Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TInput, TState, TOperationEvent,T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TInput, TState, TOperationEvent,T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}





        public OperationStack<TInput, TState, TOperationEvent, T> OnEventsWhere(Func<TOperationEvent, bool> filter, Action<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent, T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrorsWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, T> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TInput, TState, TOperationEvent,T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TInput, TState, TOperationEvent,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}

        #endregion

        #region Execute

        public IQueryResult<TInput, TState, TOperationEvent, T> Execute(TInput input, TState initialState)
        {
            return internalStack.Execute<T>(input, initialState);
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, T>> ExecuteAsync(TInput input, TState initialState)
        {
            return internalStack.ExecuteAsync<T>(input, initialState);
        }

        public IQueryResult<TInput, TState, TOperationEvent, T> Execute(TInput input)
        {
            return internalStack.Execute<T>(input);
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, T>> ExecuteAsync(TInput input)
        {
            return internalStack.ExecuteAsync<T>(input);
        }

        public IQueryResult<TInput, TState, TOperationEvent, T> Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput), initialState);
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, T>> ExecuteAsync(TState initialState)
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput), initialState);
        }

        public IQueryResult<TInput, TState, TOperationEvent, T> Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, T>> ExecuteAsync()
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput));
        }

        IQueryResult<TOperationEvent, T> IQueryOperation<TOperationEvent, T>.Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        async Task<IQueryResult<TOperationEvent, T>> IQueryOperation<TOperationEvent, T>.ExecuteAsync()
        {
            internalStack.AssertInput();
            return await this.ExecuteAsync(default(TInput)).ConfigureAwait(false);
        }

        IQueryResult<TOperationEvent, T> IQueryOperationWithInput<TInput, TOperationEvent, T>.Execute(TInput input)
        {
            return this.Execute(input);
        }

        async Task<IQueryResult<TOperationEvent, T>> IQueryOperationWithInput<TInput, TOperationEvent, T>.ExecuteAsync(TInput input)
        {
            return await this.ExecuteAsync(input).ConfigureAwait(false);
        }

        IQueryResult<TOperationEvent, T> IQueryOperationWithState<TState, TOperationEvent, T>.Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(initialState);
        }

        async Task<IQueryResult<TOperationEvent, T>> IQueryOperationWithState<TState, TOperationEvent, T>.ExecuteAsync(TState initialState)
        {
            return await this.ExecuteAsync(initialState).ConfigureAwait(false);
        }

        #endregion Result
    }

    //public class OperationStack : OperationStack<object, object, OperationEvent>
    //{
    //    public OperationStack()
    //        : base()
    //    {

    //    }
    //    public OperationStack(OperationStackOptions options)
    //        : base(options)
    //    {

    //    }
    //}

    //public class OperationStack<TOperationEvent> : OperationStack<object, object, TOperationEvent>
    //    where TOperationEvent : OperationEvent
    //{
    //    public OperationStack()
    //        : base()
    //    {

    //    }
    //    public OperationStack(OperationStackOptions options)
    //        : base(options)
    //    {

    //    }
    //}
}
