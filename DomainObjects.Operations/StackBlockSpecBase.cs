using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal interface IStackBlockSpec
    {
        string Tag { get; }
        int Index { get; }
        BlockSpecTypes BlockType { get; }
    }

    internal enum BlockSpecTypes
    {
        Operation,
        Finally,
        UnhandledExceptionHandler,
        EventsHandler
    }

    internal abstract class StackBlockSpecBase<TState> : IStackBlockSpec
    {
        public string Tag { get; private set; }
        public int Index { get; private set; }
        public BlockSpecTypes BlockType { get; private set; }
        public abstract StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input);

        public StackBlockSpecBase(int index, BlockSpecTypes blockType)
            : this("Block " + index, index, blockType)
        {
        }
        public StackBlockSpecBase(string tag, int index, BlockSpecTypes blockType)
        {
            Tag = string.IsNullOrEmpty(tag) ? "Block " + index : tag;
            Index = index;
            BlockType = blockType;

            if (blockType == BlockSpecTypes.Finally)
                Tag = BlockSpecTypes.Finally.ToString();
            else if (Tag == BlockSpecTypes.Finally.ToString())
                throw new OperationStackDeclarationException("Tag 'Finally' is reserved for Finally blocks");
        }
    }

    

   
}
