using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class CommandBlock<TState, TOperationEvent, Tin> : CommandBlock<TState, TOperationEvent>, ICommand<TState, TOperationEvent,Tin>
        where TOperationEvent : IOperationEvent
    {
        public new Emptyable<Tin> Input { get => (Emptyable<Tin>)base.Input; private set => base.Input = value; }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TState, TOperationEvent,Tin>, BlockResultVoid> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Action<ICommand<TState, TOperationEvent,Tin>> action)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { action(this); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent,Tin>, ICommandResult<TOperationEvent>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }


        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TState, TOperationEvent,Tin>, Task<BlockResultVoid>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this).ConfigureAwait(false); 
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TState, TOperationEvent,Tin>, Task> action)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { await action(this).ConfigureAwait(false); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent,Tin>, Task<ICommandResult<TOperationEvent>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }


    }


}
