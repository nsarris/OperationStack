using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class ResultDispatcher<T, TState> : IResultDispatcher<T, TState>
    {
        public BlockResult<T> Fail()
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Fail,
                }
            };
        }

        public BlockResult<T> Complete()
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Complete
                }
            };
        }

        public BlockResult<T> Complete(object overrideResult)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Complete,
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
                    FlowTarget = BlockFlowTarget.Goto,
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
                    FlowTarget = BlockFlowTarget.Goto,
                    TargetTag = tag,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResult<T> Goto(int index)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Goto,
                    TargetIndex = index
                }
            };
        }


        public BlockResult<T> Goto(int index, object overrideInput)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Goto,
                    TargetIndex = index,
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
                    FlowTarget = BlockFlowTarget.Reset
                }
            };
        }

        public BlockResult<T> Reset(TState state)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Reset,
                    State = state
                }
            };
        }

        public BlockResult<T> Restart()
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Restart
                }
            };
        }

        public BlockResult<T> Retry()
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Retry,
                }
            };
        }

        public BlockResult<T> Retry(object overrideInput)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Retry,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResult<T> Return(T result)
        {
            return new BlockResult<T>(result)
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Return,
                },
                //Result = new Emptyable<T>(result)
            };
        }

        
        public BlockResult<T> Skip(int i)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Skip,
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
                    FlowTarget = BlockFlowTarget.Skip,
                    TargetIndex = i,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResult<T> Fail(IOperationEvent error)
        {
            return new BlockResult<T>
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Fail,
                    Error = error
                }
            };
        }
    }
}
