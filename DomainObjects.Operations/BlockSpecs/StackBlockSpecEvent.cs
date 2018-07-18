using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainObjects.Operations
{

    internal class StackBlockSpecEvent<TStackInput, TState, TOperationEvent> : StackBlockSpecBase<TStackInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        Func<TStackInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TStackInput, TState, TOperationEvent>> blockBuilder;

        public override Type InputType => null;

        public StackBlockSpecEvent(int index, Func<TStackInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TStackInput, TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType = BlockSpecTypes.EventsHandler)
            : base("EventHandler " + index, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        internal override StackBlockBase<TStackInput, TState, TOperationEvent> CreateBlock(TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(stackInput, state, stackEvents, input);
        }
    }

    internal class StackBlockSpecEvent<TStackInput, TState, TOperationEvent, TBlockInput> : StackBlockSpecBase<TStackInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        Func<TStackInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TStackInput, TState, TOperationEvent>> blockBuilder;

        public override Type InputType => typeof(TBlockInput);

        public StackBlockSpecEvent(int index, Func<TStackInput, TState, IStackEvents<TOperationEvent>, IEmptyable, StackBlockBase<TStackInput, TState, TOperationEvent>> blockBuilder, BlockSpecTypes blockType = BlockSpecTypes.EventsHandler)
            : base("EventHandler " + index, index, blockType)
        {
            this.blockBuilder = blockBuilder;
        }

        internal override StackBlockBase<TStackInput, TState, TOperationEvent> CreateBlock(TStackInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
        {
            return blockBuilder(stackInput, state, stackEvents, input);
        }
    }

}
