using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput> : IQueryOperation<TInput, TState, TOutput>
        
    {
        #region Fields and Props

        public OperationStackOptions Options => internalStack.Options;
        bool IOperation.SupportsSync => internalStack.Options.SupportsSync;
        bool IOperation.SupportsAsync => internalStack.Options.SupportsAsync;
        bool IOperation.PreferAsync => internalStack.Options.PreferAsync;
        private int NextIndex => internalStack.NextIndex;

        private readonly OperationStackInternal internalStack;

        #endregion Fields and Props

        #region Ctor

        internal OperationStack(IEnumerable<IStackBlockSpec> blocks, OperationStackOptions options, Func<TState> initalStateBuilder, bool hasInput)
        {
            internalStack = new OperationStackInternal(options, initalStateBuilder, hasInput, new StackBlocks(blocks));
        }

        #endregion Ctor

        #region Block Builder
        private OperationStack<TInput, TState, IVoid> CreateNew(IStackBlockSpec block)
        {
            return internalStack.CreateNew(block);
        }

        private OperationStack<TInput, TState, TNewOutput> CreateNew<TNewOutput>(IStackBlockSpec block)
        {
            return internalStack.CreateNew<TNewOutput>(block);
        }

        #endregion

        #region OnEvents / Catch

        // Catches
        public OperationStack<TInput, TState, TOutput> Catch(Action<IErrorsHandler<OperationEvent, TInput, TState, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> Catch(Func<IErrorsHandler<OperationEvent, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptions(Action<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptions(Func<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptionsOf<TException>(Action<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptionsOf<TException>(Func<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<OperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptionsOfWhere<TException>(Func<IOperationExceptionError<OperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptionsWhere(Func<IOperationExceptionError<OperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> CatchExceptionsWhere(Func<IOperationExceptionError<OperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> CatchOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> CatchOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> CatchOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> CatchOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> CatchWhere(Func<OperationEvent, bool> filter, Action<IErrorsHandler<OperationEvent, TInput, TState, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> CatchWhere(Func<OperationEvent, bool> filter, Func<IErrorsHandler<OperationEvent, TInput, TState, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildCatchHandler(this.NextIndex, handler, filter));
        }



        // Errors
        public OperationStack<TInput, TState, TOutput> OnErrors(Action<IErrorsHandler<OperationEvent, TInput, TState, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnErrors(Func<IErrorsHandler<OperationEvent, TInput, TState, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TInput, TState, TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TInput, TState, TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TError : OperationEvent

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnErrorsWhere(Func<OperationEvent, bool> filter, Action<IErrorsHandler<OperationEvent, TInput, TState, TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnErrorsWhere(Func<OperationEvent, bool> filter, Func<IErrorsHandler<OperationEvent, TInput, TState, TOutput>, BlockResult<TOutput>> handler)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildErrorHandler(this.NextIndex, handler, filter));
        }


        // Events
        public OperationStack<TInput, TState, TOutput> OnEvents(Action<IEventsHandler<OperationEvent, TInput, TState, TOutput>> op)

        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOutput> OnEvents(Func<IEventsHandler<OperationEvent, TInput, TState, TOutput>, BlockResult<TOutput>> op)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOutput> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TInput, TState, TOutput>> op)
            where TEvent : OperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOutput> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TInput, TState, TOutput>, BlockResult<TOutput>> op)
            where TEvent : OperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TOutput> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TInput, TState, TOutput>> op)
            where TEvent : OperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TInput, TState, TOutput>, BlockResult<TOutput>> op)
            where TEvent : OperationEvent
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnEventsWhere(Func<OperationEvent, bool> filter, Action<IEventsHandler<OperationEvent, TInput, TState, TOutput>> op)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnEventsWhere(Func<OperationEvent, bool> filter, Func<IEventsHandler<OperationEvent, TInput, TState, TOutput>, BlockResult<TOutput>> op)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildEventHandler(this.NextIndex, op, filter));
        }



        // Exceptions
        public OperationStack<TInput, TState, TOutput> OnExceptions(Action<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnExceptions(Func<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler));
        }

        public OperationStack<TInput, TState, TOutput> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<OperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<OperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<OperationEvent, TException, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
            where TException : Exception
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnExceptionsWhere(Func<IOperationExceptionError<OperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        public OperationStack<TInput, TState, TOutput> OnExceptionsWhere(Func<IOperationExceptionError<OperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<OperationEvent, Exception, TInput, TState, TOutput>, BlockResult<TOutput>> handler)
        {
            return this.CreateNew<TOutput>(StackBlockSpecBuilder.BuildExceptionHandler(this.NextIndex, handler, filter));
        }

        #endregion

        #region Sync

        // Finally
        public OperationStack<TInput, TState, IVoid> Finally(Action<ICommand<TInput, TState, TOutput>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> Finally(Func<ICommand<TInput, TState, TOutput>, BlockResultVoid> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> FinallyReturn<TNewOutput>(Func<IQuery<TInput, TState, TOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> FinallyReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOutput, TNewOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildFinally(this.NextIndex, op));
        }



        // Then
        public OperationStack<TInput, TState, IVoid> Then(Action<ICommand<TInput, TState, TOutput>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> Then(Func<ICommand<TInput, TState, TOutput>, BlockResultVoid> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> Then(string tag, Action<ICommand<TInput, TState, TOutput>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> Then(string tag, Func<ICommand<TInput, TState, TOutput>, BlockResultVoid> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(tag, this.NextIndex, op));
        }



        // ThenAppend
        public OperationStack<TInput, TState, IVoid> ThenAppend(Func<IOperationBlock<TInput, TState, TOutput>, ICommandResult> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(Func<IStackBlock<TInput, TState, TOutput>, IQueryResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> ThenAppend(ICommandOperation command)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand<TOutput>(null, this.NextIndex, command));
        }

        public OperationStack<TInput, TState, IVoid> ThenAppend(ICommandResult res)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(null, this.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, IVoid> ThenAppend(string tag, Func<IOperationBlock<TInput, TState>, ICommandResult> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(string tag, Func<IStackBlock<TInput, TState, TOutput>, IQueryResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(IQueryResult<TNewOutput> res)
        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(null, this.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(IQueryOperation<TNewOutput> query)
        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery<TOutput, TNewOutput>(null, this.NextIndex, query));
        }

        public OperationStack<TInput, TState, IVoid> ThenAppend(string tag, ICommandResult res)
        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(tag, this.NextIndex, (op) => res));
        }

        public OperationStack<TInput, TState, IVoid> ThenAppend(string tag, ICommandOperation command)
        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand<TOutput>(tag, this.NextIndex, command));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(string tag, IQueryOperation<TNewOutput> query)
        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery<TOutput, TNewOutput>(tag, this.NextIndex, query));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(string tag, IQueryResult<TNewOutput> res)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(tag, this.NextIndex, (op) => res));
        }



        // ThenReturn
        public OperationStack<TInput, TState, TNewOutput> ThenReturn<TNewOutput>(Func<IQuery<TInput, TState, TOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenReturn<TNewOutput>(string tag, Func<IQuery<TInput, TState, TOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOutput, TNewOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenReturnOf<TNewOutput>(string tag, Func<ITypedQuery<TInput, TState, TOutput, TNewOutput>, BlockResult<TNewOutput>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(tag, this.NextIndex, op));
        }

        #endregion Sync

        #region Async

        // Finally
        public OperationStack<TInput, TState, IVoid> Finally(Func<ICommand<TInput, TState, TOutput>, Task<BlockResultVoid>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> FinallyReturn<TNewOutput>(Func<IQuery<TInput, TState, TOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildFinally(this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> FinallyReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOutput, TNewOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildFinally(this.NextIndex, op));
        }



        // Then
        public OperationStack<TInput, TState, IVoid> Then(Func<ICommand<TInput, TState, TOutput>, Task<BlockResultVoid>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> Then(Func<ICommand<TInput, TState, TOutput>, Task> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> Then(string tag, Func<ICommand<TInput, TState, TOutput>, Task<BlockResultVoid>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> Then(string tag, Func<ICommand<TInput, TState, TOutput>, Task> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(tag, this.NextIndex, op));
        }



        // ThenAppend
        public OperationStack<TInput, TState, IVoid> ThenAppend(Func<IOperationBlock<TInput, TState>, Task<ICommandResult>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(Func<IStackBlock<TInput, TState, TOutput>, Task<IQueryResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, IVoid> ThenAppend(string tag, Func<IOperationBlock<TInput, TState>, Task<ICommandResult>> op)

        {
            return this.CreateNew(StackBlockSpecBuilder.BuildCommand(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenAppend<TNewOutput>(string tag, Func<IStackBlock<TInput, TState, TOutput>, Task<IQueryResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(tag, this.NextIndex, op));
        }



        // ThenReturn
        public OperationStack<TInput, TState, TNewOutput> ThenReturn<TNewOutput>(Func<IQuery<TInput, TState, TOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(null, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenReturn<TNewOutput>(string tag, Func<IQuery<TInput, TState, TOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(tag, this.NextIndex, op));
        }


        public OperationStack<TInput, TState, TNewOutput> ThenReturnOf<TNewOutput>(string tag, Func<ITypedQuery<TInput, TState, TOutput, TNewOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(tag, this.NextIndex, op));
        }

        public OperationStack<TInput, TState, TNewOutput> ThenReturnOf<TNewOutput>(Func<ITypedQuery<TInput, TState, TOutput, TNewOutput>, Task<BlockResult<TNewOutput>>> op)

        {
            return this.CreateNew<TNewOutput>(StackBlockSpecBuilder.BuildQuery(null, this.NextIndex, op));
        }

        #endregion Async

        #region Execute

        // Sync
        IQueryResult<TOutput> IQueryOperation<TOutput>.Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        public IQueryResult<TInput, TState, TOutput> Execute()
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput));
        }

        IQueryResult<TOutput> IQueryOperationWithInput<TInput, TOutput>.Execute(TInput input)
        {
            return this.Execute(input);
        }

        public IQueryResult<TInput, TState, TOutput> Execute(TInput input)
        {
            return internalStack.Execute<TOutput>(input);
        }

        IQueryResult<TOutput> IQueryOperationWithState<TState, TOutput>.Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(initialState);
        }

        public IQueryResult<TInput, TState, TOutput> Execute(TState initialState)
        {
            internalStack.AssertInput();
            return this.Execute(default(TInput), initialState);
        }

        public IQueryResult<TInput, TState, TOutput> Execute(TInput input, TState initialState)
        {
            return internalStack.Execute<TOutput>(input, initialState);
        }


        // Async
        async Task<IQueryResult<TOutput>> IQueryOperation<TOutput>.ExecuteAsync()
        {
            internalStack.AssertInput();
            return await this.ExecuteAsync(default(TInput)).ConfigureAwait(false);
        }

        public Task<IQueryResult<TInput, TState, TOutput>> ExecuteAsync()
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput));
        }

        async Task<IQueryResult<TOutput>> IQueryOperationWithInput<TInput, TOutput>.ExecuteAsync(TInput input)
        {
            return await this.ExecuteAsync(input).ConfigureAwait(false);
        }

        public Task<IQueryResult<TInput, TState, TOutput>> ExecuteAsync(TInput input)
        {
            return internalStack.ExecuteAsync<TOutput>(input);
        }

        async Task<IQueryResult<TOutput>> IQueryOperationWithState<TState, TOutput>.ExecuteAsync(TState initialState)
        {
            return await this.ExecuteAsync(initialState).ConfigureAwait(false);
        }

        public Task<IQueryResult<TInput, TState, TOutput>> ExecuteAsync(TState initialState)
        {
            internalStack.AssertInput();
            return this.ExecuteAsync(default(TInput), initialState);
        }

        public Task<IQueryResult<TInput, TState, TOutput>> ExecuteAsync(TInput input, TState initialState)
        {
            return internalStack.ExecuteAsync<TOutput>(input, initialState);
        }

        #endregion Result
    }

    public class OperationStack<TInput, TState> : OperationStack<TInput, TState, IVoid>
    {
        internal OperationStack(IEnumerable<IStackBlockSpec> blocks, OperationStackOptions options, Func<TState> initalStateBuilder, bool hasInput)
            : base(blocks, options, initalStateBuilder, hasInput)
        {
        }
    }

    public class OperationStackWithoutState<TInput, TOutput> : OperationStack<TInput, object, TOutput>
    {
        internal OperationStackWithoutState(IEnumerable<IStackBlockSpec> blocks, OperationStackOptions options, Func<object> initalStateBuilder, bool hasInput) 
            : base(blocks, options, initalStateBuilder, hasInput)
        {
        }
    }
}
