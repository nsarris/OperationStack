﻿namespace DomainObjects.Operations
{
    public interface IBlockResult
    {
        BlockResultTarget Target { get; }
        ExecutionTime ExecutionTime { get; set; }
        IEmptyable Result { get; }
        IEmptyable GetNextInput();
        void OverrideResult(IEmptyable result);
    }
}
