using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class OperationStackInternal
        {
            internal OperationStackInternal(OperationStackOptions options, Func<TState> initialStateBuilder, bool hasInput)
            {
                Options = options;
                InitialStateBuilder = initialStateBuilder;
                HasInput = hasInput;
            }

            internal OperationStackInternal(OperationStackOptions options, Func<TState> initialStateBuilder, bool hasInput, StackBlocks blocks)
                : this(options, initialStateBuilder, hasInput)
            {
                Blocks = blocks;
            }
            public bool HasInput { get; private set; }
            public bool HasState => InitialStateBuilder != null;
            public OperationStackOptions Options { get; private set; }
            public Func<TState> InitialStateBuilder { get; private set; }
            public StackBlocks Blocks { get; private set; } = new StackBlocks();
            public int NextIndex => Blocks.Count;

            public OperationStack<TInput, TState> CreateNew(IStackBlockSpec block)
            {
                Blocks.AssertAddBlock(block);
                return new OperationStack<TInput, TState>(Blocks.Concat(block), Options, InitialStateBuilder, HasInput);
            }

            public OperationStack<TInput, TState, TResult> CreateNew<TResult>(IStackBlockSpec block)
            {
                Blocks.AssertAddBlock(block);
                return new OperationStack<TInput, TState, TResult>(Blocks.Concat(block), Options, InitialStateBuilder, HasInput);
            }

            public void AssertInput()
            {
                if (HasInput)
                    throw new InvalidOperationException("OperationStack as declared with Input of type " + typeof(TInput).Name + ". Please use an overload of Execute with input parameter.");
            }


            private IOperationResult<TInput, TState> Execute<T>(bool isCommand, TInput input, TState initialState)
            {
                var executionState = new OperationStackExecutionState(Options, Blocks, input, initialState);

                while (executionState.CurrentBlockSpec != null)
                {
                    if (!executionState.AssertCurrentBlockInput())
                        break;

                    var block = ((IStackBlockSpec<TInput, TState>)executionState.CurrentBlockSpec).CreateBlock(input, executionState.State, new StackEvents(executionState.StackTrace.SelectMany(x => x.FlattenedEvents)), executionState.NextInput);
                    var blockResult = block.Execute(Options.TimeMeasurement);

                    executionState.HandleBlockResultAndSetNext(block, blockResult);
                }
                return executionState.GetResult<T>(isCommand);
            }


            private async Task<IOperationResult<TInput, TState>> ExecuteAsync<T>(bool isCommand, TInput input, TState initialState)
            {
                var executionState = new OperationStackExecutionState(Options, Blocks, input, initialState);

                while (executionState.CurrentBlockSpec != null)
                {
                    if (!executionState.AssertCurrentBlockInput())
                        break;

                    var block = ((StackBlockSpecBase<TInput, T>)executionState.CurrentBlockSpec).CreateBlock(input, executionState.State, new StackEvents(executionState.StackTrace.SelectMany(x => x.FlattenedEvents)), executionState.NextInput);
                    var blockResult = await block.ExecuteAsync(Options.TimeMeasurement);

                    executionState.HandleBlockResultAndSetNext(block, blockResult);

                }
                return executionState.GetResult<T>(isCommand);
            }


            public ICommandResult<TInput, TState> Execute(TInput input, TState initialState)
            {
                return (ICommandResult<TInput, TState>)Execute<object>(true, input, initialState);
            }

            public IQueryResult<TInput, TState, T> Execute<T>(TInput input, TState initialState)
            {
                return (IQueryResult<TInput, TState, T>)Execute<T>(false, input, initialState);
            }

            public async Task<ICommandResult<TInput, TState>> ExecuteAsync(TInput input, TState initialState)
            {
                return (ICommandResult<TInput, TState>)(await ExecuteAsync<object>(true, input, initialState));
            }

            public async Task<IQueryResult<TInput, TState, T>> ExecuteAsync<T>(TInput input, TState initialState)
            {
                return (IQueryResult<TInput, TState, T>)(await ExecuteAsync<T>(false, input, initialState));
            }

            public ICommandResult<TInput, TState> Execute(TInput input)
            {
                return (ICommandResult<TInput, TState>)Execute<object>(true, input, InitialStateBuilder());
            }

            public IQueryResult<TInput, TState, T> Execute<T>(TInput input)
            {
                return (IQueryResult<TInput, TState, T>)Execute<T>(false, input, InitialStateBuilder());
            }

            public async Task<ICommandResult<TInput, TState>> ExecuteAsync(TInput input)
            {
                return (ICommandResult<TInput, TState>)(await ExecuteAsync<object>(true, input, InitialStateBuilder()));
            }

            public async Task<IQueryResult<TInput, TState, T>> ExecuteAsync<T>(TInput input)
            {
                return (IQueryResult<TInput, TState, T>)(await ExecuteAsync<T>(false, input, InitialStateBuilder()));
            }
        }
    }
}
