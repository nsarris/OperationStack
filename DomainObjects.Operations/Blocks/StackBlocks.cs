using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlocks<TState, TOperationEvent> //: List<StackBlockSpecBase<TState, TOperationEvent>>
        where TOperationEvent : IOperationEvent
    {
        private List<StackBlockSpecBase<TState, TOperationEvent>> blocks = new List<StackBlockSpecBase<TState, TOperationEvent>>();
        private Dictionary<string, StackBlockSpecBase<TState, TOperationEvent>> blockDictionaryByTag;

        public StackBlocks()
        {
            
        }
        public StackBlocks(IEnumerable<StackBlockSpecBase<TState, TOperationEvent>> blocks)
        {
            this.blocks = blocks.ToList();
        }

        public StackBlocks(List<StackBlockSpecBase<TState, TOperationEvent>> blocks)
        {
            this.blocks = blocks;
        }

        public void Add(StackBlockSpecBase<TState, TOperationEvent> block) => blocks.Add(block);
        public int Count => blocks.Count;

        public IEnumerable<StackBlockSpecBase<TState, TOperationEvent>> Concat(StackBlockSpecBase<TState, TOperationEvent> block) 
            => blocks.Concat(new[] { block });
        

        public StackBlockSpecBase<TState, TOperationEvent> GetFirst() => blocks.FirstOrDefault();
        

        public StackBlockSpecBase<TState, TOperationEvent> GetByTag(string tag)
        {
            if (blockDictionaryByTag == null) blockDictionaryByTag = blocks.ToDictionary(x => x.Tag);

            if (!blockDictionaryByTag.TryGetValue(tag, out var block))
                throw new OperationStackExecutionException("Block with tag " + tag + " not found");
            return block;
        }

        public StackBlockSpecBase<TState, TOperationEvent> GetByIndex(int index)
        {
            if (index >= 0 && index < blocks.Count)
                return blocks[index];
            else
                throw new OperationStackExecutionException("Block with index " + index + " not found");
        }

        public StackBlockSpecBase<TState, TOperationEvent> GetByTagOrIndex(string tag, int index)
        {
            if (!string.IsNullOrEmpty(tag))
                return GetByTag(tag);
            else
                return GetByIndex(index);
        }

        public StackBlockSpecBase<TState, TOperationEvent> GetNext(int currentIndex)
        {
            var next = currentIndex + 1;
            if (next >= blocks.Count)
                return null;
            return blocks[next];
        }

        public StackBlockSpecBase<TState, TOperationEvent> SkipBy(int currentIndex, int skip)
        {
            var next = currentIndex + 1 + skip;
            if (next >= blocks.Count)
                return null;
            return blocks[next];
        }

        public StackBlockSpecBase<TState, TOperationEvent> GotoEnd(int currentIndex)
        {
            if (currentIndex == blocks.Count - 1)
                return null;
            else
                return blocks.Where(x => x.BlockType == BlockSpecTypes.Finally).FirstOrDefault();
        }

        private bool ContainsFinallyBlock() => blocks.Any(x => x.BlockType == BlockSpecTypes.Finally);
        
        public void AssertAddBlock(StackBlockSpecBase<TState, TOperationEvent> block)
        {
            if (ContainsFinallyBlock())
                throw new OperationStackDeclarationException("No block can be added after a Finally block");
        }
    }
}
