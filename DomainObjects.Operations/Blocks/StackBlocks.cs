using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlocks<TInput, TState, TOperationEvent> //: List<StackBlockSpecBase<TInput,TState, TOperationEvent>>
        where TOperationEvent : OperationEvent
    {
        private List<StackBlockSpecBase<TInput, TState, TOperationEvent>> blocks = new List<StackBlockSpecBase<TInput, TState, TOperationEvent>>();
        private Dictionary<string, StackBlockSpecBase<TInput, TState, TOperationEvent>> blockDictionaryByTag;

        public StackBlocks()
        {
            
        }
        public StackBlocks(IEnumerable<StackBlockSpecBase<TInput, TState, TOperationEvent>> blocks)
        {
            this.blocks = blocks.ToList();
        }

        public StackBlocks(List<StackBlockSpecBase<TInput, TState, TOperationEvent>> blocks)
        {
            this.blocks = blocks;
        }

        public void Add(StackBlockSpecBase<TInput, TState, TOperationEvent> block) => blocks.Add(block);
        public int Count => blocks.Count;

        public IEnumerable<StackBlockSpecBase<TInput, TState, TOperationEvent>> Concat(StackBlockSpecBase<TInput, TState, TOperationEvent> block) 
            => blocks.Concat(new[] { block });
        

        public StackBlockSpecBase<TInput, TState, TOperationEvent> GetFirst() => blocks.FirstOrDefault();
        

        public StackBlockSpecBase<TInput, TState, TOperationEvent> GetByTag(string tag)
        {
            if (blockDictionaryByTag == null) blockDictionaryByTag = blocks.ToDictionary(x => x.Tag);

            if (!blockDictionaryByTag.TryGetValue(tag, out var block))
                throw new OperationStackExecutionException("Block with tag " + tag + " not found");
            return block;
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> GetByIndex(int index)
        {
            if (index >= 0 && index < blocks.Count)
                return blocks[index];
            else
                throw new OperationStackExecutionException("Block with index " + index + " not found");
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> GetByTagOrIndex(string tag, int index)
        {
            if (!string.IsNullOrEmpty(tag))
                return GetByTag(tag);
            else
                return GetByIndex(index);
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> GetNext(int currentIndex)
        {
            var next = currentIndex + 1;
            if (next >= blocks.Count)
                return null;
            return blocks[next];
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> SkipBy(int currentIndex, int skip)
        {
            var next = currentIndex + 1 + skip;
            if (next >= blocks.Count)
                return null;
            return blocks[next];
        }

        public StackBlockSpecBase<TInput, TState, TOperationEvent> GotoEnd(int currentIndex)
        {
            if (currentIndex == blocks.Count - 1)
                return null;
            else
                return blocks.Where(x => x.BlockType == BlockSpecTypes.Finally).FirstOrDefault();
        }

        private bool ContainsFinallyBlock() => blocks.Any(x => x.BlockType == BlockSpecTypes.Finally);
        
        public void AssertAddBlock(StackBlockSpecBase<TInput, TState, TOperationEvent> block)
        {
            if (ContainsFinallyBlock())
                throw new OperationStackDeclarationException("No block can be added after a Finally block");
        }
    }
}
