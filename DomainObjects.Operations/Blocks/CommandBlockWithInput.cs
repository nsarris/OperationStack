using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class CommandBlock<Tin> : CommandBlock, ICommand<TInput, TState, Tin>
        {
            public new Emptyable<Tin> Input { get => (Emptyable<Tin>)base.Input; private set => base.Input = value; }

            internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ICommand<TInput, TState, Tin>, BlockResultVoid> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executor = () => func(this);
            }

            internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Action<ICommand<TInput, TState, Tin>> action)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executor = () => { action(this); return ((IResultVoidDispatcher<TState>)this).Return(); };
            }

            internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput, TState, Tin>, ICommandResult> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executor = () => { var r = func(this); this.Append(r); return ((IResultVoidDispatcher<TState>)this).Return(); };
            }


            internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ICommand<TInput, TState, Tin>, Task<BlockResultVoid>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executorAsync = async () => await func(this).ConfigureAwait(false);
            }

            internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ICommand<TInput, TState, Tin>, Task> action)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executorAsync = async () => { await action(this).ConfigureAwait(false); return ((IResultVoidDispatcher<TState>)this).Return(); };
            }

            internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput, TState, Tin>, Task<ICommandResult>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return ((IResultVoidDispatcher<TState>)this).Return(); };
            }

            internal CommandBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, ICommandOperation commandOperation)
                : base(tag, stackInput, state, stackEvents)
            {
                if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
                    executor = () =>
                    {
                        ICommandResult r;
                        if (commandOperation is ICommandOperation<Tin, TState> operationWithInputAndState)
                            r = operationWithInputAndState.Execute(input.Value, state);
                        else if (commandOperation is ICommandOperationWithState<TState> operationWithState)
                            r = operationWithState.Execute(state);
                        else if (commandOperation is ICommandOperationWithInput<Tin> operationWithInput)
                            r = operationWithInput.Execute(input.Value);
                        else
                            r = commandOperation.Execute();

                        this.Append(r);
                        return ((IResultVoidDispatcher<TState>)this).Return();
                    };
                else
                    executorAsync = async () =>
                    {
                        ICommandResult r;

                        if (commandOperation is ICommandOperation<Tin, TState> operationWithInputAndState)
                            r = await operationWithInputAndState.ExecuteAsync(input.Value, state).ConfigureAwait(false);
                        else if (commandOperation is ICommandOperationWithState<TState> operationWithState)
                            r = await operationWithState.ExecuteAsync(state).ConfigureAwait(false);
                        else if (commandOperation is ICommandOperationWithInput<Tin> operationWithInput)
                            r = await operationWithInput.ExecuteAsync(input.Value).ConfigureAwait(false);
                        else
                            r = await commandOperation.ExecuteAsync().ConfigureAwait(false);

                        this.Append(r);
                        return ((IResultVoidDispatcher<TState>)this).Return();
                    };
            }
        }

    }
}
