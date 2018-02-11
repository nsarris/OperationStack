using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class ResultDispatcher<T, TState> : IResultDispatcher<T, TState>
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
                    FlowTarget = BlockFlowTarget.Break,
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
                    FlowTarget = BlockFlowTarget.End
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
                    FlowTarget = BlockFlowTarget.End,
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
                    FlowTarget = BlockFlowTarget.Goto,
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
                    FlowTarget = BlockFlowTarget.Goto,
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
                    FlowTarget = BlockFlowTarget.Reset
                }
            };
        }

        public BlockResult<T> Reset(TState state)
        {
            return new BlockResult<T>
            {
                Success = true,
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
                Success = true,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Restart
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
                    FlowTarget = BlockFlowTarget.Return,
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
                    FlowTarget = BlockFlowTarget.Return,
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
                    FlowTarget = BlockFlowTarget.Return,
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
