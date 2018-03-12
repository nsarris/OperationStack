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

        internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, ICommandOperation<TOperationEvent> commandOperation)
            : base(tag, stackInput, state, stackEvents)
        {
            if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
                executor = () => {
                    ICommandResult<TOperationEvent> r;
                    if (commandOperation is ICommandOperation<Tin, TState, TOperationEvent> operationWithInputAndState)
                        r = operationWithInputAndState.Execute(input.Value, state);
                    else if (commandOperation is ICommandOperationWithState<TState, TOperationEvent> operationWithState)
                        r = operationWithState.Execute(state);
                    else if (commandOperation is ICommandOperationWithInput<Tin, TOperationEvent> operationWithInput)
                        r = operationWithInput.Execute(input.Value);
                    else
                        r = commandOperation.Execute();

                    this.Append(r);
                    return ((IResultVoidDispatcher<TState>)this).Return();
                };
            else
                executorAsync = async () => {
                    ICommandResult<TOperationEvent> r;

                    if (commandOperation is ICommandOperation<Tin, TState, TOperationEvent> operationWithInputAndState)
                        r = await operationWithInputAndState.ExecuteAsync(input.Value, state).ConfigureAwait(false);
                    else if (commandOperation is ICommandOperationWithState<TState, TOperationEvent> operationWithState)
                        r = await operationWithState.ExecuteAsync(state).ConfigureAwait(false);
                    else if (commandOperation is ICommandOperationWithInput<Tin, TOperationEvent> operationWithInput)
                        r = await operationWithInput.ExecuteAsync(input.Value).ConfigureAwait(false);
                    else
                        r = await commandOperation.ExecuteAsync().ConfigureAwait(false);

                    this.Append(r);
                    return ((IResultVoidDispatcher<TState>)this).Return();
                };
        }
    }


}
