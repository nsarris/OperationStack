using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public abstract class StackBlockBase<TState, TOperationEvent> : IStackBlock<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        public string Tag { get; private set; }
        public TState StackState { get; set; }
        public IStackEvents<TOperationEvent> StackEvents { get; set; }
        public IOperationEvents<TOperationEvent> Events { get; private set; } = new OperationEvents<TOperationEvent>();
        public IEnumerable<TOperationEvent> FlattenedEvents => Events.Concat(innerStackTrace.SelectMany(x => x.FlattenedEvents));
        public bool IsEmptyEventBlock { get; protected set; }
        public bool IsAsync => executorAsync != null;
        List<BlockTraceResult<TOperationEvent>> innerStackTrace = new List<BlockTraceResult<TOperationEvent>>();
        public IEnumerable<BlockTraceResult<TOperationEvent>> InnerStackTrace => innerStackTrace.AsEnumerable();
        private Func<Exception, TOperationEvent> unhandledExceptionEventBuilder;
        public StackBlockBase(string tag, TState state, IStackEvents<TOperationEvent> stackEvents)
        {
            Tag = tag;
            StackState = state;
            StackEvents = stackEvents;

            var ctor = typeof(TOperationEvent).GetConstructor(new Type[] { typeof(Exception), typeof(bool) });
            var param1 = Expression.Parameter(typeof(Exception));
            var l = Expression.Lambda<Func<Exception, TOperationEvent>>(Expression.New(ctor, param1, Expression.Constant(true)), param1);
            unhandledExceptionEventBuilder = l.Compile();
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
                    ((BlockResultBase)result).ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
                }
                return result;
            }
            catch(Exception e)
            {
                this.Events.Add(unhandledExceptionEventBuilder(e));
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
            //Run with configure await false if no stopwatch (context) needed
            if (!measureTime)
                try
                {
                    return IsAsync ? await executorAsync().ConfigureAwait(false) : executor();
                }
                catch(Exception e)
                {
                    this.Events.Throw(unhandledExceptionEventBuilder(e));
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
                    ((BlockResultBase)result).ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
                }
                return result;
            }
            catch (Exception e)
            {
                this.Events.Add(unhandledExceptionEventBuilder(e));
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


}
