using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class OperationStackInternal<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public OperationStackOptions Options { get; set; } = new OperationStackOptions();
        
        public List<StackBlockSpecBase<TState,TOperationEvent>> Blocks { get; set; } = new List<StackBlockSpecBase<TState,TOperationEvent>>();

        public int NextIndex => Blocks.Count;
        

        public OperationStack<TState, TOperationEvent> CreateNew(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            ValidateNewBlock(block); 
            return new OperationStack<TState, TOperationEvent>(Blocks.Concat(new[] { block }), Options);
        }

        public OperationStack<TState, TOperationEvent, TResult> CreateNew<TResult>(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            ValidateNewBlock(block);
            return new OperationStack<TState, TOperationEvent, TResult>(Blocks.Concat(new[] { block }), Options);
        }

        private void ValidateNewBlock(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            if (Blocks.Any(x => x.BlockType == BlockSpecTypes.Finally))
                throw new OperationStackDeclarationException("No block can be added after a Finally block");
            //if (Blocks.Any() && Blocks.Last().BlockType == BlockSpecTypes.UnhandledExceptionHandler && (block.BlockType !=BlockSpecTypes.Finally && block.BlockType != BlockSpecTypes.UnhandledExceptionHandler))
              //  throw new OperationStackDeclarationException("Only a Finally or another UnhandledExceptionsHand block can follow an UnhandledExceptions block");
        }

        private StackBlockSpecBase<TState,TOperationEvent> HandleBlockResultAndGetNext(StackBlockSpecBase<TState,TOperationEvent> blockSpec, List<BlockTraceResult<TOperationEvent>> stackTrace, StackBlockBase<TState,TOperationEvent> block, IBlockResult blockResult, ref IEmptyable input, ref IEmptyable result, ref TState state, TState initialState)
        {
            var target = ((BlockResultBase)blockResult).Target;

            if (Options.EndOnException && block.Events.HasUnhandledErrors)
                target = new BlockResultTarget { FlowTarget = BlockFlowTarget.End };

            var time = ((BlockResultBase)blockResult).ExecutionTime;

            result = target.OverrideResult.IsEmpty ? blockResult.Result : target.OverrideResult;

            stackTrace.Add(new BlockTraceResult<TOperationEvent>(block.Tag, input, blockResult.Result, block.Events, block.InnerStackTrace, time));

            input = target.OverrideInput.IsEmpty ? blockResult.Result : target.OverrideInput;

            return GetNext(blockSpec, target,ref state, initialState);
        }

        private IOperationResult<TState, TOperationEvent> ToResult<T>(bool isCommand, TState initialState)
        {

            var state = initialState;
            var input = Emptyable.Empty;
            var result = Emptyable.Empty;
            var success = false;
            var stackTrace = new List<BlockTraceResult<TOperationEvent>>();
            var stackEvents = new StackEvents<TOperationEvent>();

            var blockSpec = Blocks.FirstOrDefault();

            while (blockSpec != null)
            {
                //Check if input is correct type and exception - Optionally check if next input type matches when using override input
                var block = blockSpec.CreateBlock(state, stackEvents, input);

                if (!block.IsEmptyEventBlock)
                {
                    var blockResult = block.Execute(Options.TimeMeasurement);
                    stackEvents.AddRange(block.FlattenedEvents);
                    
                    state = block.StackState;
                    blockSpec = HandleBlockResultAndGetNext(blockSpec, stackTrace, block, blockResult, ref input, ref result, ref state, initialState);
                    success = blockResult.Success;
                }
                else
                    blockSpec = GetNext(blockSpec, new BlockResultTarget() { OverrideInput = input }, ref state, initialState);

                
            }

            return isCommand ? new CommandResult<TState, TOperationEvent>(success, stackTrace, state) : new QueryResult<TState, TOperationEvent,T>(success, stackTrace, state, result.ConvertTo<T>());
        }

        public ICommandResult<TState, TOperationEvent> ToResult(TState initialState)
        {
            return (ICommandResult<TState, TOperationEvent>)ToResult<object>(true, initialState);
        }

        public IQueryResult<TState, TOperationEvent, T> ToResult<T>(TState initialState)
        {
            return (IQueryResult<TState, TOperationEvent, T>)ToResult<T>(false, initialState);
        }


        private async Task<IOperationResult<TState, TOperationEvent>> ToResultAsync<T>(bool isCommand, TState initialState)
        {
            var stackTrace = new List<BlockTraceResult<TOperationEvent>>();

            var state = initialState;
            var input = Emptyable.Empty;
            var result = Emptyable.Empty;
            var success = false;
            var blockSpec = Blocks.FirstOrDefault();
            var stackEvents = new StackEvents<TOperationEvent>();

            while (blockSpec != null)
            {
                //Check if input is correct type and exception - Optionally check if next input type matches when using override input
                var block = blockSpec.CreateBlock(state, stackEvents, input);

                if (!block.IsEmptyEventBlock)
                {
                    //Check here for sync and configure await (no timemeasurement) for optimization
                    var blockResult = block.IsAsync ? await block.ExecuteAsync(Options.TimeMeasurement).ConfigureAwait(Options.TimeMeasurement) : block.Execute(Options.TimeMeasurement);
                    state = block.StackState;
                    stackEvents.AddRange(block.FlattenedEvents);
                    blockSpec = HandleBlockResultAndGetNext(blockSpec, stackTrace, block, blockResult, ref input, ref result, ref state, initialState);
                    success = blockResult.Success;
                }
                else
                    blockSpec = GetNext(blockSpec, new BlockResultTarget() { OverrideInput = input }, ref state, initialState);
                
            }

            return isCommand ? new CommandResult<TState, TOperationEvent>(success, stackTrace, state) : new QueryResult<TState, TOperationEvent, T>(success, stackTrace, state, result.ConvertTo<T>());
        }

        public async Task<ICommandResult<TState, TOperationEvent>> ToResultAsync(TState initialState)
        {
            return (ICommandResult<TState, TOperationEvent>)(await ToResultAsync<object>(true, initialState));
        }

        public async Task<IQueryResult<TState, TOperationEvent, T>> ToResultAsync<T>(TState initialState)
        {
            return (IQueryResult<TState, TOperationEvent, T>)(await ToResultAsync<T>(false,initialState));
        }

        public StackBlockSpecBase<TState,TOperationEvent> GetNext(StackBlockSpecBase<TState,TOperationEvent> currentBlock, BlockResultTarget target, ref TState state, TState initialState)
        {
            int next = -1;
            switch (target.FlowTarget)
            {
                case BlockFlowTarget.Return:
                    next = currentBlock.Index + 1;
                    break;
                case BlockFlowTarget.Break:
                    next = -1;
                    break;
                case BlockFlowTarget.Goto:
                    next = Blocks.Where(x => x.Tag == target.TargetTag).FirstOrDefault().Index;
                    break;
                case BlockFlowTarget.Retry:
                    return currentBlock;
                case BlockFlowTarget.Reset:
                    next = -1;
                    state = target.ResetStateSet ?(TState)target.State : initialState;
                    break;
                case BlockFlowTarget.Restart:
                    next = 0;
                    break;
                case BlockFlowTarget.Skip:
                    next = next + 1 + target.TargetIndex;
                    break;
                case BlockFlowTarget.End:
                    //var lastBlock = Blocks.Where(x => x.BlockType == BlockSpecTypes.UnhandledExceptionHandler || x.BlockType == BlockSpecTypes.Finally).FirstOrDefault();
                    var lastBlock = Blocks.Where(x => x.BlockType == BlockSpecTypes.Finally).FirstOrDefault();
                    if (currentBlock == Blocks.Last())
                        return null;
                    if (lastBlock != null)
                        next = lastBlock.Index;
                    break;
            }
            if (next < 0 || next >= Blocks.Count)
                return null;
            else
                return Blocks[next];
        }
    }
}
