﻿using System;
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

        internal IEmptyable GetEffectiveResult()
        {
            return Target.OverrideResult.IsEmpty ?
                Result :
                Target.OverrideResult.ConvertTo<T>();
        }
        internal IEmptyable GetNextInput()
        {
            return Target.OverrideInput.IsEmpty ?
                Result :
                Target.OverrideInput;
        }

        IEmptyable IBlockResult.GetEffectiveResult() => GetEffectiveResult();
        
        IEmptyable IBlockResult.GetNextInput() => GetNextInput();

        

        
    }
}
