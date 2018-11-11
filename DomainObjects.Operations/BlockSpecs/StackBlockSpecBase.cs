using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public enum BlockSpecTypes
    {
        Operation,
        Finally,
        EventsHandler
    }
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal abstract class StackBlockSpecBase<TBlockInput, TBlockOutput> : IStackBlockSpec<TInput, TState>
        {
            private const string BLOCK_TAG = "Block";
            private const string FINALLY_TAG = "Finally";

            protected Func<TInput, TState, IStackEvents, IEmptyable, StackBlockBase> blockBuilder;

            public string Tag { get; private set; }
            public int Index { get; private set; }
            public BlockSpecTypes BlockType { get; private set; }
            public Type InputType { get; } = typeof(TBlockInput);

            private static string HandleOperationTagName(string tag, int index)
            {
                if (string.IsNullOrEmpty(tag))
                    return BLOCK_TAG + "_" + index.ToString();
                else
                    return tag;
            }

            public IStackBlock CreateBlock(TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input)
            {
                return blockBuilder(stackInput, state, stackEvents, input);
            }

            protected StackBlockSpecBase(int index, BlockSpecTypes blockType, Func<TInput, TState, IStackEvents, IEmptyable, StackBlockBase> blockBuilder)
                : this("Block " + index, index, blockType, blockBuilder)
            {

            }
            protected StackBlockSpecBase(string tag, int index, BlockSpecTypes blockType, Func<TInput, TState, IStackEvents, IEmptyable, StackBlockBase> blockBuilder)
            {
                if (BlockType == BlockSpecTypes.Finally)
                    Tag = FINALLY_TAG;
                else
                    Tag = HandleOperationTagName(tag, index);

                Index = index;
                BlockType = blockType;
                this.blockBuilder = blockBuilder;

                if (blockType == BlockSpecTypes.Finally)
                    Tag = BlockSpecTypes.Finally.ToString();
                else if (Tag == BlockSpecTypes.Finally.ToString())
                    throw new OperationStackDeclarationException("Tag 'Finally' is reserved for Finally blocks");
            }
        }
    }
}
