using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class CommandBlock<TInput, TState, TOperationEvent> : StackBlockBase<TInput, TState, TOperationEvent>, ICommand<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        private readonly ResultVoidDispatcher<TState, TOperationEvent> resultDispatcher = new ResultVoidDispatcher<TState, TOperationEvent>();

        protected CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents)
            : base(tag, input, state, stackEvents)
        {

        }

        internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TInput, TState, TOperationEvent>, BlockResultVoid> func)
            : base(tag, input, state, stackEvents)
        {
            executor = () => func(this);
        }
        internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, Action<ICommand<TInput, TState, TOperationEvent>> action)
            : base(tag, input, state, stackEvents)
        {
            executor = () => { action(this); return resultDispatcher.Return(); };
        }

        internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TInput, TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
            : base(tag, input, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispatcher.Return(); };
        }


        internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TInput, TState, TOperationEvent>, Task<BlockResultVoid>> func)
            : base(tag, input, state, stackEvents)
        {
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TInput, TState, TOperationEvent>, Task> actionAsync)
            : base(tag, input, state, stackEvents)
        {
            executorAsync = async () => { await actionAsync(this).ConfigureAwait(false); return resultDispatcher.Return(); };
        }

        internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TInput, TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> func)
            : base(tag, input, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return resultDispatcher.Return(); };
        }

        internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, ICommandOperation<TOperationEvent> commandOperation)
            : base(tag, input, state, stackEvents)
        {
            if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
                executor = () =>
                {
                    ICommandResult<TOperationEvent> r;
                    if (commandOperation is ICommandOperationWithState<TState, TOperationEvent> operationWithState)
                        r = operationWithState.Execute(state);
                    else
                        r = commandOperation.Execute();

                    this.Append(r);
                    return resultDispatcher.Return();
                };
            else
                executorAsync = async () =>
                {
                    ICommandResult<TOperationEvent> r;
                    if (commandOperation is ICommandOperationWithState<TState, TOperationEvent> operationWithState)
                        r = operationWithState.Execute(state);
                    else
                        r = await commandOperation.ExecuteAsync().ConfigureAwait(false);

                    this.Append(r);
                    return resultDispatcher.Return();
                };
        }


        //internal CommandBlock(string tag, TInput input, TState state, IStackEvents<TOperationEvent> stackEvents, ICommandOperationWithState<TState, TOperationEvent> commandOperation)
        //    : base(tag, input, state, stackEvents)
        //{
        //    if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
        //        executor = () => { var r = commandOperation.Execute(state); this.Append(r); this.StackState = (TState)r.StackState; return resultDispatcher.Return(); };
        //    else
        //        executorAsync = async () => { var r = await commandOperation.ExecuteAsync(state).ConfigureAwait(false); this.Append(r); this.StackState = (TState)r.StackState; return resultDispatcher.Return(); };
        //}

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Fail()
        {
            return resultDispatcher.Fail();
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Fail(TOperationEvent error)
        {
            return resultDispatcher.Fail(error);
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Complete()
        {
            return resultDispatcher.Complete();
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Complete(object overrideResult)
        {
            return resultDispatcher.Complete(overrideResult);
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Reset()
        {
            return resultDispatcher.Reset();
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Reset(TState state)
        {
            return resultDispatcher.Reset(state);
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Restart()
        {
            return resultDispatcher.Restart();
        }



        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Return()
        {
            return resultDispatcher.Return();
        }



        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }



        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Goto(int index)
        {
            return resultDispatcher.Goto(index);
        }



        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Goto(int index, object overrideInput)
        {
            return resultDispatcher.Goto(index, overrideInput);
        }



        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }



        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }

        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Retry()
        {
            return resultDispatcher.Retry();
        }



        BlockResultVoid IResultVoidDispatcher<TState, TOperationEvent>.Retry(object overrideInput)
        {
            return resultDispatcher.Retry(overrideInput);
        }
    }


}
