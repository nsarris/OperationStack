using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public interface IBlockResult
    {
        IEmptyable Result { get; }
    }

    public enum BlockFlowTargets
    {
        Return,
        End,
        Break,
        Reset,
        Restart,
        Goto,
        Skip,
        Retry
    }

    internal class BlockResultTarget
    {
        public BlockFlowTargets FlowTarget { get; set; }
        public string TargetTag { get; set; }
        public int TargetIndex { get; set; }
        public IEmptyable OverrideInput { get; set; } = Emptyable.Empty;
        public object State { get; set; }
        public IEmptyable OverrideResult { get; set; } = Emptyable.Empty;

        public int GetNext(List<IStackBlockSpec> blocks, IStackBlockSpec currentBlock)
        {
            int next = -1;
            switch (FlowTarget)
            {
                case BlockFlowTargets.Return:
                    next = currentBlock.Index + 1;
                    break;
                case BlockFlowTargets.Break:
                    next = -1;
                    break;
                case BlockFlowTargets.Goto:
                    next = blocks.Where(x => x.Tag == TargetTag).FirstOrDefault().Index;
                    break;
                case BlockFlowTargets.Retry:
                    return currentBlock.Index;
                case BlockFlowTargets.Reset:
                    next = -1;
                    State = State;
                    break;
                case BlockFlowTargets.Restart:
                    next = 0;
                    break;
                case BlockFlowTargets.Skip:
                    next = next + 1 + TargetIndex;
                    break;
                case BlockFlowTargets.End:
                    break;
            }
            if (next < 0 || next >= blocks.Count)
                return -1;
            else
                return next;
        }
    }

    public class BlockResultBase
    {
        internal BlockResultTarget Target { get; set; }
        internal ExecutionTime ExecutionTime { get; set; }
    }

    public class BlockResultVoid : BlockResultBase, IBlockResult
    {
        IEmptyable IBlockResult.Result => Emptyable.Empty;
    }

    public class BlockResult<T> : BlockResultBase, IBlockResult
    {
        public BlockResult()
        {
            Result = new Emptyable<T>();
        }
        public Emptyable<T> Result { get; set; }

        IEmptyable IBlockResult.Result => Result;
    }
}
