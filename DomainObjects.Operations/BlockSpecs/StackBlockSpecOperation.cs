using System;

namespace DomainObjects.Operations
{
    //internal delegate StackBlockBase<TState, TOperationEvent> BlockBuilderDelegate<TState, TOperationEvent>(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input);

    internal class StackBlockSpecOperation<TState, TOperationEvent> : StackBlockSpecBase<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder;

        public StackBlockSpecOperation(string tag, int index, Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }
}
