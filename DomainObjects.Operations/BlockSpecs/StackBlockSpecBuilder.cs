using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal static class StackBlockSpecBuilder<TStackInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        private const string BLOCK_TAG = "Block";
        private const string FINALLY_TAG = "Finally";

        private static string HandleOperationTagName(string tag, int index)
        {
            if (string.IsNullOrEmpty(tag))
                return BLOCK_TAG + "_" + index.ToString();
            else
                return tag;
        }

        // CatchExceptionHandler
        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException, TBlockInput>(int index, Action<IExceptionsErrorHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ExceptionsHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
        }
        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException, TBlockInput>(int index, Func<IExceptionsErrorHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>, BlockResult<TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ExceptionsHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
        }

        // CatchHandler
        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCatchHandler<TError, TBlockInput>(int index, Action<IErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCatchHandler<TError, TBlockInput>(int index, Func<IErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>, BlockResult<TBlockInput>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand(string tag, int index, Func<IOperationBlock<TStackInput, TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand(string tag, int index, Func<IOperationBlock<TStackInput,TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TStackInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand<TBlockInput>(string tag, int index, Func<ICommand<TStackInput,TState, TOperationEvent, TBlockInput>, BlockResultVoid> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand<TBlockInput>(string tag, int index, Action<ICommand<TStackInput,TState, TOperationEvent, TBlockInput>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), action), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand<TBlockInput>(string tag, int index, Func<ICommand<TStackInput, TState, TOperationEvent, TBlockInput>, Task<BlockResultVoid>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand<TBlockInput>(string tag, int index, Func<ICommand<TStackInput, TState, TOperationEvent, TBlockInput>, Task> asyncAction)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncAction), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand<TBlockInput>(string tag, int index, Func<IOperationBlock<TStackInput, TState, TOperationEvent, TBlockInput>, ICommandResult<TOperationEvent>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildCommand<TBlockInput>(string tag, int index, ICommandOperation<TOperationEvent> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), operation), BlockSpecTypes.Operation);
        }

        // ErrorHandler
        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildErrorHandler<TError, TBlockInput>(int index, Action<IErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildErrorHandler<TError, TBlockInput>(int index, Func<IErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>, BlockResult<TBlockInput>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ErrorsHandler<TError, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
        }

        // EventHandler
        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildEventHandler<TEvent, TBlockInput>(int index, Action<IEventsHandler<TEvent, TStackInput, TState, TOperationEvent, TBlockInput>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new EventsHandler<TEvent, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildEventHandler<TEvent, TBlockInput>(int index, Func<IEventsHandler<TEvent, TStackInput, TState, TOperationEvent, TBlockInput>, BlockResult<TBlockInput>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new EventsHandler<TEvent, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
        }

        // ExceptionHandler
        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildExceptionHandler<TError, TException, TBlockInput>(int index, Action<IExceptionsErrorHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ExceptionsHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildExceptionHandler<TError, TException, TBlockInput>(int index, Func<IExceptionsErrorHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>, BlockResult<TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput>(index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ExceptionsHandler<TError, TException, TStackInput, TState, TOperationEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildFinally<TBlockInput, TResult>(int index, Func<IQuery<TStackInput, TState, TOperationEvent, TBlockInput>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(null, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildFinally<TBlockInput, TResult>(int index, Func<IQuery<TStackInput, TState, TOperationEvent, TBlockInput>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(null, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildFinally<TBlockInput, TResult>(int index, Func<ITypedQuery<TStackInput, TState, TOperationEvent, TBlockInput, TResult>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(null, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildFinally<TBlockInput, TResult>(int index, Func<ITypedQuery<TStackInput, TState, TOperationEvent, TBlockInput, TResult>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(null, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildFinally<TBlockInput>(int index, Action<ICommand<TStackInput, TState, TOperationEvent, TBlockInput>> action)
        {
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(null, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), action), BlockSpecTypes.Finally);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildFinally<TBlockInput>(int index, Func<ICommand<TStackInput, TState, TOperationEvent, TBlockInput>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(null, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildFinally<TBlockInput>(int index, Func<ICommand<TStackInput, TState, TOperationEvent, TBlockInput>, Task<BlockResultVoid>> func)
        {
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(null, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TStackInput, TState, TOperationEvent, TBlockInput>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
        }

        // Query
        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IOperationBlock<TStackInput, TState, TOperationEvent, TBlockInput>, IQueryResult<TOperationEvent, TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IOperationBlock<TStackInput, TState, TOperationEvent, TBlockInput>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IQuery<TStackInput, TState, TOperationEvent, TBlockInput>, BlockResult<TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IQuery<TStackInput, TState, TOperationEvent, TBlockInput>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<ITypedQuery<TStackInput, TState, TOperationEvent, TBlockInput, TResult>, BlockResult<TResult>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), action), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<ITypedQuery<TStackInput, TState, TOperationEvent, TBlockInput, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TBlockInput, TResult>(string tag, int index, IQueryOperation<TOperationEvent, TResult> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent, TBlockInput>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), operation), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TStackInput, TState, TOperationEvent>, IQueryResult<TOperationEvent, TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public static StackBlockSpecBase<TStackInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TStackInput, TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TStackInput, TState, TOperationEvent>(tag, index, (TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TStackInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }
    }
}
