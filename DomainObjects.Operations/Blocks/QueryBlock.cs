using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class QueryBlock<TResult> : StackBlockBase, IQuery<TInput, TState>, ITypedQuery<TInput, TState, TResult>
        {
            private readonly ResultDispatcher<TResult, TState> resultDispatcher = new ResultDispatcher<TResult, TState>();
            protected QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents)
                : base(tag, stackInput, state, stackEvents)
            {
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IQuery<TInput, TState>, BlockResult<TResult>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                executor = () => func(this);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<ITypedQuery<TInput, TState, TResult>, BlockResult<TResult>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                executor = () => func(this);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IOperationBlock<TInput, TState>, IQueryResult<TResult>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                executor = () => { var r = func(this); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IQuery<TInput, TState>, Task<BlockResult<TResult>>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                executorAsync = async () => await func(this).ConfigureAwait(false);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<ITypedQuery<TInput, TState, TResult>, Task<BlockResult<TResult>>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                executorAsync = async () => await func(this).ConfigureAwait(false);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Func<IOperationBlock<TInput, TState>, Task<IQueryResult<TResult>>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, IQueryOperation<TResult> queryOperation)
                : base(tag, stackInput, state, stackEvents)
            {
                if (queryOperation.SupportsAsync && queryOperation.PreferAsync)
                    executor = () =>
                    {
                        IQueryResult<TResult> r;
                        if (queryOperation is IQueryOperationWithState<TState, TResult> queryOperationWithState)
                            r = queryOperationWithState.Execute(state);
                        else
                            r = queryOperation.Execute();

                        this.Append(r);
                        return resultDispatcher.Return(r.Result.Value);
                    };
                else
                    executorAsync = async () =>
                    {
                        IQueryResult<TResult> r;
                        if (queryOperation is IQueryOperationWithState<TState, TResult> queryOperationWithState)
                            r = await queryOperationWithState.ExecuteAsync(state).ConfigureAwait(false);
                        else
                            r = await queryOperation.ExecuteAsync().ConfigureAwait(false);

                        this.Append(r);
                        return resultDispatcher.Return(r.Result.Value);
                    };
            }



            IQueryResultProxy<T, TState> IQuery<TInput, TState>.DefineResult<T>()
            {
                return new QueryResultProxy<T, TState>();
            }

            IQueryResultProxy<T, TState> IQuery<TInput, TState>.DefineResult<T>(T result)
            {
                return new QueryResultProxy<T, TState>() { Result = result };
            }

            IQueryResultProxy<T, TState> IQuery<TInput, TState>.DefineResult<T>(Expression<Func<T>> expression)
            {
                return new QueryResultProxy<T, TState>();
            }



            BlockResult<T> IResultDispatcher<TState>.Fail<T>()
            {
                return new ResultDispatcher<T, TState>().Fail();
            }

            BlockResult<T> IResultDispatcher<TState>.Fail<T>(OperationEvent error)
            {
                return new ResultDispatcher<T, TState>().Fail(error);
            }

            BlockResult<T> IResultDispatcher<TState>.Complete<T>()
            {
                return new ResultDispatcher<T, TState>().Complete();
            }

            BlockResult<T> IResultDispatcher<TState>.Complete<T>(object overrideResult)
            {
                return new ResultDispatcher<T, TState>().Complete(overrideResult);
            }



            BlockResult<T> IResultDispatcher<TState>.Reset<T>()
            {
                return new ResultDispatcher<T, TState>().Reset();
            }

            BlockResult<T> IResultDispatcher<TState>.Reset<T>(TState state)
            {
                return new ResultDispatcher<T, TState>().Reset(state);
            }

            BlockResult<T> IResultDispatcher<TState>.Restart<T>()
            {
                return new ResultDispatcher<T, TState>().Restart();
            }



            BlockResult<T> IResultDispatcher<TState>.Return<T>(T result)
            {
                return new ResultDispatcher<T, TState>().Return(result);
            }



            BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag)
            {
                return new ResultDispatcher<T, TState>().Goto(tag);
            }


            BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag, object overrideInput)
            {
                return new ResultDispatcher<T, TState>().Goto(tag, overrideInput);
            }


            BlockResult<T> IResultDispatcher<TState>.Goto<T>(int index)
            {
                return new ResultDispatcher<T, TState>().Goto(index);
            }


            BlockResult<T> IResultDispatcher<TState>.Goto<T>(int index, object overrideInput)
            {
                return new ResultDispatcher<T, TState>().Goto(index, overrideInput);
            }

            BlockResult<T> IResultDispatcher<TState>.Skip<T>(int i)
            {
                return new ResultDispatcher<T, TState>().Skip(i);
            }



            BlockResult<T> IResultDispatcher<TState>.Skip<T>(int i, object overrideInput)
            {
                return new ResultDispatcher<T, TState>().Skip(i, overrideInput);
            }

            BlockResult<T> IResultDispatcher<TState>.Retry<T>()
            {
                return new ResultDispatcher<T, TState>().Retry();
            }



            BlockResult<T> IResultDispatcher<TState>.Retry<T>(object overrideInput)
            {
                return new ResultDispatcher<T, TState>().Retry(overrideInput);
            }





            BlockResult<TResult> IResultDispatcher<TResult, TState>.Return(TResult result)
            {
                return resultDispatcher.Return(result);
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Complete()
            {
                return resultDispatcher.Complete();
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Complete(object overrideResult)
            {
                return resultDispatcher.Complete(overrideResult);
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Fail()
            {
                return resultDispatcher.Fail();
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Fail(OperationEvent error)
            {
                return resultDispatcher.Fail(error);
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Reset()
            {
                return resultDispatcher.Reset();
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Reset(TState state)
            {
                return resultDispatcher.Reset(state);
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Restart()
            {
                return resultDispatcher.Restart();
            }



            BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(string tag)
            {
                return resultDispatcher.Goto(tag);
            }



            BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(string tag, object overrideInput)
            {
                return resultDispatcher.Goto(tag, overrideInput);
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(int index)
            {
                return resultDispatcher.Goto(index);
            }



            BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(int index, object overrideInput)
            {
                return resultDispatcher.Goto(index, overrideInput);
            }


            BlockResult<TResult> IResultDispatcher<TResult, TState>.Skip(int i)
            {
                return resultDispatcher.Skip(i);
            }


            BlockResult<TResult> IResultDispatcher<TResult, TState>.Skip(int i, object overrideInput)
            {
                return resultDispatcher.Skip(i, overrideInput);
            }

            BlockResult<TResult> IResultDispatcher<TResult, TState>.Retry()
            {
                return resultDispatcher.Retry();
            }


            BlockResult<TResult> IResultDispatcher<TResult, TState>.Retry(object overrideInput)
            {
                return resultDispatcher.Retry(overrideInput);
            }
        }
    }
}
