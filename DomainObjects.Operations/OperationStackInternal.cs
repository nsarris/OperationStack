using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class OperationStackInternal<TInput, TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        public OperationStackOptions Options { get; set; } = new OperationStackOptions();
        public Func<TState> InitialStateBuilder { get; set; } = () => default(TState);
        public StackBlocks<TInput, TState, TOperationEvent> Blocks { get; set; } = new StackBlocks<TInput, TState, TOperationEvent>();
        public int NextIndex => Blocks.Count;

        public OperationStack<TInput, TState, TOperationEvent> CreateNew(StackBlockSpecBase<TInput, TState, TOperationEvent> block)
        {
            Blocks.AssertAddBlock(block);
            return new OperationStack<TInput, TState, TOperationEvent>(Blocks.Concat(block), Options, InitialStateBuilder);
        }

        public OperationStack<TInput, TState, TOperationEvent, TResult> CreateNew<TResult>(StackBlockSpecBase<TInput, TState, TOperationEvent> block)
        {
            Blocks.AssertAddBlock(block);
            return new OperationStack<TInput, TState, TOperationEvent, TResult>(Blocks.Concat(block), Options, InitialStateBuilder);
        }


        private IOperationResult<TInput, TState, TOperationEvent> Execute<T>(bool isCommand, TInput input, TState initialState)
        {
            var executionState = new OperationStackExecutionState<TInput, TState, TOperationEvent>(Options, Blocks, input, initialState);

            while (executionState.CurrentBlockSpec != null)
            {
                if (!executionState.AssertCurrentBlockInput())
                    break;

                var block = executionState.CurrentBlockSpec.CreateBlock(input, executionState.State, new StackEvents<TOperationEvent>(executionState.StackTrace.SelectMany(x => x.FlattenedEvents)), executionState.NextInput);
                var blockResult = block.Execute(Options.TimeMeasurement);

                executionState.HandleBlockResultAndSetNext(block, blockResult);
            }
            return executionState.GetResult<T>(isCommand);
        }


        private async Task<IOperationResult<TInput, TState, TOperationEvent>> ExecuteAsync<T>(bool isCommand, TInput input, TState initialState)
        {
            var executionState = new OperationStackExecutionState<TInput, TState, TOperationEvent>(Options, Blocks, input, initialState);

            while (executionState.CurrentBlockSpec != null)
            {
                if (!executionState.AssertCurrentBlockInput())
                    break;

                var block = executionState.CurrentBlockSpec.CreateBlock(input, executionState.State, new StackEvents<TOperationEvent>(executionState.StackTrace.SelectMany(x => x.FlattenedEvents)), executionState.NextInput);
                var blockResult = await block.ExecuteAsync(Options.TimeMeasurement);

                executionState.HandleBlockResultAndSetNext(block, blockResult);

            }
            return executionState.GetResult<T>(isCommand);
        }


        public ICommandResult<TInput, TState, TOperationEvent> Execute(TInput input, TState initialState)
        {
            return (ICommandResult<TInput, TState, TOperationEvent>)Execute<object>(true, input, initialState);
        }

        public IQueryResult<TInput, TState, TOperationEvent, T> Execute<T>(TInput input, TState initialState)
        {
            return (IQueryResult<TInput, TState, TOperationEvent, T>)Execute<T>(false, input, initialState);
        }

        public async Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync(TInput input, TState initialState)
        {
            return (ICommandResult<TInput, TState, TOperationEvent>)(await ExecuteAsync<object>(true, input, initialState));
        }

        public async Task<IQueryResult<TInput, TState, TOperationEvent, T>> ExecuteAsync<T>(TInput input, TState initialState)
        {
            return (IQueryResult<TInput, TState, TOperationEvent, T>)(await ExecuteAsync<T>(false, input, initialState));
        }

        public ICommandResult<TInput, TState, TOperationEvent> Execute(TInput input)
        {
            return (ICommandResult<TInput, TState, TOperationEvent>)Execute<object>(true, input, InitialStateBuilder());
        }

        public IQueryResult<TInput, TState, TOperationEvent, T> Execute<T>(TInput input)
        {
            return (IQueryResult<TInput, TState, TOperationEvent, T>)Execute<T>(false, input, InitialStateBuilder());
        }

        public async Task<ICommandResult<TInput, TState, TOperationEvent>> ExecuteAsync(TInput input)
        {
            return (ICommandResult<TInput, TState, TOperationEvent>)(await ExecuteAsync<object>(true, input, InitialStateBuilder()));
        }

        public async Task<IQueryResult<TInput, TState, TOperationEvent, T>> ExecuteAsync<T>(TInput input)
        {
            return (IQueryResult<TInput, TState, TOperationEvent, T>)(await ExecuteAsync<T>(false, input, InitialStateBuilder()));
        }
    }
}
