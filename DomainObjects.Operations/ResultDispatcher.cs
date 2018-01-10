using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class ResultVoidDispatcher : IResultVoidDispatcher
    {
        public BlockResultVoid Break(bool success)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Break
                }
            };
        }

        public BlockResultVoid End(bool success)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End
                }
            };
        }

        public BlockResultVoid End(bool success, object overrideResult)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End,
                    OverrideResult = Emptyable.Create(overrideResult)
                }
            };
        }

        public BlockResultVoid Goto(string tag, bool success)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag
                }
            };
        }

        public BlockResultVoid Goto(string tag)
        {
            return Goto(tag, true);
        }

        public BlockResultVoid Goto(string tag, object overrideInput, bool success = true)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResultVoid Goto(string tag, object overrideInput)
        {
            return Goto(tag, overrideInput, true);
        }

        public BlockResultVoid Reset()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Reset,
                    //State = null
                }
            };
        }

        public BlockResultVoid Reset(object state)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Reset,
                    State = state
                }
            };
        }

        public BlockResultVoid Restart()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Restart
                }
            };
        }

        public BlockResultVoid Return(bool success)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return
                }
            };
        }

        public BlockResultVoid Return()
        {
            return Return(true);
        }

        public BlockResultVoid Skip(int i, bool success = true)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Skip,
                    TargetIndex = i,
                }
            };
        }

        public BlockResultVoid Skip(int i)
        {
            return Skip(i, true);
        }

        public BlockResultVoid Skip(int i, object overrideInput, bool success = true)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Skip,
                    TargetIndex = i,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResultVoid Skip(int i, object overrideInput)
        {
            return Skip(i, overrideInput, true);
        }
    }

    internal class ResultDispatcher<T> : IResultDispatcher<T>
    {
        public ResultDispatcher()
        {

        }
        public BlockResult<T> Break(bool success)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Break,
                }
            };
        }

        public BlockResult<T> End(bool success)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End
                }
            };
        }

        public BlockResult<T> End(bool success, object overrideResult)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End,
                    OverrideResult = Emptyable.Create(overrideResult)
                }
            };
        }

        public BlockResult<T> Goto(string tag, bool success = true)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag
                }
            };
        }

        public BlockResult<T> Goto(string tag)
        {
            return Goto(tag, true);
        }

        public BlockResult<T> Goto(string tag, object overrideInput, bool success = true)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }
        public BlockResult<T> Goto(string tag, object overrideInput)
        {
            return Goto(tag, overrideInput, true);
        }

        public BlockResult<T> Reset()
        {
            return new BlockResult<T>
            {
                Success = true,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Reset
                }
            };
        }

        public BlockResult<T> Reset(object state)
        {
            return new BlockResult<T>
            {
                Success = true,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Reset,
                    State = state
                }
            };
        }

        public BlockResult<T> Restart()
        {
            return new BlockResult<T>
            {
                Success = true,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Restart
                }
            };
        }

        //public BlockResult<T> Return()
        //{
        //    return new BlockResult<T>
        //    {
        //        Target = new BlockResultTarget
        //        {
        //            FlowTarget = BlockFlowTargets.Return
        //        }
        //    };
        //}

        public BlockResult<T> Return(T result, bool success = true)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return,
                },
                Result = new Emptyable<T>(result)
            };
        }

        public BlockResult<T> Return(T result)
        {
            return Return(result, true);
        }

        public BlockResult<T> Skip(int i, bool success = true)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return,
                    TargetIndex = i,
                }
            };
        }

        public BlockResult<T> Skip(int i)
        {
            return Skip(i, true);
        }

        public BlockResult<T> Skip(int i, object overrideInput, bool success = true)
        {
            return new BlockResult<T>
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return,
                    TargetIndex = i,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResult<T> Skip(int i, object overrideInput)
        {
            return Skip(i, overrideInput, true);
        }
    }
}
