using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal abstract class StackBlockBase<TOperationEvent> : IStackBlock<TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public string Tag { get; private set; }
        
        public IStackEvents<TOperationEvent> StackEvents { get; set; }
        public IOperationEvents<TOperationEvent> Events { get; private set; } = new OperationEvents<TOperationEvent>();
        public IEnumerable<TOperationEvent> FlattenedEvents => Events.Concat(innerStackTrace.SelectMany(x => x.FlattenedEvents));
        public bool IsEmptyEventBlock { get; protected set; }
        public bool IsAsync => executorAsync != null;
        List<BlockTraceResult<TOperationEvent>> innerStackTrace = new List<BlockTraceResult<TOperationEvent>>();
        public IEnumerable<BlockTraceResult<TOperationEvent>> InnerStackTrace => innerStackTrace.AsEnumerable();
        
        internal virtual IEmptyable Input { get; set; } = Emptyable.Empty;
        public StackBlockBase(string tag, IStackEvents<TOperationEvent> stackEvents)
        {
            Tag = tag;
            StackEvents = stackEvents;

            
        }
        
        protected Func<IBlockResult> executor;
        protected Func<Task<IBlockResult>> executorAsync;

        public void Append(IOperationResult<TOperationEvent> result)
        {
            this.Events.Append(result.Events);
            innerStackTrace.AddRange(result.StackTrace);
        }
        
        internal IBlockResult Execute(bool measureTime = true)
        {
            if (IsEmptyEventBlock)
                return new BlockResultVoid()
                {
                    Target = new BlockResultTarget
                    {
                        FlowTarget = BlockFlowTarget.Return,
                        OverrideInput = Input
                    },
                };

            System.Diagnostics.Stopwatch sw = null;
            if (measureTime)
            {
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
            }

            try
            {
                var result = IsAsync ? AsyncHelper.RunSync(executorAsync) : executor();
                if (measureTime)
                {
                    sw.Stop();
                    result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
                }
                return result;
            }
            catch(Exception e)
            {
                this.Events.Throw(e);
                var result = new BlockResultVoid()
                {
                    Target = new BlockResultTarget
                    {
                        FlowTarget = BlockFlowTarget.Return
                    },
                };
                if (measureTime)
                    result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);

                return result;
            }
        }

        internal async Task<IBlockResult> ExecuteAsync(bool measureTime = true)
        {
            if (IsEmptyEventBlock)
                return new BlockResultVoid()
                {
                    Target = new BlockResultTarget
                    {
                        FlowTarget = BlockFlowTarget.Return,
                        OverrideInput = Input
                    },
                };
            //Run with configure await false if no stopwatch (context) needed
            if (!measureTime && IsAsync)
                try
                {
                    return await executorAsync().ConfigureAwait(false);
                }
                catch(Exception e)
                {
                    this.Events.Throw(e);
                    return new BlockResultVoid()
                    {
                        Target = new BlockResultTarget
                        {
                            FlowTarget = BlockFlowTarget.Return
                        },
                    };
                }
            
            //If measure time execute without configure await
            System.Diagnostics.Stopwatch sw = null;
            if (measureTime)
            {
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
            }

            try
            {
                var result = IsAsync ? await executorAsync() : executor();
                if (measureTime)
                {
                    sw.Stop();
                    ((IBlockResult)result).ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
                }
                return result;
            }
            catch (Exception e)
            {
                this.Events.Throw(e);
                var result = new BlockResultVoid()
                {
                    Target = new BlockResultTarget
                    {
                        FlowTarget = BlockFlowTarget.Return
                    },
                };
                if (measureTime)
                    result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);

                return result;
            }
        }
    }

    internal abstract class StackBlockBase<TState, TOperationEvent> : StackBlockBase<TOperationEvent>, IStackBlock<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public StackBlockBase(string tag, TState state, IStackEvents<TOperationEvent> stackEvents) 
            : base(tag, stackEvents)
        {
            StackState = state;
        }

        public TState StackState { get; set; }
    }
}
