using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainObjects.Operations
{

    internal class StackBlockSpecEvent<TInput, TState, TOperationEvent> : StackBlockSpecBase<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder;
        public override Type InputType => null;
        public StackBlockSpecEvent(int index, Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType = BlockSpecTypes.EventsHandler)
            : base("EventHandler " + index, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TInput, TState, TOperationEvent> CreateBlock(TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(stackInput, state, stackEvents, input);
        }
    }

    internal class StackBlockSpecEvent<TInput, TState, TOperationEvent, Tin> : StackBlockSpecBase<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder;
        public override Type InputType => typeof(Tin);
        public StackBlockSpecEvent(int index, Func<TInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TInput, TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType = BlockSpecTypes.EventsHandler)
            : base("EventHandler " + index, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        public override StackBlockBase<TInput, TState, TOperationEvent> CreateBlock(TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(stackInput, state, stackEvents, input);
        }
    }

}
