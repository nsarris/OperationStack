using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Operations
{
    internal class OperationStackExecutionState<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public TState InitialState { get; private set; }
        public TState State { get; private set; }
        public IEmptyable NextInput { get; private set; } = Emptyable.Empty;
        public IEmptyable LastResult { get; private set; } = Emptyable.Empty;
        public bool IsFail { get; private set; }
        public List<BlockTraceResult<TOperationEvent>> StackTrace { get; } = new List<BlockTraceResult<TOperationEvent>>();
        public StackBlockSpecBase<TState, TOperationEvent> CurrentBlockSpec { get; private set; }
        OperationStackOptions options;
        StackBlocks<TState, TOperationEvent> blocks;
        public bool GetSuccessState()
        {
            return !IsFail && !StackTrace.SelectMany(x => x.FlattenedEvents).Any(x => !x.IsHandled);
        }
        public OperationStackExecutionState(OperationStackOptions options, StackBlocks<TState, TOperationEvent> blocks, TState initialState)
        {
            this.options = options;
            this.blocks = blocks;
            CurrentBlockSpec = blocks.GetFirst();
            this.InitialState = initialState;
            State = initialState;
        }

        public void HandleBlockResultAndSetNext(StackBlockBase<TState, TOperationEvent> block, IBlockResult blockResult)
        {
            //Add to stack trace
            //Should we add trace to empty event blocks? To event blocks?
            StackTrace.Add(new BlockTraceResult<TOperationEvent>(block, blockResult));

            //Handle Reset state in case of reset
            State = blockResult.Target.FlowTarget == BlockFlowTarget.Reset ?
                    (blockResult.Target.ResetStateSet ? (TState)Convert.ChangeType(blockResult.Target.State,typeof(TState)): default(TState)) :
                    block.StackState;

            //Check fail state
            if (options.EndOnException && block.Events.HasUnhandledErrors)
                IsFail = true;

            //Override input is only applicable on Complete. What happens when finally exists?
            //result = target.OverrideResult.IsEmpty ? blockResult.Result : target.OverrideResult;

            //Set next input
            NextInput = blockResult.GetNextInput();//Get target.OverrideInput.IsEmpty ? blockResult.Result : target.OverrideInput;

            CurrentBlockSpec = IsFail ? blocks.GotoEnd(CurrentBlockSpec.Index) : GetNext(CurrentBlockSpec, blockResult.Target);
        }

        private StackBlockSpecBase<TState, TOperationEvent> GetNext(StackBlockSpecBase<TState, TOperationEvent> currentBlock, BlockResultTarget target)
        {
            switch (target.FlowTarget)
            {
                case BlockFlowTarget.Return:
                    return blocks.GetNext(currentBlock.Index);
                case BlockFlowTarget.Goto:
                    return blocks.GetByTagOrIndex(target.TargetTag, target.TargetIndex);
                case BlockFlowTarget.Retry:
                    return currentBlock;
                case BlockFlowTarget.Reset:
                    return blocks.GetFirst();
                case BlockFlowTarget.Restart:
                    return blocks.GetFirst();
                case BlockFlowTarget.Skip:
                    return blocks.SkipBy(currentBlock.Index, target.TargetIndex);
                case BlockFlowTarget.Complete:
                case BlockFlowTarget.Fail:
                    return blocks.GotoEnd(currentBlock.Index);
                default:
                    return null;
            }
        }

        public IOperationResult<TState, TOperationEvent> GetResult<T>(bool isCommand)
        {
            return isCommand ?
                new CommandResult<TState, TOperationEvent>(GetSuccessState(), StackTrace, State) :
                new QueryResult<TState, TOperationEvent, T>(GetSuccessState(), StackTrace, State, LastResult.ConvertTo<T>());
        }
    }
}
