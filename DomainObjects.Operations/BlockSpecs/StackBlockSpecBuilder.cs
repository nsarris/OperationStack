using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlockSpecBuilder<TInput, TState, TOperationEvent>
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

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent>, BlockResultVoid> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(tag,stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, Action<ICommand<TInput,TState, TOperationEvent>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput,TState, TOperationEvent>(tag, stackInput, state, stackEvents, action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent>, Task<BlockResultVoid>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent>, Task> asyncAction)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, BlockResultVoid> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Action<ICommand<TInput,TState, TOperationEvent, Tin>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, ICommandResult<TOperationEvent>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task<BlockResultVoid>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task> asyncAction)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand<Tin>(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, Task<ICommandResult<TOperationEvent>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, ICommandOperation<TOperationEvent> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, operation), BlockSpecTypes.Operation);
        }

        //public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCommand(string tag, int index, ICommandOperation<TInput,TState, TOperationEvent> operation)
        //{
        //    tag = HandleOperationTagName(tag, index);
        //    return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        //        => new CommandBlock<TInput, TState, TOperationEvent>(tag, stackInput, state, stackEvents, operation), BlockSpecTypes.Operation);
        //}







        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IQuery<TInput,TState, TOperationEvent>, BlockResult<TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<ITypedQuery<TInput,TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, IQueryResult<TOperationEvent, TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IQuery<TInput,TState, TOperationEvent>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<ITypedQuery<TInput,TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IQuery<TInput,TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<ITypedQuery<TInput,TState, TOperationEvent, Tin, TResult>, BlockResult<TResult>> action)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent, TResult>> func)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IQuery<TInput,TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<ITypedQuery<TInput,TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<Tin, TResult>(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(tag, stackInput, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, IQueryOperation<TOperationEvent, TResult> operation)
        {
            tag = HandleOperationTagName(tag, index);
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, operation), BlockSpecTypes.Operation);
        }

        //public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildQuery<TResult>(string tag, int index, IQueryOperation<TInput,TState, TOperationEvent, TResult> operation)
        //{
        //    tag = HandleOperationTagName(tag, index);
        //    return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(tag, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        //        => new QueryBlock<TInput, TState, TOperationEvent, TResult>(tag, stackInput, state, stackEvents, operation), BlockSpecTypes.Operation);
        //}


        

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally(int index, Func<ICommand<TInput,TState, TOperationEvent>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new CommandBlock<TInput, TState, TOperationEvent>(FINALLY_TAG, stackInput, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally(int index, Action<ICommand<TInput,TState, TOperationEvent>> action)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(FINALLY_TAG, stackInput, state, stackEvents, action), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<TResult>(int index, Func<IQuery<TInput,TState, TOperationEvent>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(FINALLY_TAG, stackInput, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<TResult>(int index, Func<ITypedQuery<TInput,TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(FINALLY_TAG, stackInput, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally(int index, Func<ICommand<TInput,TState, TOperationEvent>, Task<BlockResultVoid>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent>(FINALLY_TAG, stackInput, state, stackEvents, func), BlockSpecTypes.Finally);
        }
        
        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<TResult>(int index, Func<IQuery<TInput,TState, TOperationEvent>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(FINALLY_TAG, stackInput, state, stackEvents, func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<TResult>(int index, Func<ITypedQuery<TInput,TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, TResult>(FINALLY_TAG, stackInput, state, stackEvents, func), BlockSpecTypes.Finally);
        }



        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<Tin>(int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent,Tin>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<Tin>(int index, Action<ICommand<TInput,TState, TOperationEvent, Tin>> action)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<IQuery<TInput,TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<ITypedQuery<TInput,TState, TOperationEvent,Tin, TResult>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<Tin>(int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task<BlockResultVoid>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new CommandBlock<TInput, TState, TOperationEvent, Tin>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<IQuery<TInput,TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildFinally<Tin, TResult>(int index, Func<ITypedQuery<TInput,TState, TOperationEvent, Tin,  TResult>, Task<BlockResult<TResult>>> func)
        {
            return new StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin>(null, index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new QueryBlock<TInput, TState, TOperationEvent, Tin, TResult>(FINALLY_TAG, stackInput, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Finally);
        }
     


        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildEventHandler<TEvent>(int index, Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new EventsHandler<TEvent, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildEventHandler<TEvent, Tin>(int index, Func<IEventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new EventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildErrorHandler<TError>(int index, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildErrorHandler<TError, Tin>(int index, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildExceptionHandler<TError, TException>(int index, Func<IExceptionsErrorHandler<TError, TException,TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildExceptionHandler<TError, TException, Tin>(int index, Func<IExceptionsErrorHandler<TError, TException, TInput, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }





        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildEventHandler<TEvent>(int index, Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new EventsHandler<TEvent, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildEventHandler<TEvent, Tin>(int index, Action<IEventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new EventsHandler<TEvent, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildErrorHandler<TError>(int index, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildErrorHandler<TError, Tin>(int index, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildExceptionHandler<TError, TException>(int index, Action<IExceptionsErrorHandler<TError, TException,TInput, TState, TOperationEvent>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildExceptionHandler<TError, TException, Tin>(int index, Action<IExceptionsErrorHandler<TError, TException, TInput, TState, TOperationEvent, Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter));//, BlockSpecTypes.UnhandledExceptionHandler);
        }



        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchHandler<TError>(int index, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter,false,true));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchHandler<TError, Tin>(int index, Func<IErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException>(int index, Func<IExceptionsErrorHandler<TError, TException,TInput, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException, Tin>(int index, Func<IExceptionsErrorHandler<TError, TException, TInput, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchHandler<TError>(int index, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchHandler<TError, Tin>(int index, Action<IErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ErrorsHandler<TError, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException>(int index, Action<IExceptionsErrorHandler<TError, TException, TInput, TState, TOperationEvent>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent>("", stackInput, state, stackEvents, func, filter, false, true));
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> BuildCatchExceptionHandler<TError, TException, Tin>(int index, Action<IExceptionsErrorHandler<TError, TException, TInput, TState, TOperationEvent, Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin>(index, (TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) 
                => new ExceptionsHandler<TError, TException, TInput, TState, TOperationEvent, Tin>("", stackInput, state, input.ConvertTo<Tin>(), stackEvents, func, filter, false, true));
        }
    }
}
