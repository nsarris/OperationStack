using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class QueryBlock<TInput, TState, TOperationEvent,TResult> : StackBlockBase<TInput, TState, TOperationEvent>, IQuery<TInput,TState, TOperationEvent>, ITypedQuery<TInput,TState, TOperationEvent, TResult>
        where TOperationEvent : OperationEvent
    {
        private readonly ResultDispatcher<TResult,TState, TOperationEvent> resultDispatcher = new ResultDispatcher<TResult,TState, TOperationEvent>();
                protected QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents)
            : base(tag, stackInput, state, stackEvents)
        {
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IQuery<TInput,TState, TOperationEvent>, BlockResult<TResult>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ITypedQuery<TInput,TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TInput,TState, TOperationEvent>, IQueryResult<TOperationEvent,TResult>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IQuery<TInput,TState, TOperationEvent>, Task<BlockResult<TResult>>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ITypedQuery<TInput,TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TInput,TState, TOperationEvent>, Task<IQueryResult<TOperationEvent,TResult>>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, IQueryOperation<TOperationEvent, TResult> queryOperation)
            : base(tag, stackInput, state, stackEvents)
        {
            if (queryOperation.SupportsAsync && queryOperation.PreferAsync)
                executor = () => {
                    IQueryResult<TOperationEvent,TResult> r;
                    if (queryOperation is IQueryOperationWithState<TState, TOperationEvent, TResult> queryOperationWithState)
                        r = queryOperationWithState.Execute(state);
                    else
                        r = queryOperation.Execute();

                    this.Append(r);
                    return resultDispatcher.Return(r.Result.Value);
                };
            else
                executorAsync = async () => {
                    IQueryResult<TOperationEvent, TResult> r;
                    if (queryOperation is IQueryOperationWithState<TState, TOperationEvent, TResult> queryOperationWithState)
                        r = await queryOperationWithState.ExecuteAsync(state).ConfigureAwait(false);
                    else
                        r = await queryOperation.ExecuteAsync().ConfigureAwait(false);

                    this.Append(r);
                    return resultDispatcher.Return(r.Result.Value);
                };
        }

      

        IQueryResultProxy<T,TState, TOperationEvent> IQuery<TInput,TState, TOperationEvent>.DefineResult<T>()
        {
            return new QueryResultProxy<T,TState, TOperationEvent>();
        }

        IQueryResultProxy<T, TState, TOperationEvent> IQuery<TInput,TState, TOperationEvent>.DefineResult<T>(T result)
        {
            return new QueryResultProxy<T,TState, TOperationEvent>() { Result = result };
        }

        IQueryResultProxy<T, TState, TOperationEvent> IQuery<TInput,TState, TOperationEvent>.DefineResult<T>(Expression<Func<T>> expression)
        {
            return new QueryResultProxy<T,TState, TOperationEvent>();
        }



        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Fail<T>()
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Fail();
        }

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Fail<T>(TOperationEvent error)
        {
            return new ResultDispatcher<T, TState, TOperationEvent>().Fail(error);
        }

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Complete<T>()
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Complete();
        }

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Complete<T>(object overrideResult)
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Complete(overrideResult);
        }

        

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Reset<T>()
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Reset();
        }

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Reset<T>(TState state)
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Reset(state);
        }

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Restart<T>()
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Restart();
        }

        

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Return<T>(T result)
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Return(result);
        }

  

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Goto<T>(string tag)
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Goto(tag);
        }

    
        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Goto<T>(string tag, object overrideInput)
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Goto(tag, overrideInput);
        }


        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Goto<T>(int index)
        {
            return new ResultDispatcher<T, TState, TOperationEvent>().Goto(index);
        }


        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Goto<T>(int index, object overrideInput)
        {
            return new ResultDispatcher<T, TState, TOperationEvent>().Goto(index, overrideInput);
        }

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Skip<T>(int i)
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Skip(i);
        }

     

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Skip<T>(int i, object overrideInput)
        {
            return new ResultDispatcher<T,TState, TOperationEvent>().Skip(i, overrideInput);
        }

        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Retry<T>()
        {
            return new ResultDispatcher<T, TState, TOperationEvent>().Retry();
        }



        BlockResult<T> IResultDispatcher<TState, TOperationEvent>.Retry<T>(object overrideInput)
        {
            return new ResultDispatcher<T, TState, TOperationEvent>().Retry(overrideInput);
        }





        BlockResult<TResult> IResultDispatcher<TResult,TState, TOperationEvent>.Return(TResult result)
        {
            return resultDispatcher.Return(result);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Complete()
        {
            return resultDispatcher.Complete();
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Complete(object overrideResult)
        {
            return resultDispatcher.Complete( overrideResult);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Fail()
        {
            return resultDispatcher.Fail();
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Fail(TOperationEvent error)
        {
            return resultDispatcher.Fail(error);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Reset()
        {
            return resultDispatcher.Reset();
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Reset(TState state)
        {
            return resultDispatcher.Reset(state);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Restart()
        {
            return resultDispatcher.Restart();
        }

      

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }

    

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Goto(int index)
        {
            return resultDispatcher.Goto(index);
        }



        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Goto(int index, object overrideInput)
        {
            return resultDispatcher.Goto(index, overrideInput);
        }


        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }

      
        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Retry()
        {
            return resultDispatcher.Retry();
        }


        BlockResult<TResult> IResultDispatcher<TResult, TState, TOperationEvent>.Retry(object overrideInput)
        {
            return resultDispatcher.Retry(overrideInput);
        }
    }


}
