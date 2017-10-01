using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlockSpecBuilder<TState>
    {
        public StackBlockSpecBase<TState> Build(string tag, int index, Func<ICommand<TState>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build(string tag, int index, Action<ICommand<TState>> action)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState>(tag, state, stackEvents, action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build(string tag, int index, Func<IOperationBlock<TState>, ICommandResult> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build(string tag, int index, Func<ICommand<TState>, Task<BlockResultVoid>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build(string tag, int index, Func<ICommand<TState>, Task> asyncAction)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState>(tag, state, stackEvents, asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build(string tag, int index, Func<IOperationBlock<TState>, Task<ICommandResult>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin>(string tag, int index, Func<ICommand<TState, Tin>, BlockResultVoid> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin>(string tag, int index, Action<ICommand<TState, Tin>> action)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin>(string tag, int index, Func<IOperationBlock<TState, Tin>, ICommandResult> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin>(string tag, int index, Func<ICommand<TState, Tin>, Task<BlockResultVoid>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin>(string tag, int index, Func<ICommand<TState, Tin>, Task> asyncAction)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncAction), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin>(string tag, int index, Func<IOperationBlock<TState, Tin>, Task<ICommandResult>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Command<TState, Tin>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }








        public StackBlockSpecBase<TState> Build<TResult>(string tag, int index, Func<IQuery<TState>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<TResult>(string tag, int index, Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<TResult>(string tag, int index, Func<IOperationBlock<TState>, IQueryResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, TResult>(tag, state, stackEvents, func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<TResult>(string tag, int index, Func<IQuery<TState>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<TResult>(string tag, int index, Func<ITypedQuery<TState, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<TResult>(string tag, int index, Func<IOperationBlock<TState>, Task<IQueryResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, TResult>(tag, state, stackEvents, asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin, TResult>(string tag, int index, Func<IQuery<TState, Tin>, BlockResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin, TResult>(string tag, int index, Func<ITypedQuery<TState, Tin, TResult>, BlockResult<TResult>> action)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), action), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin, TResult>(string tag, int index, Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> func)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), func), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin, TResult>(string tag, int index, Func<IQuery<TState, Tin>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin, TResult>(string tag, int index, Func<ITypedQuery<TState, Tin, TResult>, Task<BlockResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }

        public StackBlockSpecBase<TState> Build<Tin, TResult>(string tag, int index, Func<IOperationBlock<TState, Tin>, Task<IQueryResult<TResult>>> asyncFunc)
        {
            return new StackBlockSpecOperation<TState>(tag, index, (TState state, IStackEvents stackEvents, IEmptyable input) => new Query<TState, Tin, TResult>(tag, state, stackEvents, input.ConvertTo<Tin>(), asyncFunc), BlockSpecTypes.Operation);
        }







        //public StackBlockSpecBase<TState> Build<TEvent>(int index, Func<IEventHandler<TEvent, TState>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
        //    where TEvent : IOperationEvent
        //{
        //    return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState>("", state, stackEvents,func, filter));
        //}

        //public StackBlockSpecBase<TState> Build<TEvent, Tin>(int index, Func<IEventHandler<TEvent, TState, Tin>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
        //    where TEvent : IOperationEvent
        //{
        //    return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, filter));
        //}


        public StackBlockSpecBase<TState> Build<TEvent>(int index, Func<IEventsHandler<TEvent, TState>, BlockResultVoid> func, Func<TEvent, bool> filter = null)
            where TEvent : IOperationEvent
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TEvent, Tin>(int index, Func<IEventsHandler<TEvent, TState, Tin>, BlockResult<Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : IOperationEvent
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TError>(int index, Func<IErrorsHandler<TError, TState>, BlockResultVoid> func, Func<TError, bool> filter = null)
            where TError : IOperationError
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TError, Tin>(int index, Func<IErrorsHandler<TError, TState, Tin>, BlockResult<Tin>> func, Func<TError, bool> filter = null)
            where TError : IOperationError
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TException>(int index, Func<IExceptionsErrorHandler<TException, TState>, BlockResultVoid> func, Func<IOperationExceptionError<TException>, bool> filter = null)
            where TException : Exception
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ExceptionsHandler<TException, TState>("", state, stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TState> Build<TException, Tin>(int index, Func<IExceptionsErrorHandler<TException, TState, Tin>, BlockResult<Tin>> func, Func<IOperationExceptionError<TException>, bool> filter = null)
            where TException : Exception
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ExceptionsHandler<TException, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }





        public StackBlockSpecBase<TState> Build<TEvent>(int index, Action<IEventsHandler<TEvent, TState>> func, Func<TEvent, bool> filter = null)
            where TEvent : IOperationEvent
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TEvent, Tin>(int index, Action<IEventsHandler<TEvent, TState, Tin>> func, Func<TEvent, bool> filter = null)
            where TEvent : IOperationEvent
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new EventsHandler<TEvent, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TError>(int index, Action<IErrorsHandler<TError, TState>> func, Func<TError, bool> filter = null)
            where TError : IOperationError
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState>("", state, stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TError, Tin>(int index, Action<IErrorsHandler<TError, TState, Tin>> func, Func<TError, bool> filter = null)
            where TError : IOperationError
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ErrorsHandler<TError, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter));
        }

        public StackBlockSpecBase<TState> Build<TException>(int index, Action<IExceptionsErrorHandler<TException, TState>> func, Func<IOperationExceptionError<TException>, bool> filter = null)
            where TException : Exception
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ExceptionsHandler<TException, TState>("", state, stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }

        public StackBlockSpecBase<TState> Build<TException, Tin>(int index, Action<IExceptionsErrorHandler<TException, TState, Tin>> func, Func<IOperationExceptionError<TException>, bool> filter = null)
            where TException : Exception
        {
            return new StackBlockSpecEvent<TState>(index, (TState state, IStackEvents stackEvents, IEmptyable input) => new ExceptionsHandler<TException, TState, Tin>("", state, input.ConvertTo<Tin>(), stackEvents, func, filter), BlockSpecTypes.UnhandledExceptionHandler);
        }
    }






    internal class StackBlockSpecEvent<TState> : StackBlockSpecBase<TState>
    {
        Func<TState, IStackEvents, IEmptyable, StackBlockBase<TState>> blockBuilder;
        public StackBlockSpecEvent(int index, Func<TState, IStackEvents, IEmptyable, StackBlockBase<TState>> blockBuilder, BlockSpecTypes blockType= BlockSpecTypes.EventsHandler)
            : base("EventHandler " + index, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }

    //internal delegate StackBlockBase<TState> BlockBuilderDelegate<TState>(TState state, IStackEvents stackEvents, IEmptyable input);

    internal class StackBlockSpecOperation<TState> : StackBlockSpecBase<TState>
    {
        Func<TState,IStackEvents,IEmptyable,StackBlockBase<TState>> blockBuilder;

        public StackBlockSpecOperation(string tag, int index, Func<TState, IStackEvents, IEmptyable, StackBlockBase<TState>> blockBuilder, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }
}
