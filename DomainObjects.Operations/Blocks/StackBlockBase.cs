using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal abstract class StackBlockBase : IStackBlock<TInput, TState>
        {
            public string Tag { get; private set; }

            public IStackEvents StackEvents { get; set; }
            public IOperationEvents Events { get; private set; } = new OperationEvents();
            public IEnumerable<OperationEvent> FlattenedEvents => Events.Concat(innerStackTrace.SelectMany(x => x.FlattenedEvents));
            public bool IsEmptyEventBlock { get; protected set; }
            public bool IsAsync => executorAsync != null;
            public TInput StackInput { get; private set; }
            public TState StackState { get; set; }

            public IEnumerable<BlockTraceResult> InnerStackTrace => innerStackTrace.AsEnumerable();
            public IEmptyable Input { get; set; } = Emptyable.Empty;

            object IStackBlock.StackState => StackState;

            readonly List<BlockTraceResult> innerStackTrace = new List<BlockTraceResult>();

            protected StackBlockBase(string tag, TInput input, TState state, IStackEvents stackEvents)
            {
                Tag = tag;
                StackEvents = stackEvents;
                StackState = state;
                StackInput = input;
            }

            protected Func<IBlockResult> executor;
            protected Func<Task<IBlockResult>> executorAsync;

            public void Append(IOperationResult result)
            {
                //this.Events.Append(result.Events);
                innerStackTrace.AddRange(result.StackTrace);
            }

            public void Throw(OperationEvent error)
            {
                this.Events.Throw(error);
            }

            public IBlockResult Execute(bool timeMeasurment)
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
                if (timeMeasurment)
                {
                    sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                }

                try
                {
                    var result = IsAsync ? AsyncHelper.RunSync(executorAsync) : executor();
                    if (timeMeasurment)
                    {
                        sw.Stop();
                        result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
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
                    if (timeMeasurment)
                        result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);

                    return result;
                }
            }

            public async Task<IBlockResult> ExecuteAsync(bool timeMeasurment)
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
                if (!timeMeasurment && IsAsync)
                    try
                    {
                        return await executorAsync().ConfigureAwait(false);
                    }
                    catch (Exception e)
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
                if (timeMeasurment)
                {
                    sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                }

                try
                {
                    var result = IsAsync ? await executorAsync() : executor();
                    if (timeMeasurment)
                    {
                        sw.Stop();
                        result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
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
                    if (timeMeasurment)
                        result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);

                    return result;
                }
            }
        }
    }
}
