using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public abstract class OperationResult<TOperationEvent> : IOperationResult<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public bool Success { get; private set; }
        public IEnumerable<TOperationEvent> Events { get; private set; }
        public IReadOnlyList<BlockTraceResult<TOperationEvent>> StackTrace { get; private set; }

        public OperationResult(bool success, IEnumerable<BlockTraceResult<TOperationEvent>> stackTrace)
        {
            Success = success;

            StackTrace = new System.Collections.ObjectModel.ReadOnlyCollection<BlockTraceResult<TOperationEvent>>(
                stackTrace
                .ToList());

            Events = StackTrace.SelectMany(x => x.Events).ToList();
        }
    }
}
