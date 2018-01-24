using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlockSpecBuilder<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public StackBlockSpecBase<TState, TOperationEvent> Build(string tag, int index, Func<ICommand<TState, TOperationEvent>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build(string tag, int index, Action<ICommand<TState, TOperationEvent>> action)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent>(tag, state, stackEvents, action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build(string tag, int index, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build(string tag, int index, Func<ICommand<TState, TOperationEvent>, Task> asyncAction)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent>(tag, state, stackEvents, asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin>(string tag, int index, Func<ICommand<TState, TOperationEvent, Tin>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin>(string tag, int index, Action<ICommand<TState, TOperationEvent, Tin>> action)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, ICommandResult<TOperationEvent>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin>(string tag, int index, Func<ICommand<TState, TOperationEvent, Tin>, Task<BlockResultVoid>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin>(string tag, int index, Func<ICommand<TState, TOperationEvent, Tin>, Task> asyncAction)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<ICommandResult<TOperationEvent>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Command<TState, TOperationEvent, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }








        public StackBlockSpecBase<TState, TOperationEvent> Build<TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent, TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin, TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin, TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, BlockResult<TResult>> action)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin, TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent, TResult>> func)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin, TResult>(string tag, int index, Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin, TResult>(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<Tin, TResult>(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent, TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState, TOperationEvent>(tag, index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new Query<TState, TOperationEvent, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
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


        public StackBlockSpecBase<TState, TOperationEvent> Build<TEvent>(int index, Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TEvent, Tin>(int index, Func<IEventsHandler<TEvent, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError>(int index, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError, Tin>(int index, Func<IErrorsHandler<TError, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError, TException>(int index, Func<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent>, BlockResultVoid> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent>("", state, stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError, TException, Tin>(int index, Func<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }





        public StackBlockSpecBase<TState, TOperationEvent> Build<TEvent>(int index, Action<IEventsHandler<TEvent, TState, TOperationEvent>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TEvent, Tin>(int index, Action<IEventsHandler<TEvent, TState, TOperationEvent, Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError>(int index, Action<IErrorsHandler<TError, TState, TOperationEvent>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError, Tin>(int index, Action<IErrorsHandler<TError, TState, TOperationEvent, Tin>> func, Func<TError, bool> filter = null)
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError, TException>(int index, Action<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent>("", state, stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TState, TOperationEvent> Build<TError, TException, Tin>(int index, Action<IExceptionsErrorHandler<TError, TException, TState, TOperationEvent, Tin>> func, Func<IOperationExceptionError<TError, TException>, bool> filter = null)
            where TException : Exception
            where TError : TOperationEvent
        {
            return new StackBlockSpecEvent<TState, TOperationEvent>(index, (TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input) => new ExceptionsHandler<TError, TException, TState, TOperationEvent, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }
    }






    internal class StackBlockSpecEvent<TState, TOperationEvent> : StackBlockSpecBase<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder;
        public StackBlockSpecEvent(int index, Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType = BlockSpecTypes.EventsHandler)
            : base("EventHandler " + index, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }

    //internal delegate StackBlockBase<TState, TOperationEvent> BlockBuilderDelegate<TState, TOperationEvent>(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input);

    internal class StackBlockSpecOperation<TState, TOperationEvent> : StackBlockSpecBase<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder;

        public StackBlockSpecOperation(string tag, int index, Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }
}
