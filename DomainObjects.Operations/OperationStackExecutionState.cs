﻿using System;
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
        private IEmptyable OverrideResult { get; set; } = Emptyable.Empty;
        public bool IsFail { get; private set; }
        public List<BlockTraceResult<TOperationEvent>> StackTrace { get; } = new List<BlockTraceResult<TOperationEvent>>();
        public StackBlockSpecBase<TState, TOperationEvent> PreviousBlockSpec { get; private set; } 
        public StackBlockSpecBase<TState, TOperationEvent> CurrentBlockSpec { get; private set; }
        OperationStackOptions options;
        StackBlocks<TState, TOperationEvent> blocks;
        private bool GetSuccessState()
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

        /// <summary>
        /// Checks if input is compatible and address accordingly (Exception, Empty or Fail)
        /// </summary>
        /// <returns>False if stack in fail state and needs to end, true otherwise</returns>
        public bool AssertCurrentBlockInput()
        {
            if (CurrentBlockSpec.InputType != null)
            {
                try
                {
                    NextInput = NextInput.ConvertTo(CurrentBlockSpec.InputType);
                }
                catch (Exception e)
                {
                    if (options.IncompatibleInputTypeBehaviour == IncompatibleInputTypeBehaviour.Exception)
                        throw new OperationStackExecutionException("Invalid input coming from " + PreviousBlockSpec?.Tag ?? "unknwon" + " block and going into " + CurrentBlockSpec.Tag + " block. Exception was " + e.Message);
                    else if (options.IncompatibleInputTypeBehaviour == IncompatibleInputTypeBehaviour.AssumeEmpty)
                        NextInput = Emptyable.Empty;
                    else if (options.IncompatibleInputTypeBehaviour == IncompatibleInputTypeBehaviour.Fail)
                    {
                        IsFail = true;
                        var error = ExceptionErrorBuilder<TOperationEvent>.Build(new OperationStackExecutionException("Invalid input coming from " + PreviousBlockSpec?.Tag ?? "unknwon" + " block and going into " + CurrentBlockSpec.Tag + " block. Exception was " + e.Message));
                        StackTrace.Add(new BlockTraceResult<TOperationEvent>(CurrentBlockSpec.Tag, error, NextInput));
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Handle current block result and set next block to execute
        /// </summary>
        /// <param name="block">The execution block</param>
        /// <param name="blockResult">The execution block result</param>
        public void HandleBlockResultAndSetNext(StackBlockBase<TState, TOperationEvent> block, IBlockResult blockResult)
        {
            PreviousBlockSpec = CurrentBlockSpec;

            //Add to stack trace
            //Should we add trace to empty event blocks? To event blocks?
            StackTrace.Add(new BlockTraceResult<TOperationEvent>(block, blockResult));

            //Handle Reset state in case of reset
            State = blockResult.Target.FlowTarget == BlockFlowTarget.Reset ?
                    (blockResult.Target.ResetStateSet ? (TState)Convert.ChangeType(blockResult.Target.State,typeof(TState)): default(TState)) :
                    block.StackState;

            //Check fail state
            if (options.FailOnException && block.Events.HasUnhandledErrors)
                IsFail = true;

            //Override result is only applicable on Complete. Cache here in case of finally
            OverrideResult = blockResult.Target.OverrideResult;
            
            //Set next input
            NextInput = blockResult.GetNextInput();//Get target.OverrideInput.IsEmpty ? blockResult.Result : target.OverrideInput;
            
            //Get next block to execute
            CurrentBlockSpec = IsFail ? blocks.GotoEnd(CurrentBlockSpec.Index) : GetNext(CurrentBlockSpec, blockResult.Target);

            //If complete set overriden result if any
            if (CurrentBlockSpec == null && OverrideResult.HasValue) blockResult.OverrideResult(OverrideResult);
        }

        /// <summary>
        /// Gets then next block spec to execute
        /// </summary>
        /// <param name="currentBlock">The current block</param>
        /// <param name="target">The next target</param>
        /// <returns></returns>
        private StackBlockSpecBase<TState, TOperationEvent> GetNext(StackBlockSpecBase<TState, TOperationEvent> currentBlock,BlockResultTarget target)
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

        /// <summary>
        /// Get the operation stack result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isCommand">Command or query flag</param>
        /// <returns></returns>
        public IOperationResult<TState, TOperationEvent> GetResult<T>(bool isCommand)
        {
            return isCommand ?
                new CommandResult<TState, TOperationEvent>(GetSuccessState(), StackTrace, State) :
                new QueryResult<TState, TOperationEvent, T>(GetSuccessState(), StackTrace, State, LastResult.ConvertTo<T>());
        }
    }
}