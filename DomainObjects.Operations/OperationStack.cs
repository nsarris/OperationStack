using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class OperationStackInternal<TState>
    {
        public OperationStackOptions Options { get; set; } = new OperationStackOptions();
        public TState State { get; set; }
        public List<StackBlockSpecBase<TState>> Blocks { get; set; } = new List<StackBlockSpecBase<TState>>();

        public int NextIndex => Blocks.Count;

        public OperationStack<TState> CreateNew(StackBlockSpecBase<TState> block)
        {
            ValidateNewBlock(block); 
            return new OperationStack<TState>(State, Blocks.Concat(new[] { block }), Options);
        }

        public OperationStack<TState, TResult> CreateNew<TResult>(StackBlockSpecBase<TState> block)
        {
            ValidateNewBlock(block);
            return new OperationStack<TState, TResult>(State, Blocks.Concat(new[] { block }), Options);
        }

        private void ValidateNewBlock(StackBlockSpecBase<TState> block)
        {
            if (Blocks.Any(x => x.BlockType == BlockSpecTypes.Finally))
                throw new OperationStackDeclarationException("No block can be added after a Finally block");
            if (Blocks.Any() && Blocks.Last().BlockType == BlockSpecTypes.UnhandledExceptionHandler && (block.BlockType !=BlockSpecTypes.Finally && block.BlockType != BlockSpecTypes.UnhandledExceptionHandler))
                throw new OperationStackDeclarationException("Only a Finally or another UnhandledExceptionsHand block can follow an UnhandledExceptions block");
        }

        private StackBlockSpecBase<TState> HandleBlockResultAndGetNext(StackBlockSpecBase<TState> blockSpec, List<BlockTraceResult> stackTrace, StackBlockBase<TState> block, IBlockResult blockResult, ref IEmptyable input, ref IEmptyable result)
        {
            State = block.StackState;

            var target = ((BlockResultBase)blockResult).Target;

            if (Options.EndOnException && block.Events.HasUnhandledException)
                target = new BlockResultTarget { FlowTarget = BlockFlowTargets.End };

            var time = ((BlockResultBase)blockResult).ExecutionTime;

            result = target.OverrideResult.IsEmpty ? blockResult.Result : target.OverrideResult;

            stackTrace.Add(new BlockTraceResult(block.Tag, input, blockResult.Result, block.Events, block.InnerStackTrace, time));

            input = target.OverrideInput.IsEmpty ? blockResult.Result : target.OverrideInput;

            return GetNext(blockSpec, target);
        }

        private IOperationResult ToResult<T>(bool isCommand)
        {

            var input = Emptyable.Empty;
            var result = Emptyable.Empty;
            var success = false;
            var stackTrace = new List<BlockTraceResult>();

            var blockSpec = Blocks.FirstOrDefault();

            while (blockSpec != null)
            {
                //Check if input is correct type and exception - Optionally check if next input type matches when using override input
                var block = blockSpec.CreateBlock(State, new StackEvents(stackTrace.SelectMany(x => x.Events)), input);

                if (!block.IsEmptyEventBlock)
                {
                    var blockResult = block.Execute(Options.TimeMeasurement);
                    blockSpec = HandleBlockResultAndGetNext(blockSpec, stackTrace, block, blockResult, ref input, ref result);
                    success = blockResult.Success;
                }
                else
                    blockSpec = GetNext(blockSpec, new BlockResultTarget() { OverrideInput = input });
            }

            return isCommand ? new CommandResult(success, stackTrace) : new QueryResult<T>(success, stackTrace, result.ConvertTo<T>());
        }

        public ICommandResult ToResult()
        {
            return (ICommandResult)ToResult<object>(true);
        }

        public IQueryResult<T> ToResult<T>()
        {
            return (IQueryResult<T>)ToResult<T>(false);
        }


        private async Task<IOperationResult> ToResultAsync<T>(bool isCommand)
        {
            var stackTrace = new List<BlockTraceResult>();

            IEmptyable input = Emptyable.Empty;
            IEmptyable result = Emptyable.Empty;
            var success = false;
            var blockSpec = Blocks.FirstOrDefault();

            while (blockSpec != null)
            {
                //Check if input is correct type and exception - Optionally check if next input type matches when using override input
                var block = blockSpec.CreateBlock(State, new StackEvents(stackTrace.SelectMany(x => x.Events)), input);

                if (!block.IsEmptyEventBlock)
                {
                    var blockResult = await block.ExecuteAsync(Options.TimeMeasurement);
                    blockSpec = HandleBlockResultAndGetNext(blockSpec, stackTrace, block, blockResult, ref input, ref result);
                    success = blockResult.Success;
                }
                else
                    blockSpec = GetNext(blockSpec, new BlockResultTarget() { OverrideInput = input });
                
            }

            return isCommand ? new CommandResult(success, stackTrace) : new QueryResult<T>(success, stackTrace, result.ConvertTo<T>());
        }

        public async Task<ICommandResult> ToResultAsync()
        {
            return (ICommandResult)(await ToResultAsync<object>(true));
        }

        public async Task<IQueryResult<T>> ToResultAsync<T>()
        {
            return (IQueryResult<T>)(await ToResultAsync<T>(false));
        }

        public StackBlockSpecBase<TState> GetNext(StackBlockSpecBase<TState> currentBlock, BlockResultTarget target)
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
    public class OperationStack<TState>
    {
        #region Fields and Props
        public OperationStackOptions Options => internalStack.Options;

        private OperationStackInternal<TState> internalStack = new OperationStackInternal<TState>();

        private StackBlockSpecBuilder<TState> blockSpecBuilder = new StackBlockSpecBuilder<TState>();
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

        internal OperationStack(TState state, IEnumerable<StackBlockSpecBase<TState>> blocks, OperationStackOptions options)
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
        public OperationStack<TState> Then(Func<ICommand<TState>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// Add a command operation
        /// </summary>
        /// <remarks>
        /// A command operation is a function that doesn't return a value
        /// </remarks>
        /// <param name="op">The command to be executed</param>
        /// <returns></returns>
        public OperationStack<TState> Then(Action<ICommand<TState>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// Add a query operation
        /// </summary>
        /// <remarks>
        /// A query operation is a function that returns a value
        /// </remarks>
        /// <param name="op">The query to be executed</param>
        /// <returns></returns>
        public OperationStack<TState, T> ThenReturn<T>(Func<IQuery<TState>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenReturnOf<T>(Func<ITypedQuery<TState, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command stack to append</param>
        /// <returns></returns>
        public OperationStack<TState> ThenAppend(Func<IOperationBlock<TState>, ICommandResult> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query stack to append</param>
        /// <returns></returns>
        public OperationStack<TState, T> ThenAppend<T>(Func<IOperationBlock<TState>, IQueryResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External command's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TState> ThenAppend(ICommandResult res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op">External query's stack result to append</param>
        /// <returns></returns>
        public OperationStack<TState, T> ThenAppend<T>(IQueryResult<T> res)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
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
        public OperationStack<TState> Then(string tag, Func<ICommand<TState>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
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
        public OperationStack<TState> Then(string tag, Action<ICommand<TState>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
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
        public OperationStack<TState, T> ThenReturn<T>(string tag, Func<IQuery<TState>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TState, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(string tag, Func<IOperationBlock<TState>, ICommandResult> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenAppend<T>(string tag, Func<IOperationBlock<TState>, IQueryResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(string tag, ICommandResult res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenAppend<T>(string tag, IQueryResult<T> res)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }



        public OperationStack<TState> Finally(Func<ICommand<TState>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState> Finally(Action<ICommand<TState>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, T> FinallyReturn<T>(Func<IQuery<TState>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, T> FinallyReturnOf<T>(Func<ITypedQuery<TState, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        #endregion Sync

        #region Async

        public OperationStack<TState> Then(Func<ICommand<TState>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> Then(Func<ICommand<TState>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenReturn<T>(Func<IQuery<TState>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenReturnOf<T>(Func<ITypedQuery<TState, T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(Func<IOperationBlock<TState>, Task<ICommandResult>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenAppend<T>(Func<IOperationBlock<TState>, Task<IQueryResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState> Then(string tag, Func<ICommand<TState>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> Then(string tag, Func<ICommand<TState>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenReturn<T>(string tag, Func<IQuery<TState>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenReturnOf<T>(string tag, Func<ITypedQuery<TState, T>, Task<BlockResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(string tag, Func<IOperationBlock<TState>, Task<ICommandResult>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, T> ThenAppend<T>(string tag, Func<IOperationBlock<TState>, Task<IQueryResult<T>>> op)
        {
            return internalStack.CreateNew<T>(new StackBlockSpecQuery<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState> Finally(Func<ICommand<TState>, Task<BlockResultVoid>> op)
        {
            return null;
        }

        public OperationStack<TState, T> FinallyReturn<T>(Func<IQuery<TState>, Task<BlockResult<T>>> op)
        {
            return null;
        }

        public OperationStack<TState, T> FinallyReturnOf<T>(Func<ITypedQuery<TState, T>, Task<BlockResult<T>>> op)
        {
            return null;
        }

        #endregion Async

        #region OnEvents

        public OperationStack<TState> OnEvents(Func<IEventsHandler<IOperationEvent, TState>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState>, BlockResultVoid> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState> OnErrors(Func<IErrorsHandler<IOperationError, TState>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState>, BlockResultVoid> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnExceptions(Func<IExceptionsErrorHandler<Exception, TState>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TException, TState>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnUnhandledExceptions(Func<IExceptionsErrorHandler<Exception, TState>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TException, TState>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }



        public OperationStack<TState> OnEventsWhere(Func<IOperationEvent, bool> filter, Func<IEventsHandler<IOperationEvent, TState>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState>, BlockResultVoid> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState> OnErrorsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState>, BlockResultVoid> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnExceptionsWhere(Func<IOperationError, bool> filter, Func<IExceptionsErrorHandler<Exception, TState>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionsErrorHandler<TException, TState>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<Exception>, bool> filter, Func<IExceptionsErrorHandler<Exception, TState>, BlockResultVoid> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionsErrorHandler<TException, TState>, BlockResultVoid> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }



        public OperationStack<TState> OnEvents(Action<IEventsHandler<IOperationEvent, TState>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState>> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState> OnErrors(Action<IErrorsHandler<IOperationError, TState>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState>> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }
        
        public OperationStack<TState> OnExceptions(Action<IExceptionsErrorHandler<Exception, TState>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnUnhandledExceptions(Action<IExceptionsErrorHandler<Exception, TState>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler));
        }



        public OperationStack<TState> OnEventsWhere(Func<IOperationEvent, bool> filter, Action<IEventsHandler<IOperationEvent, TState>> op)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState>> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState> OnErrorsWhere(Func<IOperationError, bool> filter, Action<IErrorsHandler<IOperationError, TState>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState>> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnExceptionsWhere(Func<IOperationError, bool> filter, Action<IExceptionsErrorHandler<Exception, TState>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionsErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<Exception>, bool> filter, Action<IExceptionsErrorHandler<Exception, TState>> handler)
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionsErrorHandler<TException, TState>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew(blockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        #endregion

        #region Result

        public ICommandResult ToResult()
        {
            return internalStack.ToResult();
        }

        public Task<ICommandResult> ToResultAsync()
        {
            return internalStack.ToResultAsync();
        }

        #endregion Result
    }



    public class OperationStack<TState, T>
    {
        #region Fields and Props

        public OperationStackOptions Options => internalStack.Options;
        private OperationStackInternal<TState> internalStack = new OperationStackInternal<TState>();
        private StackBlockSpecBuilder<TState> stackBlockSpecBuilder = new StackBlockSpecBuilder<TState>();
        #endregion Fields and Props

        #region Ctor

        internal OperationStack(TState state, IEnumerable<StackBlockSpecBase<TState>> blocks, OperationStackOptions options)
        {
            internalStack.Blocks = blocks.ToList();
            internalStack.State = state;
            internalStack.Options = options;
        }

        #endregion Ctor

        #region Sync

        public OperationStack<TState> Then(Func<ICommand<TState, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> Then(Action<ICommand<TState, T>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, Tout> ThenReturn<Tout>(Func<IQuery<TState, T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TState, T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(Func<IOperationBlock<TState>, ICommandResult> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenAppend<Tout>(Func<IStackBlock<TState, T>, IQueryResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(ICommandResult res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenAppend<Tout>(IQueryResult<Tout> res)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }



        public OperationStack<TState> Then(string tag, Func<ICommand<TState, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> Then(string tag, Action<ICommand<TState, T>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TState, T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TState, T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(string tag, Func<IOperationBlock<TState>, ICommandResult> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TState, T>, IQueryResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(string tag, ICommandResult res)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenAppend<Tout>(string tag, IQueryResult<Tout> res)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(tag, internalStack.NextIndex, (op) => res, BlockSpecTypes.Operation));
        }


        public OperationStack<TState> Finally(Func<ICommand<TState, T>, BlockResultVoid> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState> Finally(Action<ICommand<TState, T>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, Tout> FinallyReturn<Tout>(Func<IQuery<TState, T>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        public OperationStack<TState, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TState, T, Tout>, BlockResult<Tout>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Finally));
        }

        #endregion Sync

        #region Sync

        public OperationStack<TState> Then(Func<ICommand<TState, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> Then(Func<ICommand<TState, T>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState, Tout> ThenReturn<Tout>(Func<IQuery<TState, T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenReturnOf<Tout>(Func<ITypedQuery<TState, T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(Func<IOperationBlock<TState>, Task<ICommandResult>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenAppend<Tout>(Func<IStackBlock<TState, T>, Task<IQueryResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }



        public OperationStack<TState> Then(string tag, Func<ICommand<TState, T>, Task<BlockResultVoid>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> Then(string tag, Func<ICommand<TState, T>, Task> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenReturn<Tout>(string tag, Func<IQuery<TState, T>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenReturnOf<Tout>(string tag, Func<ITypedQuery<TState, T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState> ThenAppend(string tag, Func<IOperationBlock<TState>, Task<ICommandResult>> op)
        {
            return internalStack.CreateNew(new StackBlockSpecCommand<TState, T>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }

        public OperationStack<TState, Tout> ThenAppend<Tout>(string tag, Func<IStackBlock<TState, T>, Task<IQueryResult<Tout>>> op)
        {
            return internalStack.CreateNew<Tout>(new StackBlockSpecQuery<TState, T, Tout>(tag, internalStack.NextIndex, op, BlockSpecTypes.Operation));
        }


        public OperationStack<TState> Finally(Func<ICommand<TState, T>, Task<BlockResultVoid>> op)
        {
            return null;
        }

        public OperationStack<TState, Tout> FinallyReturn<Tout>(Func<IQuery<TState, T>, Task<BlockResult<Tout>>> op)
        {
            return null;
        }

        public OperationStack<TState, Tout> FinallyReturnOf<Tout>(Func<ITypedQuery<TState, T, Tout>, Task<BlockResult<Tout>>> op)
        {
            return null;
        }

        #endregion Sync

        #region OnEvents


        public OperationStack<TState,T> OnEvents(Func<IEventsHandler<IOperationEvent, TState, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState,T> OnEventsOf<TEvent>(Func<IEventsHandler<TEvent, TState, T>, BlockResult<T>> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState,T> OnErrors(Func<IErrorsHandler<IOperationError, TState, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnErrorsOf<TError>(Func<IErrorsHandler<TError, TState, T>, BlockResult<T>> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnExceptions(Func<IExceptionsErrorHandler<Exception,TState, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnExceptionsOf<TException>(Func<IExceptionsErrorHandler<TException, TState, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnUnhandledExceptions(Func<IExceptionsErrorHandler<Exception, TState, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnUnhandledExceptionsOf<TException>(Func<IExceptionsErrorHandler<TException, TState, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }






        public OperationStack<TState,T> OnEventsWhere(Func<IOperationEvent, bool> filter, Func<IEventsHandler<IOperationEvent, TState, T>, BlockResult<T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState,T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Func<IEventsHandler<TEvent, TState, T>, BlockResult<T>> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState,T> OnErrorsWhere(Func<IOperationError, bool> filter, Func<IErrorsHandler<IOperationError, TState, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Func<IErrorsHandler<TError, TState, T>, BlockResult<T>> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnExceptionsWhere(Func<IOperationError, bool> filter, Func<IExceptionsErrorHandler<Exception, TState, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionsErrorHandler<TException, TState, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<Exception>, bool> filter, Func<IExceptionsErrorHandler<Exception, TState, T>, BlockResult<T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Func<IExceptionsErrorHandler<TException, TState, T>, BlockResult<T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }



        public OperationStack<TState,T> OnEvents(Action<IEventsHandler<IOperationEvent, TState, T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState,T> OnEventsOf<TEvent>(Action<IEventsHandler<TEvent, TState, T>> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op));
        }

        public OperationStack<TState,T> OnErrors(Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnErrorsOf<TError>(Action<IErrorsHandler<TError, TState, T>> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnExceptions(Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnExceptionsOf<TException>(Action<IExceptionsErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnUnhandledExceptions(Action<IExceptionsErrorHandler<Exception, TState, T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }

        public OperationStack<TState,T> OnUnhandledExceptionsOf<TException>(Action<IExceptionsErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler));
        }





        public OperationStack<TState,T> OnEventsWhere(Func<IOperationEvent, bool> filter, Action<IEventsHandler<IOperationEvent, TState, T>> op)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState,T> OnEventsOfWhere<TEvent>(Func<TEvent, bool> filter, Action<IEventsHandler<TEvent, TState, T>> op)
            where TEvent : IOperationEvent
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, op, filter));
        }

        public OperationStack<TState,T> OnErrorsWhere(Func<IOperationError, bool> filter, Action<IErrorsHandler<IOperationError, TState, T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnErrorsOfWhere<TError>(Func<TError, bool> filter, Action<IErrorsHandler<TError, TState, T>> handler)
            where TError : IOperationError
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnExceptionsWhere(Func<IOperationError, bool> filter, Action<IExceptionsErrorHandler<Exception, TState, T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionsErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnUnhandledExceptionsWhere(Func<IOperationExceptionError<Exception>, bool> filter, Action<IExceptionsErrorHandler<Exception, TState, T>> handler)
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        public OperationStack<TState,T> OnUnhandledExceptionsOfWhere<TException>(Func<IOperationExceptionError<TException>, bool> filter, Action<IExceptionsErrorHandler<TException, TState, T>> handler)
            where TException : Exception
        {
            return internalStack.CreateNew<T>(stackBlockSpecBuilder.Build(internalStack.NextIndex, handler, filter));
        }

        #endregion

        #region Result

        public IQueryResult<T> ToResult()
        {
            return internalStack.ToResult<T>();
        }

        public Task<IQueryResult<T>> ToResultAsync()
        {
            return internalStack.ToResultAsync<T>();
        }

        #endregion Result
    }

    public class OperationStack : OperationStack<object>
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
