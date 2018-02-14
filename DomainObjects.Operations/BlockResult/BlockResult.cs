using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public sealed class BlockResult<T> : IBlockResult
    {
        internal BlockResult()
        {
            Result = new Emptyable<T>();
        }

        internal BlockResult(Emptyable<T> result)
        {
            Result = result;
        }

        internal BlockResultTarget Target { get; set; }
        internal ExecutionTime ExecutionTime { get; set; }
        internal Emptyable<T> Result { get; private set; } = new Emptyable<T>();

        BlockResultTarget IBlockResult.Target => Target;

        ExecutionTime IBlockResult.ExecutionTime { get => ExecutionTime; set => ExecutionTime = value; }

        IEmptyable IBlockResult.Result => Result;

        public void OverrideResult(IEmptyable result)
        {
            try
            {
                Result = result.ConvertTo<T>();
            }
            catch
            {
                //Handle?
            }
        }

        internal IEmptyable GetNextInput()
        {
            return Target.OverrideInput.IsEmpty ?
                Result :
                Target.OverrideInput;
        }

        
        
        IEmptyable IBlockResult.GetNextInput() => GetNextInput();

        

        
    }
}
