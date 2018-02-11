using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public class QueryBlock<TState, TOperationEvent,TResult> : StackBlockBase<TState, TOperationEvent>, IQuery<TState, TOperationEvent>, ITypedQuery<TState, TOperationEvent, TResult>
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



        BlockResult<T> IResultDispatcher<TState>.Break<T>(bool success)
        {
            return new ResultDispatcher<T,TState>().Break(success);
        }

        BlockResult<T> IResultDispatcher<TState>.End<T>(bool success)
        {
            return new ResultDispatcher<T,TState>().End(success);
        }

        BlockResult<T> IResultDispatcher<TState>.End<T>(bool success, object overrideResult)
        {
            return new ResultDispatcher<T,TState>().End(success, overrideResult);
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

        BlockResult<T> IResultDispatcher<TState>.Return<T>(T result, bool success)
        {
            return new ResultDispatcher<T,TState>().Return(result, success);
        }

        BlockResult<T> IResultDispatcher<TState>.Return<T>(T result)
        {
            return new ResultDispatcher<T,TState>().Return(result);
        }

        BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag, bool success)
        {
            return new ResultDispatcher<T,TState>().Goto(tag, success);
        }

        BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag)
        {
            return new ResultDispatcher<T,TState>().Goto(tag);
        }

        BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag, object overrideInput, bool success)
        {
            return new ResultDispatcher<T,TState>().Goto(tag, overrideInput, success);
        }

        BlockResult<T> IResultDispatcher<TState>.Goto<T>(string tag, object overrideInput)
        {
            return new ResultDispatcher<T,TState>().Goto(tag, overrideInput);
        }

        BlockResult<T> IResultDispatcher<TState>.Skip<T>(int i, bool success)
        {
            return new ResultDispatcher<T,TState>().Skip(i, success);
        }

        BlockResult<T> IResultDispatcher<TState>.Skip<T>(int i)
        {
            return new ResultDispatcher<T,TState>().Skip(i);
        }

        BlockResult<T> IResultDispatcher<TState>.Skip<T>(int i, object overrideInput, bool success)
        {
            return new ResultDispatcher<T,TState>().Skip(i, overrideInput, success);
        }

        BlockResult<T> IResultDispatcher<TState>.Skip<T>(int i, object overrideInput)
        {
            return new ResultDispatcher<T,TState>().Skip(i, overrideInput);
        }



        BlockResult<TResult> IResultDispatcher<TResult,TState>.Return(TResult result, bool success)
        {
            return resultDispatcher.Return(result, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult,TState>.Return(TResult result)
        {
            return resultDispatcher.Return(result);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.End(bool success)
        {
            return resultDispatcher.End(success);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.End(bool success, object overrideResult)
        {
            return resultDispatcher.End(success, overrideResult);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Break(bool success)
        {
            return resultDispatcher.Break(success);
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

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(string tag, bool success)
        {
            return resultDispatcher.Goto(tag, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(string tag, object overrideInput, bool success)
        {
            return resultDispatcher.Goto(tag, overrideInput, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Skip(int i, bool success)
        {
            return resultDispatcher.Skip(i, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Skip(int i, object overrideInput, bool success)
        {
            return resultDispatcher.Skip(i, overrideInput, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult, TState>.Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }
    }


}
