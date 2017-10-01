using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class ResultVoidDispatcher : IResultVoidDispatcher
    {
        public BlockResultVoid Break()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Break
                }
            };
        }

        public BlockResultVoid End()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End
                }
            };
        }

        public BlockResultVoid End(object overrideResult)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End,
                    OverrideResult = Emptyable.Create(overrideResult)
                }
            };
        }

        public BlockResultVoid Goto(string tag)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag
                }
            };
        }

        public BlockResultVoid Goto(string tag, object overrideInput)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
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

        public BlockResultVoid Return()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return
                }
            };
        }

        public BlockResultVoid Skip(int i)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Skip,
                    TargetIndex = i,
                }
            };
        }

        public BlockResultVoid Skip(int i, object overrideInput)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Skip,
                    TargetIndex = i,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }
    }

    internal class ResultDispatcher<T> : IResultDispatcher<T>
    {
        public ResultDispatcher()
        {
            
        }
        public BlockResult<T> Break()
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Break
                }
            };
        }

        public BlockResult<T> End()
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End
                }
            };
        }

        public BlockResult<T> End(object overrideResult)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.End,
                    OverrideResult = Emptyable.Create(overrideResult)
                }
            };
        }

        public BlockResult<T> Goto(string tag)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag
                }
            };
        }

        public BlockResult<T> Goto(string tag, object overrideInput)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Goto,
                    TargetTag = tag,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResult<T> Reset()
        {
            return new BlockResult<T>
            {
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
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Reset,
                    State =  state
                }
            };
        }

        public BlockResult<T> Restart()
        {
            return new BlockResult<T>
            {
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

        public BlockResult<T> Return(T result)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return,
                },
                Result = new Emptyable<T>(result)
            };
        }

        

        public BlockResult<T> Skip(int i)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return,
                    TargetIndex = i,
                }
            };
        }

        public BlockResult<T> Skip(int i, object overrideInput)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTargets.Return,
                    TargetIndex = i,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }
    }
}
