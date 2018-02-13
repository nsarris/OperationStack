namespace DomainObjects.Operations
{
    internal interface IBlockResult
    {
        BlockResultTarget Target { get; }
        ExecutionTime ExecutionTime { get; set; }
        IEmptyable Result { get; }
        IEmptyable GetNextInput();
        IEmptyable GetEffectiveResult();
    }
}
