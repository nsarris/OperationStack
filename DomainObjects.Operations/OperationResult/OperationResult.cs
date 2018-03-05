using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public abstract class OperationResult<TState, TOperationEvent> : IOperationResult<TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        public bool Success { get; private set; }
        public IEnumerable<TOperationEvent> Events { get; private set; }
        public IReadOnlyList<BlockTraceResult<TOperationEvent>> StackTrace { get; private set; }

        public TState StackState { get; private set; } 

        object IOperationResult.StackState => StackState;

        IEnumerable<OperationEvent> IOperationResult.Events => Events;

        public OperationResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace, TState stackState)
        {
            Success = success;
            StackState = stackState;

            StackTrace = new System.Collections.ObjectModel.ReadOnlyCollection<BlockTraceResult<TOperationEvent>>(
                stackTrace
                .ToList());

            Events = StackTrace.SelectMany(x => x.Events).ToList();
        }
    }
}
