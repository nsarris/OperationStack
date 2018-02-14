using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class OperationStackInternal<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public OperationStackOptions Options { get; set; } = new OperationStackOptions();
        public StackBlocks<TState,TOperationEvent> Blocks { get; set; } = new StackBlocks<TState, TOperationEvent>();
        public int NextIndex => Blocks.Count;
        
        public OperationStack<TState, TOperationEvent> CreateNew(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            Blocks.AssertAddBlock(block); 
            return new OperationStack<TState, TOperationEvent>(Blocks.Concat(block), Options);
        }

        public OperationStack<TState, TOperationEvent, TResult> CreateNew<TResult>(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            Blocks.AssertAddBlock(block);
            return new OperationStack<TState, TOperationEvent, TResult>(Blocks.Concat(block), Options);
        }
        

        private IOperationResult<TState, TOperationEvent> ToResult<T>(bool isCommand, TState initialState)
        {
            var executionState = new OperationStackExecutionState<TState,TOperationEvent>(Options, Blocks, initialState);
            
            while (executionState.CurrentBlockSpec != null)
            {
                if (!executionState.AssertCurrentBlockInput())
                    break;

                var block = executionState.CurrentBlockSpec.CreateBlock(executionState.State, new StackEvents<TOperationEvent>(executionState.StackTrace.SelectMany(x => x.FlattenedEvents)), executionState.NextInput);
                var blockResult = block.Execute(Options.TimeMeasurement);

                executionState.HandleBlockResultAndSetNext(block, blockResult);
            }
            return executionState.GetResult<T>(isCommand);
        }

        
        private async Task<IOperationResult<TState, TOperationEvent>> ToResultAsync<T>(bool isCommand, TState initialState)
        {
            var executionState = new OperationStackExecutionState<TState, TOperationEvent>(Options, Blocks, initialState);

            while (executionState.CurrentBlockSpec != null)
            {
                if (!executionState.AssertCurrentBlockInput())
                    break;
                
                var block = executionState.CurrentBlockSpec.CreateBlock(executionState.State, new StackEvents<TOperationEvent>(executionState.StackTrace.SelectMany(x => x.FlattenedEvents)), executionState.NextInput);
                var blockResult = await block.ExecuteAsync(Options.TimeMeasurement);

                executionState.HandleBlockResultAndSetNext(block, blockResult);
                
            }
            return executionState.GetResult<T>(isCommand);
        }

        
        public ICommandResult<TState, TOperationEvent> ToResult(TState initialState)
        {
            return (ICommandResult<TState, TOperationEvent>)ToResult<object>(true, initialState);
        }

        public IQueryResult<TState, TOperationEvent, T> ToResult<T>(TState initialState)
        {
            return (IQueryResult<TState, TOperationEvent, T>)ToResult<T>(false, initialState);
        }

        public async Task<ICommandResult<TState, TOperationEvent>> ToResultAsync(TState initialState)
        {
            return (ICommandResult<TState, TOperationEvent>)(await ToResultAsync<object>(true, initialState));
        }

        public async Task<IQueryResult<TState, TOperationEvent, T>> ToResultAsync<T>(TState initialState)
        {
            return (IQueryResult<TState, TOperationEvent, T>)(await ToResultAsync<T>(false,initialState));
        }
    }
}
