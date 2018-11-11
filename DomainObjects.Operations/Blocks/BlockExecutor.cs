using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations.Blocks
{
    internal interface IBlockExecutor<TState, TInput, TBlockInput, TBlockOutput>
    {
        bool IsAsync { get; }
        IBlockResult Execute(IOperationBlock<TInput, TState> block);
        Task<IBlockResult> ExecuteAsync(IOperationBlock<TInput, TState> block);
    }

    internal class StaticResultBlockExecutor<TState, TInput, TBlockInput, TBlockOutput>
        : IBlockExecutor<TState, TInput, TBlockInput, TBlockOutput>
    {
        public bool IsAsync { get; } = false;

        readonly TBlockOutput result;
        public StaticResultBlockExecutor(TBlockOutput result)
        {
            this.result = result;
        }

        public IBlockResult Execute(IOperationBlock<TInput, TState> block)
        {
            return new BlockResult<TBlockOutput>(result)
            {
                Target = new BlockResultTarget()
                {
                    FlowTarget = BlockFlowTarget.Return,
                    State = block.StackState
                },
                ExecutionTime = new ExecutionTime()
            };
        }

        public Task<IBlockResult> ExecuteAsync(IOperationBlock<TInput, TState> block)
        {
            return Task.FromResult(Execute(block));
        }
    }

    internal class CommandBlockExecutor<TState, TInput, TBlockInput>
        : IBlockExecutor<TState, TInput, TBlockInput, IVoid>
    {
        public bool IsAsync { get; } = false;

        readonly Func<ICommand<TInput, TState, TBlockInput>, BlockResultVoid> func;
        public CommandBlockExecutor(Func<ICommand<TInput, TState, TBlockInput>, BlockResultVoid> func)
        {
            this.func = func;
        }

        public IBlockResult Execute(IOperationBlock<TInput, TState> block)
        {
            return func((ICommand<TInput, TState, TBlockInput>)block);
        }

        public Task<IBlockResult> ExecuteAsync(IOperationBlock<TInput, TState> block)
        {
            return Task.FromResult(Execute(block));
        }
    }
}
