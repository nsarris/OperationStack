using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class CommandBlock<TInput, TState, TOperationEvent, Tin> : CommandBlock<TInput, TState, TOperationEvent>, ICommand<TInput,TState, TOperationEvent,Tin>
        where TOperationEvent : OperationEvent
    {
        public new Emptyable<Tin> Input { get => (Emptyable<Tin>)base.Input; private set => base.Input = value; }

        internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TInput,TState, TOperationEvent,Tin>, BlockResultVoid> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Action<ICommand<TInput,TState, TOperationEvent,Tin>> action)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executor = () => { action(this); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }

        internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput,TState, TOperationEvent,Tin>, ICommandResult<TOperationEvent>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }


        internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TInput,TState, TOperationEvent,Tin>, Task<BlockResultVoid>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this).ConfigureAwait(false); 
        }

        internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TInput,TState, TOperationEvent,Tin>, Task> action)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { await action(this).ConfigureAwait(false); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }

        internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput,TState, TOperationEvent,Tin>, Task<ICommandResult<TOperationEvent>>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return ((IResultVoidDispatcher<TState>)this).Return(); };
        }


    }


}
