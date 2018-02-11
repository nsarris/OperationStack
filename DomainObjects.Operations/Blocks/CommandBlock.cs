using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class CommandBlock<TState, TOperationEvent> : StackBlockBase<TState, TOperationEvent>, ICommand<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        private ResultVoidDispatcher<TState> resultDispacther = new ResultVoidDispatcher<TState>();
        
        protected CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents)
            : base(tag, state, stackEvents)
        {

        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, BlockResultVoid> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }
        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<ICommand<TState, TOperationEvent>> action)
            : base(tag, state, stackEvents)
        {
            executor = () => { action(this); return resultDispacther.Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispacther.Return(); };
        }


        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, Task> actionAsync)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { await actionAsync(this).ConfigureAwait(false); return resultDispacther.Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return resultDispacther.Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, ICommandOperation<TOperationEvent> commandOperation)
            : base(tag, state, stackEvents)
        {
            if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
                executor = () => { var r = commandOperation.ToResult(); this.Append(r); return resultDispacther.Return(); };
            else
                executorAsync = async () => { var r = await commandOperation.ToResultAsync().ConfigureAwait(false); this.Append(r); return resultDispacther.Return(); };
        }

        //need to expose state to result
        //internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, ICommandOperation<TState, TOperationEvent> commandOperation)
        //    : base(tag, state, stackEvents)
        //{
        //    if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
        //        executor = () => { var r = commandOperation.ToResult(state); this.Append(r); this.StackState = r.State; return resultDispacther.Return(); };
        //    else
        //        executorAsync = async () => { var r = await commandOperation.ToResultAsync(state).ConfigureAwait(false); this.Append(r); this.StackState = r.State; return resultDispacther.Return(); };
        //}

        BlockResultVoid IResultVoidDispatcher<TState>.Break(bool success)
        {
            return resultDispacther.Break(success);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.End(bool success)
        {
            return resultDispacther.End(success);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.End(bool success , object overrideResult)
        {
            return resultDispacther.End(success, overrideResult);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Reset()
        {
            return resultDispacther.Reset();
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Reset(TState state)
        {
            return resultDispacther.Reset(state);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Restart()
        {
            return resultDispacther.Restart();
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Return(bool success)
        {
            return resultDispacther.Return(success);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Return()
        {
            return resultDispacther.Return();
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Goto(string tag, bool success)
        {
            return resultDispacther.Goto(tag, success);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Goto(string tag)
        {
            return resultDispacther.Goto(tag);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Goto(string tag, object overrideInput, bool success)
        {
            return resultDispacther.Goto(tag, overrideInput, success);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Goto(string tag, object overrideInput)
        {
            return resultDispacther.Goto(tag, overrideInput);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Skip(int i, bool success)
        {
            return resultDispacther.Skip(i, success);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Skip(int i)
        {
            return resultDispacther.Skip(i);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Skip(int i, object overrideInput, bool success)
        {
            return resultDispacther.Skip(i, overrideInput,success);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Skip(int i, object overrideInput)
        {
            return resultDispacther.Skip(i, overrideInput);
        }
    }


}
