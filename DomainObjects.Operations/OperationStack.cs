using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class OperationStackInternal<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public OperationStackOptions Options { get; set; } = new OperationStackOptions();
        public TState State { get; set; }
        public List<StackBlockSpecBase<TState,TOperationEvent>> Blocks { get; set; } = new List<StackBlockSpecBase<TState,TOperationEvent>>();

        public int NextIndex => Blocks.Count;
        

        public OperationStack<TState, TOperationEvent> CreateNew(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            ValidateNewBlock(block); 
            return new OperationStack<TState, TOperationEvent>(State, Blocks.Concat(new[] { block }), Options);
        }

        public OperationStack<TState, TOperationEvent, TResult> CreateNew<TResult>(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            ValidateNewBlock(block);
            return new OperationStack<TState, TOperationEvent, TResult>(State, Blocks.Concat(new[] { block }), Options);
        }

        private void ValidateNewBlock(StackBlockSpecBase<TState,TOperationEvent> block)
        {
            if (Blocks.Any(x => x.BlockType == BlockSpecTypes.Finally))
                throw new OperationStackDeclarationException("No block can be added after a Finally block");
            if (Blocks.Any() && Blocks.Last().BlockType == BlockSpecTypes.UnhandledExceptionHandler && (block.BlockType !=BlockSpecTypes.Finally && block.BlockType != BlockSpecTypes.UnhandledExceptionHandler))
                throw new OperationStackDeclarationException("Only a Finally or another UnhandledExceptionsHand block can follow an UnhandledExceptions block");
        }

        private StackBlockSpecBase<TState,TOperationEvent> HandleBlockResultAndGetNext(StackBlockSpecBase<TState,TOperationEvent> blockSpec, List<BlockTraceResult<TOperationEvent>> stackTrace, StackBlockBase<TState,TOperationEvent> block, IBlockResult blockResult, ref IEmptyable input, ref IEmptyable result)
        {
            State = block.StackState;

            var target = ((BlockResultBase)blockResult).Target;

            if (Options.EndOnException && block.Events.HasUnhandledException)
                target = new BlockResultTarget { FlowTarget = BlockFlowTargets.End };

            var time = ((BlockResultBase)blockResult).ExecutionTime;

            result = target.OverrideResult.IsEmpty ? blockResult.Result : target.OverrideResult;

            stackTrace.Add(new BlockTraceResult<TOperationEvent>(block.Tag, input, blockResult.Result, block.Events, block.InnerStackTrace, time));

            input = target.OverrideInput.IsEmpty ? blockResult.Result : target.OverrideInput;

            return GetNext(blockSpec, target);
        }

        private IOperationResult<TOperationEvent> ToResult<T>(bool isCommand)
        {

            var input = Emptyable.Empty;
            var result = Emptyable.Empty;
            var success = false;
            var stackTrace = new List<BlockTraceResult<TOperationEvent>>();

            var blockSpec = Blocks.FirstOrDefault();

            while (blockSpec != null)
            {
                //Check if input is correct type and exception - Optionally check if next input type matches when using override input
                var block = blockSpec.CreateBlock(State, new StackEvents<TOperationEvent>(stackTrace.SelectMany(x => x.Events)), input);

                if (!block.IsEmptyEventBlock)
                {
                    var blockResult = block.Execute(Options.TimeMeasurement);
                    blockSpec = HandleBlockResultAndGetNext(blockSpec, stackTrace, block, blockResult, ref input, ref result);
                    success = blockResult.Success;
                }
                else
                    blockSpec = GetNext(blockSpec, new BlockResultTarget() { OverrideInput = input });
            }

            return isCommand ? new CommandResult<TOperationEvent>(success, stackTrace) : new QueryResult<TOperationEvent,T>(success, stackTrace, result.ConvertTo<T>());
        }

        public ICommandResult<TOperationEvent> ToResult()
        {
            return (ICommandResult<TOperationEvent>)ToResult<object>(true);
        }

        public IQueryResult<TOperationEvent,T> ToResult<T>()
        {
            return (IQueryResult<TOperationEvent,T>)ToResult<T>(false);
        }


        private async Task<IOperationResult<TOperationEvent>> ToResultAsync<T>(bool isCommand)
        {
            var stackTrace = new List<BlockTraceResult<TOperationEvent>>();

            IEmptyable input = Emptyable.Empty;
            IEmptyable result = Emptyable.Empty;
            var success = false;
            var blockSpec = Blocks.FirstOrDefault();

            while (blockSpec != null)
            {
                //Check if input is correct type and exception - Optionally check if next input type matches when using override input
                var block = blockSpec.CreateBlock(State, new StackEvents<TOperationEvent>(stackTrace.SelectMany(x => x.Events)), input);

                if (!block.IsEmptyEventBlock)
                {
                    var blockResult = await block.ExecuteAsync(Options.TimeMeasurement);
                    blockSpec = HandleBlockResultAndGetNext(blockSpec, stackTrace, block, blockResult, ref input, ref result);
                    success = blockResult.Success;
                }
                else
                    blockSpec = GetNext(blockSpec, new BlockResultTarget() { OverrideInput = input });
                
            }

            return isCommand ? new CommandResult<TOperationEvent>(success, stackTrace) : new QueryResult<TOperationEvent,T>(success, stackTrace, result.ConvertTo<T>());
        }

        public async Task<ICommandResult<TOperationEvent>> ToResultAsync()
        {
            return (ICommandResult<TOperationEvent>)(await ToResultAsync<object>(true));
        }

        public async Task<IQueryResult<TOperationEvent,T>> ToResultAsync<T>()
        {
            return (IQueryResult<TOperationEvent,T>)(await ToResultAsync<T>(false));
        }

        public StackBlockSpecBase<TState,TOperationEvent> GetNext(StackBlockSpecBase<TState,TOperationEvent> currentBlock, BlockResultTarget target)
        {
            int next = -1;
            switch (target.FlowTarget)
            {
                case BlockFlowTargets.Return:
                    next = currentBlock.Index + 1;
                    break;
                case BlockFlowTargets.Break:
                    next = -1;
                    break;
                case BlockFlowTargets.Goto:
                    next = Blocks.Where(x => x.Tag == target.TargetTag).FirstOrDefault().Index;
                    break;
                case BlockFlowTargets.Retry:
                    return currentBlock;
                case BlockFlowTargets.Reset:
                    next = -1;
                    State = (TState)target.State;
                    break;
                case BlockFlowTargets.Restart:
                    next = 0;
                    break;
                case BlockFlowTargets.Skip:
                    next = next + 1 + target.TargetIndex;
                    break;
                case BlockFlowTargets.End:
                    var lastBlock = Blocks.Where(x => x.BlockType == BlockSpecTypes.UnhandledExceptionHandler || x.BlockType == BlockSpecTypes.Finally).FirstOrDefault();
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

    public class OperationStackOptions
    {
        public bool TimeMeasurement { get; set; } = true;
        public bool EndOnException { get; set; } = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TState">Type of the state of this operation stack</typeparam>
    /// <typeparam name="TState">Type of the event of this operation stack</typeparam>
    public class OperationStack<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        #region Fields and Props
        public OperationStackOptions Options => internalStack.Options;

        private OperationStackInternal<TState, TOperationEvent> internalStack = new OperationStackInternal<TState, TOperationEvent>();

        private StackBlockSpecBuilder<TState,TOperationEvent> blockSpecBuilder = new StackBlockSpecBuilder<TState, TOperationEvent>();
        #endregion Fields and Props

        #region Ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Initial state of the stack</param>
        public OperationStack(TState state)
        {
            internalStack.State = state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Initial state of the stack</param>
        /// <param name="options">Override the default options</param>
        public OperationStack(TState state, OperationStackOptions options)
            : this(state)
        {
            internalStack.State = state;
            internalStack.Options = options;
        }

        internal OperationStack(TState state, IEnumerable<StackBlockSpecBase<TState,TOperationEvent>> blocks, OperationStackOptions options)
            : this(state, options)
        {
            internalStack.Blocks = blocks.ToList();
        }

        #endregion Ctor

        #region Sync

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent> Then(Action<ICommand<TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// Add a query operation
        /// </summary>
        /// <remarks>
        /// A query operation is a function that returns a value
        /// </remarks>
        /// <param name="op">The query to be executed</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(Func<IQuery<TState, TOperationEvent>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command stack to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query stack to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent,T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent> ThenAppend(ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(IQueryResult<TOperationEvent,T> res)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="tag">Mark the block with a tag for reference</param>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        /// /// <returns></returns>
        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="tag">Mark the block with a tag for reference</param>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        /// /// <returns></returns>
        public OperationStack<TState, TOperationEvent> Then(string tag, Action<ICommand<TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// Add a query operation
        /// </summary>
        /// <remarks>
        /// A query operation is a function that returns a value
        /// </remarks>
        /// <param name="op">The query to be executed</param>
        /// <param name="tag">Mark the block with a tag for reference</param>
        /// <returns></returns>
        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(string tag, Func<IQuery<TState, TOperationEvent>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent,T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, IQueryResult<TOperationEvent,T> res)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, TOperationEvent> Finally(Action<ICommand<TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturn<T>(Func<IQuery<TState, TOperationEvent>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        #endregion Sync

        #region Async

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(Func<IQuery<TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent,T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturn<T>(string tag, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TState, TOperationEvent,T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState,TOperationEvent>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, T> ThenAppend<T>(string tag, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent,T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, TOperationEvent,T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> op)
        {
            return null;
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturn<T>(Func<IQuery<TState, TOperationEvent>, Task<BlockResult<T>>> op)
        {
            return null;
        }

        public OperationStack<TState, TOperationEvent, T> FinallyReturnOf<T>(Func<ITypedQuery<TState, TOperationEvent,T>, Task<BlockResult<T>>> op)
        {
            return null;
        }

        #endregion Async

        #region OnEvents

        public OperationStack<TState, TOperationEvent> OnEvents(Func<IEventsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnErrors(Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnExceptions(Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptions(Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }



        public OperationStack<TState, TOperationEvent> OnEventsWhere(Func<TOperationEvent, bool> filter, Func<IEventsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState, TOperationEvent>, BlockResultVoid> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, TOperationEvent>, BlockResultVoid> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }



        public OperationStack<TState, TOperationEvent> OnEvents(Action<IEventsHandler<TOperationEvent, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState, TOperationEvent>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent> OnErrors(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }
        
        public OperationStack<TState, TOperationEvent> OnExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }



        public OperationStack<TState, TOperationEvent> OnEventsWhere(Func<TOperationEvent, bool> filter, Action<IEventsHandler<TOperationEvent, TState, TOperationEvent>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState, TOperationEvent>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, TOperationEvent>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        #endregion

        #region Result

        public ICommandResult<TOperationEvent> ToResult()
        {
            return internalStack.ToResult();
        }

        public Task<ICommandResult<TOperationEvent>> ToResultAsync()
        {
            return internalStack.ToResultAsync();
        }

        #endregion Result
    }



    public class OperationStack<TState, TOperationEvent, T>
        where TOperationEvent : IOperationEvent
    {
        #region Fields and Props

        public OperationStackOptions Options => internalStack.Options;
        private OperationStackInternal<TState, TOperationEvent> internalStack = new OperationStackInternal<TState, TOperationEvent>();
        private StackBlockSpecBuilder<TState, TOperationEvent> stackBlockSpecBuilder = new StackBlockSpecBuilder<TState, TOperationEvent>();
        #endregion Fields and Props

        #region Ctor

        internal OperationStack(TState state, IEnumerable<StackBlockSpecBase<TState,TOperationEvent>> blocks, OperationStackOptions options)
        {
            internalStack.Blocks = blocks.ToList();
            internalStack.State = state;
            internalStack.Options = options;
        }

        #endregion Ctor

        #region Sync

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> Then(Action<ICommand<TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(Func<IStackBlock<TState, TOperationEvent,T>, IQueryResult<TOperationEvent,Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(IQueryResult<TOperationEvent,Tout> res)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> Then(string tag, Action<ICommand<TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TState, TOperationEvent,T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TState, TOperationEvent,T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TState, TOperationEvent,T>, IQueryResult<TOperationEvent,Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, ICommandResult<TOperationEvent> res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, IQueryResult<TOperationEvent,Tout> res)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }


        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, TOperationEvent> Finally(Action<ICommand<TState, TOperationEvent, T>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        #endregion Sync

        #region Async

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> Then(Func<ICommand<TState, TOperationEvent, T>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(Func<IStackBlock<TState, TOperationEvent,T>, Task<IQueryResult<TOperationEvent,Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> Then(string tag, Func<ICommand<TState, TOperationEvent, T>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TState, TOperationEvent,T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TState, TOperationEvent,T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent> ThenAppend(string tag, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, TOperationEvent, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, TOperationEvent, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TState, TOperationEvent, T>, Task<IQueryResult<TOperationEvent,Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, TOperationEvent,T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }


        public OperationStack<TState, TOperationEvent> Finally(Func<ICommand<TState, TOperationEvent, T>, Task<BlockResultVoid>> op)
        {
            return null;
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturn<Tout>(Func<IQuery<TState, TOperationEvent,T>, Task<BlockResult<Tout>>> op)
        {
            return null;
        }

        public OperationStack<TState, TOperationEvent, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TState, TOperationEvent,T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return null;
        }

        #endregion Sync

        #region OnEvents


        public OperationStack<TState, TOperationEvent,T> OnEvents(Func<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrors(Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptions(Func<IExceptionsErrorHandler<TOperationEvent,Exception,TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptions(Func<IExceptionsErrorHandler<TOperationEvent, Exception, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }






        public OperationStack<TState, TOperationEvent,T> OnEventsWhere(Func<TOperationEvent, bool> filter, Func<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState, TOperationEvent,T>, BlockResult<T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsWhere(Func<TOperationEvent, bool> filter, Func<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent,Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Func<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }



        public OperationStack<TState, TOperationEvent,T> OnEvents(Action<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState, TOperationEvent,T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrors(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState, TOperationEvent,T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptions(Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptions(Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }





        public OperationStack<TState, TOperationEvent,T> OnEventsWhere(Func<TOperationEvent, bool> filter, Action<IEventsHandler<TOperationEvent, TState, TOperationEvent,T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState, TOperationEvent,T>> op)
            where TEvent : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsWhere(Func<TOperationEvent, bool> filter, Action<IErrorsHandler<TOperationEvent, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, TOperationEvent,T>> handler)
            where TError : TOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,TException, TState, TOperationEvent,T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<TOperationEvent, Exception>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent,Exception, TState, TOperationEvent,T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState, TOperationEvent,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TOperationEvent, TException>, bool> filter, Action<IExceptionsErrorHandler<TOperationEvent, TException, TState, TOperationEvent,T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        #endregion

        #region Result

        public IQueryResult<TOperationEvent,T> ToResult()
        {
            return internalStack.ToResult<T>();
        }

        public Task<IQueryResult<TOperationEvent,T>> ToResultAsync()
        {
            return internalStack.ToResultAsync<T>();
        }

        #endregion Result
    }

    public class OperationStack : OperationStack<object, OperationEvent>
    {
        public OperationStack()
            : base(null)
        {

        }
        public OperationStack(OperationStackOptions options)
            : base(null, options)
        {

        }
        public OperationStack(object state, OperationStackOptions options)
            : base(state, options)
        {

        }
    }

    public class OperationStack<TOperationEvent> : OperationStack<object, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public OperationStack()
            : base(null)
        {

        }
        public OperationStack(OperationStackOptions options)
            : base(null, options)
        {

        }
        public OperationStack(object state, OperationStackOptions options)
            : base(state, options)
        {

        }
    }
}
