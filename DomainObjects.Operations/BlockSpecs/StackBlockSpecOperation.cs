using System;

namespace DomainObjects.Operations
{


    internal class StackBlockSpecOperation<TState, TOperationEvent> : StackBlockSpecBase<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder;

        public StackBlockSpecOperation(string tag, int index, Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override Type InputType => null;

        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }

    internal class StackBlockSpecOperation<TState, TOperationEvent, Tin> : StackBlockSpecBase<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder;

        public StackBlockSpecOperation(string tag, int index, Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override Type InputType => typeof(Tin);

        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }
}
