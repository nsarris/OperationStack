using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class QueryResultProxy<T, TState> : IQueryResultProxy<T,TState>
    {
        private ResultDispatcher<T,TState> resultDispatcher = new ResultDispatcher<T,TState>();
        public T Result { get; set; }

        public BlockResult<T> Fail()
        {
            return resultDispatcher.Fail();
        }

        public BlockResult<T> Complete()
        {
            return resultDispatcher.Complete();
        }

        public BlockResult<T> Complete(object overrideResult)
        {
            return resultDispatcher.Complete(overrideResult);
        }

        
        public BlockResult<T> Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }

        

        public BlockResult<T> Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        public BlockResult<T> Goto(int index)
        {
            return resultDispatcher.Goto(index);
        }



        public BlockResult<T> Goto(int index, object overrideInput)
        {
            return resultDispatcher.Goto(index, overrideInput);
        }

        public BlockResult<T> Reset()
        {
            return resultDispatcher.Reset();
        }

        public BlockResult<T> Reset(TState state)
        {
            return resultDispatcher.Reset(state);
        }

        public BlockResult<T> Restart()
        {
            return resultDispatcher.Restart();
        }

        

        public BlockResult<T> Return(T result)
        {
            return resultDispatcher.Return(result);
        }

        

        public BlockResult<T> Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }

        

        public BlockResult<T> Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }

        

        public BlockResult<T> Return()
        {
            return resultDispatcher.Return(Result);
        }

        public BlockResult<T> Fail(IOperationEvent error)
        {
            return resultDispatcher.Fail(error);
        }

        public BlockResult<T> Retry()
        {
            return resultDispatcher.Retry();
        }

        public BlockResult<T> Retry(object overrideInput)
        {
            return resultDispatcher.Retry(overrideInput);
        }
    }
}
