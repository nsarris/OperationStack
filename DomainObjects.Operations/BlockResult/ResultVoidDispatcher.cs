namespace DomainObjects.Operations
{
    internal class ResultVoidDispatcher<TState> : IResultVoidDispatcher<TState>
    {
        public BlockResultVoid Break(bool success)
        {
            return new BlockResultVoid
            {
                Success = success,
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Break
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
                    FlowTarget = BlockFlowTarget.End
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
                    FlowTarget = BlockFlowTarget.End,
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
                    FlowTarget = BlockFlowTarget.Goto,
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
                    FlowTarget = BlockFlowTarget.Goto,
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
                    FlowTarget = BlockFlowTarget.Reset,
                    //State = null
                }
            };
        }

        public BlockResultVoid Reset(TState state)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Reset,
                    State = state,
                    ResetStateSet = true
                }
            };
        }

        public BlockResultVoid Restart()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Restart
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
                    FlowTarget = BlockFlowTarget.Return
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
                    FlowTarget = BlockFlowTarget.Skip,
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
                    FlowTarget = BlockFlowTarget.Skip,
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
}
