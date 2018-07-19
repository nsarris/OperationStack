using System;

namespace DomainObjects.Operations
{


    internal class StackBlockSpecOperation<TInput, TState, TOperationEvent> : StackBlockSpecBase<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        readonly Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder;

        public StackBlockSpecOperation(string tag, int index, Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override Type InputType => null;

        internal override StackBlockBase<TInput, TState, TOperationEvent> CreateBlock(TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(stackInput, state, stackEvents, input);
        }
    }

    internal class StackBlockSpecOperation<TInput, TState, TOperationEvent, Tin> : StackBlockSpecBase<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        readonly Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder;

        public StackBlockSpecOperation(string tag, int index, Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override Type InputType => typeof(Tin);

        internal override StackBlockBase<TInput, TState, TOperationEvent> CreateBlock(TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(stackInput, state, stackEvents, input);
        }
    }
}
