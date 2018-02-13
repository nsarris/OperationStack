using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    internal class QueryBlock<TState, TOperationEvent,TResult> : StackBlockBase<TState, TOperationEvent>, IQuery<TState, TOperationEvent>, ITypedQuery<TState, TOperationEvent, TResult>
        where TOperationEvent : IOperationEvent
    {
        private ResultDispatcher<TResult,TState> resultDispatcher = new ResultDispatcher<TResult,TState>();
                protected QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents)
            : base(tag, state, stackEvents)
        {
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent,TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent,TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, IQueryOperation<TOperationEvent, TResult> queryOperation)
            : base(tag, state, stackEvents)
        {
            if (queryOperation.SupportsAsync && queryOperation.PreferAsync)
                executor = () => { var r = queryOperation.ToResult(); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
            else
                executorAsync = async () => { var r = await queryOperation.ToResultAsync().ConfigureAwait(false); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }


        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, IQueryOperation<TState, TOperationEvent, TResult> queryOperation)
            : base(tag, state, stackEvents)
        {
            if (queryOperation.SupportsAsync && queryOperation.PreferAsync)
                executor = () => { var r = queryOperation.ToResult(state); this.Append(r); this.StackState = r.StackState; return resultDispatcher.Return(r.Result.Value); };
            else
                executorAsync = async () => { var r = await queryOperation.ToResultAsync(state).ConfigureAwait(false); this.Append(r); this.StackState = r.StackState; return resultDispatcher.Return(r.Result.Value); };
        }


        IQueryResultProxy<T,TState> IQuery<TState, TOperationEvent>.DefineResult<T>()
        {
            return new QueryResultProxy<T,TState>();
        }

        IQueryResultProxy<T, TState> IQuery<TState, TOperationEvent>.DefineResult<T>(T result)
        {
            return new QueryResultProxy<T,TState>() { Result = result };
        }

        IQueryResultProxy<T, TState> IQuery<TState, TOperationEvent>.DefineResult<T>(Expression<Func<T>> expression)
        {
            return new QueryResultProxy<T,TState>();
        }



        BlockResult<T> IResultDispatcher<TState>.Fail<T>()
        {
            return new ResultDispatcher<T,TState>().Fail();
        }

        BlockResult<T> IResultDispatcher<TState>.Fail<T>(IOperationEvent error)
        {
            return new ResultDispatcher<T, TState>().Fail(error);
        }

        BlockResult<T> IResultDispatcher<TState>.Complete<T>()
        {
            return new ResultDispatcher<T,TState>().Complete();
        }

        BlockResult<T> IResultDispatcher<TState>.Complete<T>(object overrideResult)
        {
            return new ResultDispatcher<T,TState>().Complete(overrideResult);
        }

        

        BlockResult<T> IResultDispatcher<TState>.Reset<T>()
        {
            return new ResultDispatcher<T,TState>().Reset();
        }

        BlockResult<T> IResultDispatcher<TState>.Reset<T>(TState state)
        {
            return new ResultDispatcher<T,TState>().Reset(state);
        }

        BlockResult<T> IResultDispatcher<TState>.Restart<T>()
        {
            return new ResultDispatcher<T,TState>().Restart();
        }

        

        BlockResult<T> IResultDispatcher<TState>.Return<T>(T result)
        {
            return new ResultDispatcher<T,TState>().Return(result);
        }

  

        BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag)
        {
            return new ResultDispatcher<T,TState>().Goto(tag);
        }

    
        BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag, object overrideInput)
        {
            return new ResultDispatcher<T,TState>().Goto(tag, overrideInput);
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
            return new ResultDispatcher<T,TState>().Skip(i);
        }

     

        BlockResult<T> IResultDispatcher<TState>.Skip<T>(int i, object overrideInput)
        {
            return new ResultDispatcher<T,TState>().Skip(i, overrideInput);
        }

        BlockResult<T> IResultDispatcher<TState>.Retry<T>()
        {
            return new ResultDispatcher<T, TState>().Retry();
        }



        BlockResult<T> IResultDispatcher<TState>.Retry<T>(object overrideInput)
        {
            return new ResultDispatcher<T, TState>().Retry(overrideInput);
        }





        BlockResult<TResult> IResultDispatcher<TResult,TState>.Return(TResult result)
        {
            return resultDispatcher.Return(result);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Complete()
        {
            return resultDispatcher.Complete();
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Complete(object overrideResult)
        {
            return resultDispatcher.Complete( overrideResult);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Fail()
        {
            return resultDispatcher.Fail();
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Fail(IOperationEvent error)
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
