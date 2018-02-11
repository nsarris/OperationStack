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

        public BlockResult<T> Break(bool success)
        {
            return resultDispatcher.Break(success);
        }

        public BlockResult<T> End(bool success)
        {
            return resultDispatcher.End(success);
        }

        public BlockResult<T> End(bool success,object overrideResult)
        {
            return resultDispatcher.End(success,overrideResult);
        }

        public BlockResult<T> Goto(string tag, bool success = true)
        {
            return resultDispatcher.Goto(tag,success);
        }

        public BlockResult<T> Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }

        public BlockResult<T> Goto(string tag, object overrideInput, bool success = true)
        {
            return resultDispatcher.Goto(tag, overrideInput, success);
        }

        public BlockResult<T> Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
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

        public BlockResult<T> Return(T result, bool success = true)
        {
            return resultDispatcher.Return(result, success);
        }

        public BlockResult<T> Return(T result)
        {
            return resultDispatcher.Return(result);
        }

        public BlockResult<T> Skip(int i, bool success = true)
        {
            return resultDispatcher.Skip(i, success);
        }

        public BlockResult<T> Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }

        public BlockResult<T> Skip(int i, object overrideInput, bool success = true)
        {
            return resultDispatcher.Skip(i, overrideInput,success);
        }

        public BlockResult<T> Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }

        public BlockResult<T> Return(bool success = true)
        {
            return resultDispatcher.Return(Result,success);
        }

        public BlockResult<T> Return()
        {
            return resultDispatcher.Return(Result);
        }
    }
}
