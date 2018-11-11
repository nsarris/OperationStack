using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class StackBlockSpecOperation<TBlockInput, TBlockOutput> : StackBlockSpecBase<TBlockInput, TBlockOutput>
        {
            public StackBlockSpecOperation(string tag, int index, Func<TInput, TState, IStackEvents, IEmptyable, StackBlockBase> blockBuilder, BlockSpecTypes blockType)
                : base(tag, index, blockType, blockBuilder)
            {
            }

            public StackBlockSpecOperation(string tag, int index, Func<IOperationBlock<TInput, TState>, ICommandResult> func)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock(tag, stackInput, state, stackEvents, func);
            }

            public StackBlockSpecOperation(string tag, int index, Func<IOperationBlock<TInput, TState>, Task<ICommandResult>> asyncFunc)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock(tag, stackInput, state, stackEvents, asyncFunc);
            }

            public StackBlockSpecOperation(string tag, int index, Func<ICommand<TInput, TState, TBlockInput>, BlockResultVoid> func)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func);
            }

            public StackBlockSpecOperation(string tag, int index, Action<ICommand<TInput, TState, TBlockInput>> action)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), action);
            }

            public StackBlockSpecOperation(string tag, int index, Func<ICommand<TInput, TState, TBlockInput>, Task<BlockResultVoid>> asyncFunc)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncFunc);
            }

            public StackBlockSpecOperation(string tag, int index, Func<ICommand<TInput, TState, TBlockInput>, Task> asyncAction)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), asyncAction);
            }

            public StackBlockSpecOperation(string tag, int index, Func<IOperationBlock<TInput, TState, TBlockInput>, ICommandResult> func)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), func);
            }

            public StackBlockSpecOperation(string tag, int index, ICommandOperation operation)
                : base(tag, index, BlockSpecTypes.Operation, null)
            {
                blockBuilder = (TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
                    => new CommandBlock<TBlockInput>(tag, stackInput, state, stackEvents, input.ConvertTo<TBlockInput>(), operation);
            }
        }
    }
}
