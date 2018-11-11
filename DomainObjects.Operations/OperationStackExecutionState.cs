using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class OperationStackExecutionState
        {
            public TState InitialState { get; private set; }
            public TState State { get; private set; }
            public TInput StackInput { get; private set; }
            public IEmptyable NextInput { get; private set; } = Emptyable.Empty;
            public IEmptyable LastResult { get; private set; } = Emptyable.Empty;
            private IEmptyable OverrideResult { get; set; } = Emptyable.Empty;
            public bool IsFail { get; private set; }
            public List<BlockTraceResult> StackTrace { get; } = new List<BlockTraceResult>();
            public IStackBlockSpec PreviousBlockSpec { get; private set; }
            public IStackBlockSpec CurrentBlockSpec { get; private set; }

            private readonly OperationStackOptions options;
            private readonly StackBlocks blocks;

            private bool GetSuccessState()
            {
                return !IsFail && !StackTrace.SelectMany(x => x.FlattenedEvents).Any(x => !x.IsHandled && !x.IsSwallowed);
            }

            public OperationStackExecutionState(OperationStackOptions options, StackBlocks blocks, TInput stackInput, TState initialState)
            {
                this.options = options;
                this.blocks = blocks;
                StackInput = stackInput;
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
                            var error = ExceptionErrorBuilder.Build(new OperationStackExecutionException("Invalid input coming from " + PreviousBlockSpec?.Tag ?? "unknwon" + " block and going into " + CurrentBlockSpec.Tag + " block. Exception was " + e.Message));
                            StackTrace.Add(new BlockTraceResult(CurrentBlockSpec.Tag, error, NextInput));
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
            public void HandleBlockResultAndSetNext(IStackBlock block, IBlockResult blockResult)
            {
                PreviousBlockSpec = CurrentBlockSpec;

                //Add to stack trace
                //Should we add trace to empty event blocks? To event blocks?
                StackTrace.Add(new BlockTraceResult(block, blockResult));

                //Handle Reset state in case of reset
                State = blockResult.Target.FlowTarget == BlockFlowTarget.Reset ?
                        (blockResult.Target.ResetStateSet ? (TState)Convert.ChangeType(blockResult.Target.State, typeof(TState)) : default(TState)) :
                        (TState)block.StackState;

                //Check fail state
                if (options.FailOnException && block.Events.HasUnhandledErrors)
                    IsFail = true;

                //Override result is only applicable on Complete. Cache here in case of finally
                OverrideResult = blockResult.Target.OverrideResult;

                //Set next input
                NextInput = blockResult.GetNextInput();

                //Get next block to execute
                CurrentBlockSpec = IsFail ? blocks.GotoEnd(CurrentBlockSpec.Index) : GetNext(CurrentBlockSpec, blockResult.Target);

                //If complete set overriden result if any
                if (CurrentBlockSpec == null && OverrideResult.HasValue)
                {
                    //Set last result
                    LastResult = OverrideResult;
                    blockResult.OverrideResult(OverrideResult);
                }
                else
                {
                    //Set last result
                    LastResult = blockResult.Result;
                }
            }

            /// <summary>
            /// Gets then next block spec to execute
            /// </summary>
            /// <param name="currentBlock">The current block</param>
            /// <param name="target">The next target</param>
            /// <returns></returns>
            private IStackBlockSpec GetNext(IStackBlockSpec currentBlock, BlockResultTarget target)
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
            public IOperationResult<TInput, TState> GetResult<T>(bool isCommand)
            {
                return isCommand ?
                    new CommandResult<TInput, TState>(GetSuccessState(), StackTrace, StackInput, State) :
                    new QueryResult<TInput, TState, T>(GetSuccessState(), StackTrace, StackInput, State, LastResult.ConvertTo<T>());
            }
        }
    }
}
