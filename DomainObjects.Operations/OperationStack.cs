using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOperationEvent, TOutput> : IQueryOperation<TInput, TState, TOperationEvent, TOutput>
        where TOperationEvent : OperationEvent
    {
        #region Fields and Props

        public OperationStackOptions Options => internalStack.Options;
        bool IOperation.SupportsSync => internalStack.Options.SupportsSync;
        bool IOperation.SupportsAsync => internalStack.Options.SupportsAsync;
        bool IOperation.PreferAsync => internalStack.Options.PreferAsync;
        public int NextIndex => internalStack.NextIndex;

        private readonly OperationStackInternal<TInput, TState, TOperationEvent> internalStack;

        #endregion Fields and Props

        #region Ctor

        internal OperationStack(IEnumerable<StackBlockSpecBase<TInput, TState, TOperationEvent>> blocks, OperationStackOptions options, Func<TState> initalStateBuilder, bool hasInput)
        {
            internalStack = new OperationStackInternal<TInput, TState, TOperationEvent>(options, initalStateBuilder, hasInput, new StackBlocks<TInput, TState, TOperationEvent>(blocks));
        }

        #endregion Ctor

        #region Block Builder
        private OperationStack<TInput, TState, TOperationEvent, IVoid> CreateNew(StackBlockSpecBase<TInput, TState, TOperationEvent> block)
        {
            return internalStack.CreateNew(block);
        }

        private OperationStack<TInput, TState, TOperationEvent, TNewOutput> CreateNew<TNewOutput>(StackBlockSpecBase<TInput, TState, TOperationEvent> block)
        {
            return internalStack.CreateNew<TNewOutput>(block);
        }

        #endregion

        #region OnEvents / Catch

        // Catches
        public OperationStack<TInput, TState, TOperationEvent, TOutput> Catch(Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> Catch(Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptions(Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> CatchWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCatchHandler(this.NextIndex, handler, filter));
        }



        // Errors
        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrors(Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrors(Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TError : TOperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrorsWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnErrorsWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildErrorHandler(this.NextIndex, handler, filter));
        }


        // Events
        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEvents(Action<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>> op)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEvents(Func<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> op)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent, TOutput>> op)
            where TEvent : TOperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> op)
            where TEvent : TOperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent, TOutput>> op)
            where TEvent : TOperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> op)
            where TEvent : TOperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEventsWhere(Func<TOperationEvent, bool> filter, Action<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>> op)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnEventsWhere(Func<TOperationEvent, bool> filter, Func<IEventsHandler<TOperationEvent, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> op)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildEventHandler(this.NextIndex, op, filter));
        }



        // Exceptions
        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptions(Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, TException, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOperationEvent, TOutput> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent, Exception, TInput, TState, TOperationEvent, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        #endregion

        #region Sync

        // Finally
        public OperationStack<TInput, TState, TOperationEvent, IVoid> Finally(Action<ICommand<TInput, TState, TOperationEvent, TOutput>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Finally(Func<ICommand<TInput, TState, TOperationEvent, TOutput>, BlockResultVoid> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> FinallyReturn<TNewOutput>(Func<IQuery<TInput, TState, TOperationEvent, TOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> FinallyReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOperationEvent, TOutput, TNewOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildFinally(this.NextIndex, op));
        }



        // Then
        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(Action<ICommand<TInput, TState, TOperationEvent, TOutput>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(Func<ICommand<TInput, TState, TOperationEvent, TOutput>, BlockResultVoid> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(string tag, Action<ICommand<TInput, TState, TOperationEvent, TOutput>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent, TOutput>, BlockResultVoid> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(tag, this.NextIndex, op));
        }



        // ThenAppend
        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(Func<IOperationBlock<TInput, TState, TOperationEvent, TOutput>, ICommandResult<TOperationEvent>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(Func<IStackBlock<TInput, TState, TOperationEvent, TOutput>, IQueryResult<TOperationEvent, TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(ICommandOperation<TOperationEvent> command)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand<TOutput>(null, this.NextIndex, command));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(ICommandResult<TOperationEvent> res)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(null, this.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(string tag, Func<IStackBlock<TInput, TState, TOperationEvent, TOutput>, IQueryResult<TOperationEvent, TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(IQueryResult<TOperationEvent, TNewOutput> res)
        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(null, this.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(IQueryOperation<TOperationEvent, TNewOutput> query)
        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery<TOutput, TNewOutput>(null, this.NextIndex, query));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(string tag, ICommandResult<TOperationEvent> res)
        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(tag, this.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(string tag, ICommandOperation<TOperationEvent> command)
        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand<TOutput>(tag, this.NextIndex, command));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(string tag, IQueryOperation<TOperationEvent, TNewOutput> query)
        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery<TOutput, TNewOutput>(tag, this.NextIndex, query));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(string tag, IQueryResult<TOperationEvent, TNewOutput> res)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(tag, this.NextIndex, (op) => res));
        }



        // ThenReturn
        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturn<TNewOutput>(Func<IQuery<TInput, TState, TOperationEvent, TOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturn<TNewOutput>(string tag, Func<IQuery<TInput, TState, TOperationEvent, TOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOperationEvent, TOutput, TNewOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturnOf<TNewOutput>(string tag, Func<ITypedQuery<TInput, TState, TOperationEvent, TOutput, TNewOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(tag, this.NextIndex, op));
        }

        #endregion Sync

        #region Async

        // Finally
        public OperationStack<TInput, TState, TOperationEvent, IVoid> Finally(Func<ICommand<TInput, TState, TOperationEvent, TOutput>, Task<BlockResultVoid>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> FinallyReturn<TNewOutput>(Func<IQuery<TInput, TState, TOperationEvent, TOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> FinallyReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOperationEvent, TOutput, TNewOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildFinally(this.NextIndex, op));
        }



        // Then
        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(Func<ICommand<TInput, TState, TOperationEvent, TOutput>, Task<BlockResultVoid>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(Func<ICommand<TInput, TState, TOperationEvent, TOutput>, Task> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent, TOutput>, Task<BlockResultVoid>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> Then(string tag, Func<ICommand<TInput, TState, TOperationEvent, TOutput>, Task> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(tag, this.NextIndex, op));
        }



        // ThenAppend
        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(Func<IStackBlock<TInput, TState, TOperationEvent, TOutput>, Task<IQueryResult<TOperationEvent, TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, IVoid> ThenAppend(string tag, Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenAppend<TNewOutput>(string tag, Func<IStackBlock<TInput, TState, TOperationEvent, TOutput>, Task<IQueryResult<TOperationEvent, TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(tag, this.NextIndex, op));
        }



        // ThenReturn
        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturn<TNewOutput>(Func<IQuery<TInput, TState, TOperationEvent, TOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturn<TNewOutput>(string tag, Func<IQuery<TInput, TState, TOperationEvent, TOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(tag, this.NextIndex, op));
        }


        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturnOf<TNewOutput>(string tag, Func<ITypedQuery<TInput, TState, TOperationEvent, TOutput, TNewOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOperationEvent, TNewOutput> ThenReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOperationEvent, TOutput, TNewOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder<TInput, TState, TOperationEvent>.BuildQuery(null, this.NextIndex, op));
        }

        #endregion Async

        #region Execute

        // Sync
        IQueryResult<TOperationEvent, TOutput> IQueryOperation<TOperationEvent, TOutput>.Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        public IQueryResult<TInput, TState, TOperationEvent, TOutput> Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        IQueryResult<TOperationEvent, TOutput> IQueryOperationWithInput<TInput, TOperationEvent, TOutput>.Execute(TInput input)
        {
            return this.Execute(input);
        }

        public IQueryResult<TInput, TState, TOperationEvent, TOutput> Execute(TInput input)
        {
            return internalStack.Execute<TOutput>(input);
        }

        IQueryResult<TOperationEvent, TOutput> IQueryOperationWithState<TState, TOperationEvent, TOutput>.Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(initialState);
        }

        public IQueryResult<TInput, TState, TOperationEvent, TOutput> Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput), initialState);
        }

        public IQueryResult<TInput, TState, TOperationEvent, TOutput> Execute(TInput input, TState initialState)
        {
            return internalStack.Execute<TOutput>(input, initialState);
        }


        // Async
        async Task<IQueryResult<TOperationEvent, TOutput>> IQueryOperation<TOperationEvent, TOutput>.ExecuteAsync()
        {
            internalStack.AssertInput();
            return await this.ExecuteAsync(default(TInput)).ConfigureAwait(false);
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, TOutput>> ExecuteAsync()
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput));
        }

        async Task<IQueryResult<TOperationEvent, TOutput>> IQueryOperationWithInput<TInput, TOperationEvent, TOutput>.ExecuteAsync(TInput input)
        {
            return await this.ExecuteAsync(input).ConfigureAwait(false);
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, TOutput>> ExecuteAsync(TInput input)
        {
            return internalStack.ExecuteAsync<TOutput>(input);
        }

        async Task<IQueryResult<TOperationEvent, TOutput>> IQueryOperationWithState<TState, TOperationEvent, TOutput>.ExecuteAsync(TState initialState)
        {
            return await this.ExecuteAsync(initialState).ConfigureAwait(false);
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, TOutput>> ExecuteAsync(TState initialState)
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput), initialState);
        }

        public Task<IQueryResult<TInput, TState, TOperationEvent, TOutput>> ExecuteAsync(TInput input, TState initialState)
        {
            return internalStack.ExecuteAsync<TOutput>(input, initialState);
        }

        #endregion Result
    }

    public class OperationStack<TInput, TState, TOperationEvent> : OperationStack<TInput, TState, TOperationEvent, IVoid>
    where TOperationEvent : OperationEvent
    {
        internal OperationStack(IEnumerable<StackBlockSpecBase<TInput, TState, TOperationEvent>> blocks, OperationStackOptions options, Func<TState> initalStateBuilder, bool hasInput)
            : base(blocks, options, initalStateBuilder, hasInput)
        {
        }
    }

    public class OperationStack<TInput, TOutput> : OperationStack<TInput, object, OperationEvent, TOutput>
    {
        internal OperationStack(IEnumerable<StackBlockSpecBase<TInput, object, OperationEvent>> blocks, OperationStackOptions options, Func<object> initalStateBuilder, bool hasInput) 
            : base(blocks, options, initalStateBuilder, hasInput)
        {
        }
    }


}
