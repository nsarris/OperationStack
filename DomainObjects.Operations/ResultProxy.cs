using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class QueryResultProxy<T> : IQueryResultProxy<T>
    {
        private ResultDispatcher<T> resultDispatcher = new ResultDispatcher<T>();
        public T Result { get; set; }

        public BlockResult<T> Break()
        {
            return resultDispatcher.Break();
        }

        public BlockResult<T> End()
        {
            return resultDispatcher.End();
        }

        public BlockResult<T> End(object overrideResult)
        {
            return resultDispatcher.End(overrideResult);
        }

        public BlockResult<T> Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }

        public BlockResult<T> Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        public BlockResult<T> Reset()
        {
            return resultDispatcher.Reset();
        }

        public BlockResult<T> Reset(object state)
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
    }
}
