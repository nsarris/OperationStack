using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class StackBlockSpecEvent<TBlockInput> : StackBlockSpecBase<TBlockInput, TBlockInput>
        {
            public StackBlockSpecEvent(int index, Func<TInput, TState, IStackEvents, IEmptyable, StackBlockBase> blockBuilder, BlockSpecTypes blockType = BlockSpecTypes.EventsHandler)
                : base("EventHandler " + index, index, blockType, blockBuilder)
            {
            }
        }
    }
}
