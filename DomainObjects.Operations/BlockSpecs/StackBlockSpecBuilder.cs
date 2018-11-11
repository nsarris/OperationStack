using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal static class StackBlockSpecBuilder
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

            #region CatchExceptionHandler

            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildCatchExceptionHandler<TError, TException, TBlockInput>(int index, Action<IExceptionsErrorHandler<TError, TException, TInput, TState, TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
                where TException : Exception
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ExceptionsHandler<TError, TException, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
            }
            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildCatchExceptionHandler<TError, TException, TBlockInput>(int index, Func<IExceptionsErrorHandler<TError, TException, TInput, TState, TBlockInput>, BlockResult<TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
                where TException : Exception
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ExceptionsHandler<TError, TException, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
            }

            #endregion

            #region CatchHandler
            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildCatchHandler<TError, TBlockInput>(int index, Action<IErrorsHandler<TError, TInput, TState, TBlockInput>> func, Func<TError, bool> filter = null)
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ErrorsHandler<TError, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
            }

            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildCatchHandler<TError, TBlockInput>(int index, Func<IErrorsHandler<TError, TInput, TState, TBlockInput>, BlockResult<TBlockInput>> func, Func<TError, bool> filter = null)
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ErrorsHandler<TError, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter, false, true));
            }

            #endregion

            #region Command

            public static StackBlockSpecBase<object,IVoid> BuildCommand(string tag, int index, Func<IOperationBlock<TInput, TState>, ICommandResult> func)
            {
                return new StackBlockSpecOperation<object, IVoid>(tag, index, func);
            }

            public static StackBlockSpecBase<object, IVoid> BuildCommand(string tag, int index, Func<IOperationBlock<TInput, TState>, Task<ICommandResult>> asyncFunc)
            {
                return new StackBlockSpecOperation<object, IVoid>(tag, index, asyncFunc);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildCommand<TBlockInput>(string tag, int index, Func<ICommand<TInput, TState, TBlockInput>, BlockResultVoid> func)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(tag, index, func);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildCommand<TBlockInput>(string tag, int index, Action<ICommand<TInput, TState, TBlockInput>> action)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(tag, index, action);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildCommand<TBlockInput>(string tag, int index, Func<ICommand<TInput, TState, TBlockInput>, Task<BlockResultVoid>> asyncFunc)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(tag, index, asyncFunc);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildCommand<TBlockInput>(string tag, int index, Func<ICommand<TInput, TState, TBlockInput>, Task> asyncAction)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(tag, index, asyncAction);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildCommand<TBlockInput>(string tag, int index, Func<IOperationBlock<TInput, TState, TBlockInput>, ICommandResult> func)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(tag, index, func);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildCommand<TBlockInput>(string tag, int index, ICommandOperation operation)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(tag, index, operation);
            }

            #endregion

            #region ErrorHandler

            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildErrorHandler<TError, TBlockInput>(int index, Action<IErrorsHandler<TError, TInput, TState, TBlockInput>> func, Func<TError, bool> filter = null)
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ErrorsHandler<TError, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
            }

            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildErrorHandler<TError, TBlockInput>(int index, Func<IErrorsHandler<TError, TInput, TState, TBlockInput>, BlockResult<TBlockInput>> func, Func<TError, bool> filter = null)
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ErrorsHandler<TError, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
            }

            #endregion

            #region EventHandler
            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildEventHandler<TEvent, TBlockInput>(int index, Action<IEventsHandler<TEvent, TInput, TState, TBlockInput>> func, Func<TEvent, bool> filter = null)
                where TEvent : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new EventsHandler<TEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
            }

            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildEventHandler<TEvent, TBlockInput>(int index, Func<IEventsHandler<TEvent, TInput, TState, TBlockInput>, BlockResult<TBlockInput>> func, Func<TEvent, bool> filter = null)
                where TEvent : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new EventsHandler<TEvent, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
            }

            #endregion

            #region ExceptionHandler
            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildExceptionHandler<TError, TException, TBlockInput>(int index, Action<IExceptionsErrorHandler<TError, TException, TInput, TState, TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
                where TException : Exception
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ExceptionsHandler<TError, TException, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
            }

            public static StackBlockSpecBase<TBlockInput, TBlockInput> BuildExceptionHandler<TError, TException, TBlockInput>(int index, Func<IExceptionsErrorHandler<TError, TException, TInput, TState, TBlockInput>, BlockResult<TBlockInput>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
                where TException : Exception
                where TError : OperationEvent
            {
                return new StackBlockSpecEvent<TBlockInput>(index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new ExceptionsHandler<TError, TException, TBlockInput>("", stackInput, state, input.ConvertTo<TBlockInput>(), stackEvents, func, filter));
            }

            #endregion

            #region Finally

            public static StackBlockSpecBase<TBlockInput, TResult> BuildFinally<TBlockInput, TResult>(int index, Func<IQuery<TInput, TState, TBlockInput>, BlockResult<TResult>> func)
            {
                return new StackBlockSpecOperation<TBlockInput, TResult>(null, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildFinally<TBlockInput, TResult>(int index, Func<IQuery<TInput, TState, TBlockInput>, Task<BlockResult<TResult>>> func)
            {
                return new StackBlockSpecOperation<TBlockInput, TResult>(null, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildFinally<TBlockInput, TResult>(int index, Func<ITypedQuery<TInput, TState, TBlockInput, TResult>, Task<BlockResult<TResult>>> func)
            {
                return new StackBlockSpecOperation<TBlockInput, TResult>(null, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildFinally<TBlockInput, TResult>(int index, Func<ITypedQuery<TInput, TState, TBlockInput, TResult>, BlockResult<TResult>> func)
            {
                return new StackBlockSpecOperation<TBlockInput, TResult>(null, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildFinally<TBlockInput>(int index, Action<ICommand<TInput, TState, TBlockInput>> action)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(null, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), action), BlockSpecTypes.Finally);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildFinally<TBlockInput>(int index, Func<ICommand<TInput, TState, TBlockInput>, BlockResultVoid> func)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(null, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
            }

            public static StackBlockSpecBase<TBlockInput, IVoid> BuildFinally<TBlockInput>(int index, Func<ICommand<TInput, TState, TBlockInput>, Task<BlockResultVoid>> func)
            {
                return new StackBlockSpecOperation<TBlockInput, IVoid>(null, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Finally);
            }

            #endregion

            #region Query

            public static StackBlockSpecBase<TBlockInput, TResult> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IOperationBlock<TInput, TState, TBlockInput>, IQueryResult<TResult>> func)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<TBlockInput, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IOperationBlock<TInput, TState, TBlockInput>, Task<IQueryResult<TResult>>> asyncFunc)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<TBlockInput, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IQuery<TInput, TState, TBlockInput>, BlockResult<TResult>> func)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<TBlockInput, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<IQuery<TInput, TState, TBlockInput>, Task<BlockResult<TResult>>> asyncFunc)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<TBlockInput, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<ITypedQuery<TInput, TState, TBlockInput, TResult>, BlockResult<TResult>> action)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<TBlockInput, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), action), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildQuery<TBlockInput, TResult>(string tag, int index, Func<ITypedQuery<TInput, TState, TBlockInput, TResult>, Task<BlockResult<TResult>>> asyncFunc)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<TBlockInput, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<TBlockInput, TResult> BuildQuery<TBlockInput, TResult>(string tag, int index, IQueryOperation<TResult> operation)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<TBlockInput, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TBlockInput, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), operation), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<object, TResult> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TInput, TState>, IQueryResult<TResult>> func)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<object, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TResult>(tag, stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
            }

            public static StackBlockSpecBase<object, TResult> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TInput, TState>, Task<IQueryResult<TResult>>> asyncFunc)
            {
                tag = HandleOperationTagName(tag, index);
                return new StackBlockSpecOperation<object, TResult>(tag, index, (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new QueryBlock<TResult>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
            }

            #endregion
        }
    }
}
