namespace DomainObjects.Operations
{
    internal class ResultVoidDispatcher<TState, TOperationEvent> : IResultVoidDispatcher<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        public BlockResultVoid Fail()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Fail
                }
            };
        }

        public BlockResultVoid Fail(TOperationEvent error)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Fail,
                    Error = error
                }
            };
        }

        public BlockResultVoid Complete()
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Complete
                }
            };
        }

        public BlockResultVoid Complete(object overrideResult)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Complete,
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
                    FlowTarget = BlockFlowTarget.Goto,
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
                    FlowTarget = BlockFlowTarget.Goto,
                    TargetTag = tag,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResultVoid Goto(int index)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Goto,
                    TargetIndex = index
                }
            };
        }



        public BlockResultVoid Goto(int index, object overrideInput)
        {
            return new BlockResultVoid
            {
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Goto,
                    TargetIndex = index,
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
                    FlowTarget = BlockFlowTarget.Reset,
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

        public BlockResultVoid Return()
        {
            return new BlockResultVoid
            {
                
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Return
                }
            };
        }

     

        public BlockResultVoid Skip(int i)
        {
            return new BlockResultVoid
            {
        
                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Skip,
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
                    FlowTarget = BlockFlowTarget.Skip,
                    TargetIndex = i,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }

        public BlockResultVoid Retry()
        {
            return new BlockResultVoid
            {

                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Retry,
                }
            };
        }

        public BlockResultVoid Retry(object overrideInput)
        {
            return new BlockResultVoid
            {

                Target = new BlockResultTarget
                {
                    FlowTarget = BlockFlowTarget.Retry,
                    OverrideInput = Emptyable.Create(overrideInput)
                }
            };
        }
    }
}
