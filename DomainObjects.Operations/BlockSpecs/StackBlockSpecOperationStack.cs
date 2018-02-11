using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainObjects.Operations
{
    internal class StackBlockSpecOperationStack<TState, TOperationEvent> : StackBlockSpecBase<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder;
        public StackBlockSpecOperationStack(int index, Func<TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType = BlockSpecTypes.EventsHandler)
            : base("EventHandler " + index, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(state, stackEvents, input);
        }
    }
}
