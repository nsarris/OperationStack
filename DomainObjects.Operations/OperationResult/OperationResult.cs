using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public abstract class OperationResult<TInput, TState> : IOperationResult<TInput, TState>
    {
        public bool Success { get; private set; }
        public IEnumerable<OperationEvent> Events { get; private set; }
        public IReadOnlyList<BlockTraceResult> StackTrace { get; private set; }

        public TState StackState { get; private set; }
        public TInput StackInput { get; private set; }

        object IOperationResult.StackState => StackState;
        object IOperationResult.StackInput => StackInput;

        IEnumerable<OperationEvent> IOperationResult.Events => Events;

        protected OperationResult(bool success, IEnumerable<BlockTraceResult> stackTrace, TInput stackInput, TState stackState)
        {
            Success = success;
            StackState = stackState;
            StackInput = stackInput;

            StackTrace = new System.Collections.ObjectModel.ReadOnlyCollection<BlockTraceResult>(
                stackTrace
                .ToList());

            Events = StackTrace.SelectMany(x => x.FlattenedEvents.Where(e => !e.IsSwallowed)).ToList();
        }
    }
}
