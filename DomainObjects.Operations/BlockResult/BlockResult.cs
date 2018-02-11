using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class BlockResultBase
    {
        public bool Success { get; internal set; }
        internal BlockResultTarget Target { get; set; }
        internal ExecutionTime ExecutionTime { get; set; }
    }

    public class BlockResultVoid : BlockResultBase, IBlockResult
    {
        IEmptyable IBlockResult.Result => Emptyable.Empty;
    }

    public class BlockResult<T> : BlockResultBase, IBlockResult
    {
        public BlockResult()
        {
            Result = new Emptyable<T>();
        }
        public Emptyable<T> Result { get; set; }

        IEmptyable IBlockResult.Result => Result;
    }
}
