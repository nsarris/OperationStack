namespace DomainObjects.Operations
{
    internal enum BlockFlowTarget
    {
        Return,
        Complete,
        Fail,
        Reset,
        Restart,
        Goto,
        Skip,
        Retry,
    }

    internal class BlockResultTarget
    {
        public BlockFlowTarget FlowTarget { get; set; }
        public string TargetTag { get; set; }
        public int TargetIndex { get; set; }
        public IEmptyable OverrideInput { get; set; } = Emptyable.Empty;
        public object State { get; set; }
        public bool ResetStateSet { get; set; }
        public IEmptyable OverrideResult { get; set; } = Emptyable.Empty;
        public IOperationEvent Error { get; set; }
    }
}
