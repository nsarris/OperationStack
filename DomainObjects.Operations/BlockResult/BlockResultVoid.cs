namespace DomainObjects.Operations
{
    public sealed class BlockResultVoid : IBlockResult
    {
        internal BlockResultVoid()
            :base()
        {
            
        }

        internal BlockResultTarget Target { get; set; }
        internal ExecutionTime ExecutionTime { get; set; }
        internal IEmptyable Result { get; private set; } = Emptyable.Empty;

        BlockResultTarget IBlockResult.Target => Target;

        ExecutionTime IBlockResult.ExecutionTime { get => ExecutionTime; set => ExecutionTime = value; }

        IEmptyable IBlockResult.Result => Result;

        internal IEmptyable GetEffectiveResult()
        {
            return Target.OverrideResult.IsEmpty ?
                Result :
                Target.OverrideResult.ConvertTo<object>();
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
