using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class OperationStackInternal<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
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
        

        private IOperationResult<TState, TOperationEvent> Execute<T>(bool isCommand, TState initialState)
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

        
        private async Task<IOperationResult<TState, TOperationEvent>> ExecuteAsync<T>(bool isCommand, TState initialState)
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

        
        public ICommandResult<TState, TOperationEvent> Execute(TState initialState)
        {
            return (ICommandResult<TState, TOperationEvent>)Execute<object>(true, initialState);
        }

        public IQueryResult<TState, TOperationEvent, T> Execute<T>(TState initialState)
        {
            return (IQueryResult<TState, TOperationEvent, T>)Execute<T>(false, initialState);
        }

        public async Task<ICommandResult<TState, TOperationEvent>> ExecuteAsync(TState initialState)
        {
            return (ICommandResult<TState, TOperationEvent>)(await ExecuteAsync<object>(true, initialState));
        }

        public async Task<IQueryResult<TState, TOperationEvent, T>> ExecuteAsync<T>(TState initialState)
        {
            return (IQueryResult<TState, TOperationEvent, T>)(await ExecuteAsync<T>(false,initialState));
        }
    }
}
