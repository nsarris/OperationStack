using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class StackBlocks
    {
        private readonly List<IStackBlockSpec> blocks = new List<IStackBlockSpec>();
        private Dictionary<string, IStackBlockSpec> blockDictionaryByTag;

        public StackBlocks()
        {

        }
        public StackBlocks(IEnumerable<IStackBlockSpec> blocks)
        {
            this.blocks = blocks.ToList();
        }

        public StackBlocks(List<IStackBlockSpec> blocks)
        {
            this.blocks = blocks;
        }

        public void Add(IStackBlockSpec block) => blocks.Add(block);
        public int Count => blocks.Count;

        public IEnumerable<IStackBlockSpec> Concat(IStackBlockSpec block)
            => blocks.Concat(new[] { block });


        public IStackBlockSpec GetFirst() => blocks.FirstOrDefault();


        public IStackBlockSpec GetByTag(string tag)
        {
            if (blockDictionaryByTag == null) blockDictionaryByTag = blocks.ToDictionary(x => x.Tag);

            if (!blockDictionaryByTag.TryGetValue(tag, out var block))
                throw new OperationStackExecutionException("Block with tag " + tag + " not found");
            return block;
        }

        public IStackBlockSpec GetByIndex(int index)
        {
            if (index >= 0 && index < blocks.Count)
                return blocks[index];
            else
                throw new OperationStackExecutionException("Block with index " + index + " not found");
        }

        public IStackBlockSpec GetByTagOrIndex(string tag, int index)
        {
            if (!string.IsNullOrEmpty(tag))
                return GetByTag(tag);
            else
                return GetByIndex(index);
        }

        public IStackBlockSpec GetNext(int currentIndex)
        {
            var next = currentIndex + 1;
            if (next >= blocks.Count)
                return null;
            return blocks[next];
        }

        public IStackBlockSpec SkipBy(int currentIndex, int skip)
        {
            var next = currentIndex + 1 + skip;
            if (next >= blocks.Count)
                return null;
            return blocks[next];
        }

        public IStackBlockSpec GotoEnd(int currentIndex)
        {
            if (currentIndex == blocks.Count - 1)
                return null;
            else
                return blocks.FirstOrDefault(x => x.BlockType == BlockSpecTypes.Finally);
        }

        private bool ContainsFinallyBlock() => blocks.Any(x => x.BlockType == BlockSpecTypes.Finally);

        public void AssertAddBlock(IStackBlockSpec block)
        {
            if (ContainsFinallyBlock())
                throw new OperationStackDeclarationException("No block can be added after a Finally block");
        }
    }
}
