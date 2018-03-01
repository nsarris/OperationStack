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
    public class OperationStack<TState, TOperationEvent> : ICommandOperation<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        #region Fields and Props
        public OperationStackOptions Options => internalStack.Options;

        bool IOperation.SupportsSync => internalStack.Options.SupportsSync;
        bool IOperation.SupportsAsync => internalStack.Options.SupportsAsync;
        bool IOperation.PreferAsync => internalStack.Options.PreferAsync;

        private OperationStackInternal<TState, TOperationEvent> internalStack = new OperationStackInternal<TState, TOperationEvent>();

        private StackBlockSpecBuilder<TState,TOperationEvent> blockSpecBuilder = new StackBlockSpecBuilder<TState, TOperationEvent>();
        #endregion Fields and Props

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        public OperationStack()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">Override the default options</param>
        public OperationStack(OperationStackOptions options)
        {
            internalStack.Options = options;
        }

        internal OperationStack(IEnumerable<StackBlockSpecBase<TState,TOperationEvent>> blocks, OperationStackOptions options)
            : this(options)
        {
            internalStack.Blocks = new StackBlocks<TState,TOperationEvent>(blocks);
        }

        #endregion Ctor

        #region Sync

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent>, BlockResultVoid> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
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
        public OperationStack<TState, TOperationEvent> Then(Action<ICommand<TState, TOperationEvent>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// Add a query operation
        /// </summary>
        /// <remarks>
        /// A query operation is a function that returns a value
        /// </remarks>
        /// <param name="op">The query to be executed</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(Func<IQuery<TState, TOperationEvent>, BlockResult<T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command stack to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query stack to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent,T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent> ThenAppend(ICommandResult<TOperationEvent> res)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, (op) => res));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(IQueryResult<TOperationEvent,T> res)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, (op) => res));
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
        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent>, BlockResultVoid> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
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
        public OperationStack<TState, TOperationEvent> Then(string tag, Action<ICommand<TState, TOperationEvent>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
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
        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(string tag, Func<IQuery<TState, TOperationEvent>, BlockResult<T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent,T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, ICommandResult<TOperationEvent> res)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, IQueryResult<TOperationEvent,T> res)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, ICommandOperation<TOperationEvent> operation)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, ICommandOperation<TState, TOperationEvent> operation)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, IQueryOperation<TOperationEvent, T> operation)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, IQueryOperation<TState, TOperationEvent, T> operation)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, operation));
        }


        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent>, BlockResultVoid> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Finally(Action<ICommand<TState, TOperationEvent>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturn<T>(Func<IQuery<TState, TOperationEvent>, BlockResult<T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Sync

        #region Async

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent>, Task> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(Func<IQuery<TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, Task<BlockResult<T>>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent,T>>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }



        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent>, Task> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(string tag, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TState, TOperationEvent,T>, Task<BlockResult<T>>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent,T>>> op)
        {
            //return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        


        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturn<T>(Func<IQuery<TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Async

        #region OnEvents / Catch

        public OperationStack<TState, TOperationEvent> OnEvents(Func<IEventsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnErrors(Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnExceptions(Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> Catch(Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> CatchOf<TError>(Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptions(Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, x => x.Error.IsException && !x.Error.IsHandled));
        //}

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, x => x.Error.IsException && !x.Error.IsHandled));
        //}



        public OperationStack<TState, TOperationEvent> OnEventsWhere(Func<TOperationEvent, bool> filter, Func<IEventsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}



        public OperationStack<TState, TOperationEvent> OnEvents(Action<IEventsHandler<TOperationEvent, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState, TOperationEvent>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnErrors(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }
        
        public OperationStack<TState, TOperationEvent> OnExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> Catch(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> CatchOf<TError>(Action<IErrorsHandler<TError, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptions(Action<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler));
        //}

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler));
        //}



        public OperationStack<TState, TOperationEvent> OnEventsWhere(Func<TOperationEvent, bool> filter, Action<IEventsHandler<TOperationEvent, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState, TOperationEvent>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, handler, filter));
        //}

        #endregion

        #region Result

        public ICommandResult<TState, TOperationEvent> ToResult(TState initialState)
        {
            return internalStack.ToResult(initialState);
        }

        public Task<ICommandResult<TState, TOperationEvent>> ToResultAsync(TState initialState)
        {
            return internalStack.ToResultAsync(initialState);
        }

        public ICommandResult<TState, TOperationEvent> ToResult()
        {
            return internalStack.ToResult(default(TState));
        }

        public Task<ICommandResult<TState, TOperationEvent>> ToResultAsync()
        {
            return internalStack.ToResultAsync(default(TState));
        }

        ICommandResult<TOperationEvent> ICommandOperation<TOperationEvent>.ToResult()
        {
            return internalStack.ToResult(default(TState));
        }

        async Task<ICommandResult<TOperationEvent>> ICommandOperation<TOperationEvent>.ToResultAsync()
        {
            return await internalStack.ToResultAsync(default(TState)).ConfigureAwait(false);
        }

        #endregion Result
    }

    
    public class OperationStack<TState, TOperationEvent, T> : IQueryOperation<TState, TOperationEvent, T>
        where TOperationEvent : OperationEvent
    {
        #region Fields and Props

        public OperationStackOptions Options => internalStack.Options;
        bool IOperation.SupportsSync => internalStack.Options.SupportsSync;
        bool IOperation.SupportsAsync => internalStack.Options.SupportsAsync;
        bool IOperation.PreferAsync => internalStack.Options.PreferAsync;

        private OperationStackInternal<TState, TOperationEvent> internalStack = new OperationStackInternal<TState, TOperationEvent>();
        private StackBlockSpecBuilder<TState, TOperationEvent> blockSpecBuilder = new StackBlockSpecBuilder<TState, TOperationEvent>();
        
        #endregion Fields and Props

        #region Ctor
        
        internal OperationStack(IEnumerable<StackBlockSpecBase<TState,TOperationEvent>> blocks, OperationStackOptions options)
        {
            internalStack.Blocks = new StackBlocks<TState, TOperationEvent>(blocks);
            internalStack.Options = options;
        }

        #endregion Ctor

        #region Sync

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Then(Action<ICommand<TState, TOperationEvent, T>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }



        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, BlockResult<Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, BlockResult<Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(Func<IStackBlock<TState, TOperationEvent,T>, IQueryResult<TOperationEvent,Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(ICommandResult<TOperationEvent> res)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(IQueryResult<TOperationEvent,Tout> res)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, (op) => res));
        }



        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Then(string tag, Action<ICommand<TState, TOperationEvent, T>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TState, TOperationEvent,T>, BlockResult<Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TState, TOperationEvent,T, Tout>, BlockResult<Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TState, TOperationEvent,T>, IQueryResult<TOperationEvent,Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, ICommandResult<TOperationEvent> res)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, (op) => res));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, IQueryResult<TOperationEvent,Tout> res)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, (op) => res));
        }


        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Finally(Action<ICommand<TState, TOperationEvent, T>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, BlockResult<Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, BlockResult<Tout>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Sync

        #region Async

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent, T>, Task> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }



        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, Task<BlockResult<Tout>>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, Task<BlockResult<Tout>>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(null, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(Func<IStackBlock<TState, TOperationEvent,T>, Task<IQueryResult<TOperationEvent,Tout>>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(null, internalStack.NextIndex, op));
        }



        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent, T>, Task> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TState, TOperationEvent,T>, Task<BlockResult<Tout>>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TState, TOperationEvent,T, Tout>, Task<BlockResult<Tout>>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            //return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew(blockSpecBuilder.BuildCommand(tag, internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TState, TOperationEvent, T>, Task<IQueryResult<TOperationEvent,Tout>>> op)
        {
            //return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildQuery(tag, internalStack.NextIndex, op));
        }


        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(blockSpecBuilder.BuildFinally(internalStack.NextIndex, op));
        }

        #endregion Sync

        #region OnEvents / Catch


        public OperationStack<TState, TOperationEvent,T> OnEvents(Func<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrors(Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptions(Func<IExceptionsErrorHandler<TOperationEvent,Exception,TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> Catch(Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> CatchOf<TError>(Func<IErrorsHandler<TError, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}






        public OperationStack<TState, TOperationEvent,T> OnEventsWhere(Func<TOperationEvent, bool> filter, Func<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent,Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}



        public OperationStack<TState, TOperationEvent,T> OnEvents(Action<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState, TOperationEvent,T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrors(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState, TOperationEvent,T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptions(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> Catch(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> CatchOf<TError>(Action<IErrorsHandler<TError, TState, TOperationEvent, T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptions(Action<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler));
        }

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler));
        //}





        public OperationStack<TState, TOperationEvent,T> OnEventsWhere(Func<TOperationEvent, bool> filter, Action<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>> op)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState, TOperationEvent,T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildEventHandler(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, TOperationEvent,T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildErrorHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, TOperationEvent, T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent, T>> handler)
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent, T> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(blockSpecBuilder.BuildCatchExceptionHandler(internalStack.NextIndex, handler, filter));
        }

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>> handler)
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}

        //public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>> handler)
        //    where TException : Exception
        //{
        //    return internalStack.CreateNew<T>(blockSpecBuilder.BuildExceptionHandler(internalStack.NextIndex, handler, filter));
        //}

        #endregion

        #region Result

        public IQueryResult<TState, TOperationEvent,T> ToResult(TState initialState)
        {
            return internalStack.ToResult<T>(initialState);
        }

        public Task<IQueryResult<TState, TOperationEvent, T>> ToResultAsync(TState initialState)
        {
            return internalStack.ToResultAsync<T>(initialState);
        }

        public IQueryResult<TState, TOperationEvent, T> ToResult()
        {
            return internalStack.ToResult<T>(default(TState));
        }

        public Task<IQueryResult<TState, TOperationEvent, T>> ToResultAsync()
        {
            return internalStack.ToResultAsync<T>(default(TState));
        }

    
        IQueryResult<TOperationEvent, T> IQueryOperation<TOperationEvent, T>.ToResult()
        {
            return internalStack.ToResult<T>(default(TState));
        }

        async Task<IQueryResult<TOperationEvent, T>> IQueryOperation<TOperationEvent, T>.ToResultAsync()
        {
            return await internalStack.ToResultAsync<T>(default(TState)).ConfigureAwait(false);
        }

        #endregion Result
    }

    public class OperationStack : OperationStack<object, OperationEvent>
    {
        public OperationStack()
            : base()
        {

        }
        public OperationStack(OperationStackOptions options)
            : base(options)
        {

        }
    }

    public class OperationStack<TOperationEvent> : OperationStack<object, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        public OperationStack()
            : base()
        {

        }
        public OperationStack(OperationStackOptions options)
            : base( options)
        {

        }
    }
}
