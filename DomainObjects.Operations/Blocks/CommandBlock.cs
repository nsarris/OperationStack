﻿using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class CommandBlock<TState, TOperationEvent> : StackBlockBase<TState, TOperationEvent>, ICommand<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        private ResultVoidDispatcher<TState> resultDispatcher = new ResultVoidDispatcher<TState>();

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
            executor = () => { action(this); return resultDispatcher.Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispatcher.Return(); };
        }


        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, Task> actionAsync)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { await actionAsync(this).ConfigureAwait(false); return resultDispatcher.Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return resultDispatcher.Return(); };
        }

        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, ICommandOperation<TOperationEvent> commandOperation)
            : base(tag, state, stackEvents)
        {
            if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
                executor = () => { var r = commandOperation.ToResult(); this.Append(r); return resultDispatcher.Return(); };
            else
                executorAsync = async () => { var r = await commandOperation.ToResultAsync().ConfigureAwait(false); this.Append(r); return resultDispatcher.Return(); };
        }


        internal CommandBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, ICommandOperation<TState, TOperationEvent> commandOperation)
            : base(tag, state, stackEvents)
        {
            if (commandOperation.SupportsAsync && commandOperation.PreferAsync)
                executor = () => { var r = commandOperation.ToResult(state); this.Append(r); this.StackState = r.StackState; return resultDispatcher.Return(); };
            else
                executorAsync = async () => { var r = await commandOperation.ToResultAsync(state).ConfigureAwait(false); this.Append(r); this.StackState = r.StackState; return resultDispatcher.Return(); };
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Fail()
        {
            return resultDispatcher.Fail();
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Fail(IOperationEvent error)
        {
            return resultDispatcher.Fail(error);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Complete()
        {
            return resultDispatcher.Complete();
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Complete(object overrideResult)
        {
            return resultDispatcher.Complete(overrideResult);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Reset()
        {
            return resultDispatcher.Reset();
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Reset(TState state)
        {
            return resultDispatcher.Reset(state);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Restart()
        {
            return resultDispatcher.Restart();
        }



        BlockResultVoid IResultVoidDispatcher<TState>.Return()
        {
            return resultDispatcher.Return();
        }



        BlockResultVoid IResultVoidDispatcher<TState>.Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }



        BlockResultVoid IResultVoidDispatcher<TState>.Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Goto(int index)
        {
            return resultDispatcher.Goto(index);
        }



        BlockResultVoid IResultVoidDispatcher<TState>.Goto(int index, object overrideInput)
        {
            return resultDispatcher.Goto(index, overrideInput);
        }



        BlockResultVoid IResultVoidDispatcher<TState>.Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }



        BlockResultVoid IResultVoidDispatcher<TState>.Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }

        BlockResultVoid IResultVoidDispatcher<TState>.Retry()
        {
            return resultDispatcher.Retry();
        }



        BlockResultVoid IResultVoidDispatcher<TState>.Retry(object overrideInput)
        {
            return resultDispatcher.Retry(overrideInput);
        }
    }


}