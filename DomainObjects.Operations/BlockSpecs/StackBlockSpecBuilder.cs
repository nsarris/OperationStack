using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlockSpecBuilder<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        private const string BLOCK_TAG = "Block";
        private const string FINALLY_TAG = "Finally";

        private string HandleOperationTagName(string tag, int index)
        {
            if (string.IsNullOrEmpty(tag))
                return BLOCK_TAG + "_" + index.ToString();
            else
                return tag;
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, Func<ICommand<TState, TOperationEvent>, BlockResultVoid> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, Action<ICommand<TState, TOperationEvent>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, Func<ICommand<TState, TOperationEvent>, Task> asyncAction)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<ICommand<TState, TOperationEvent, Tin>, BlockResultVoid> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Action<ICommand<TState, TOperationEvent, Tin>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, ICommandResult<TOperationEvent>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<ICommand<TState, TOperationEvent, Tin>, Task<BlockResultVoid>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<ICommand<TState, TOperationEvent, Tin>, Task> asyncAction)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<ICommandResult<TOperationEvent>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, ICommandOperation<TOperationEvent> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, operation), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCommand(string tag, int index, ICommandOperation<TState, TOperationEvent> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TState, TOperationEvent>(tag, state, stackEvents, operation), BlockSpecTypes.Operation);
        }







        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent, TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, BlockResult<TResult>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent, TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, IQueryOperation<TOperationEvent, TResult> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, operation), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, IQueryOperation<TState, TOperationEvent, TResult> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TState, TOperationEvent, TResult>(tag, state, stackEvents, operation), BlockSpecTypes.Operation);
        }


        

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally(int index, Func<ICommand<TState, TOperationEvent>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(FINALLY_TAG, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally(int index, Action<ICommand<TState, TOperationEvent>> action)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(FINALLY_TAG, state, stackEvents, action), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<TResult>(int index, Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(FINALLY_TAG, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<TResult>(int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(FINALLY_TAG, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally(int index, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent>(FINALLY_TAG, state, stackEvents, func), BlockSpecTypes.Finally);
        }
        
        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<TResult>(int index, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(FINALLY_TAG, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<TResult>(int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, TResult>(FINALLY_TAG, state, stackEvents, func), BlockSpecTypes.Finally);
        }



        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<Tin>(int index, Func<ICommand<TState, TOperationEvent, Tin>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent,Tin>(FINALLY_TAG, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<Tin>(int index, Action<ICommand<TState, TOperationEvent, Tin>> action)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(FINALLY_TAG, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<ITypedQuery<TState, TOperationEvent,Tin, TResult>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<Tin>(int index, Func<ICommand<TState, TOperationEvent, Tin>, Task<BlockResultVoid>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new CommandBlock<TState, TOperationEvent, Tin>(FINALLY_TAG, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<ITypedQuery<TState, TOperationEvent, Tin,  TResult>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent, Tin>(null, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new QueryBlock<TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }
        //public StackBlockSpecBase<TState, TOperationEvent> Build<TEvent>(int index, Func<IEventHandler<TEvent, TState>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
        //    where TEvent : IOperationEvent
        //{
        //    return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState>("", state, stackEvents,func, filter));
        //}

        //public StackBlockSpecBase<TState, TOperationEvent> Build<TEvent, Tin>(int index, Func<IEventHandler<TEvent, TState, Tin>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
        //    where TEvent : IOperationEvent
        //{
        //    return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, filter));
        //}


        public StackBlockSpecBase<TState, TOperationEvent> BuildEventHandler<TEvent>(int index, Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildEventHandler<TEvent, Tin>(int index, Func<IEventsHandler<TEvent, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildErrorHandler<TError>(int index, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildErrorHandler<TError, Tin>(int index, Func<IErrorsHandler<TError, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildExceptionHandler<TError, TException>(int index, Func<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent>("", state, stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildExceptionHandler<TError, TException, Tin>(int index, Func<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }





        public StackBlockSpecBase<TState, TOperationEvent> BuildEventHandler<TEvent>(int index, Action<IEventsHandler<TEvent, TState, TOperationEvent>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildEventHandler<TEvent, Tin>(int index, Action<IEventsHandler<TEvent, TState, TOperationEvent, Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildErrorHandler<TError>(int index, Action<IErrorsHandler<TError, TState, TOperationEvent>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildErrorHandler<TError, Tin>(int index, Action<IErrorsHandler<TError, TState, TOperationEvent, Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildExceptionHandler<TError, TException>(int index, Action<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent>("", state, stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildExceptionHandler<TError, TException, Tin>(int index, Action<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent, Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }



        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchHandler<TError>(int index, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent>("", state, stackEvents, func, filter,false,true));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchHandler<TError, Tin>(int index, Func<IErrorsHandler<TError, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException>(int index, Func<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent>("", state, stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException, Tin>(int index, Func<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchHandler<TError>(int index, Action<IErrorsHandler<TError, TState, TOperationEvent>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent>("", state, stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchHandler<TError, Tin>(int index, Action<IErrorsHandler<TError, TState, TOperationEvent, Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException>(int index, Action<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent>("", state, stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException, Tin>(int index, Action<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent, Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent, Tin>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }
    }
}
