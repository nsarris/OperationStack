namespace DomainObjects.Operations
{
    internal enum BlockFlowTarget
    {
        Return,
        End,
        Break,
        Reset,
        Restart,
        Goto,
        Skip,
        Retry
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
    }

    //internal class BlockResultReturnTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultBreakTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultGotoIndexTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultGotoTagTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultRetryTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultResetTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultRestartTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultSkipTarget : BlockResultTarget
    //{

    //}

    //internal class BlockResultEndTarget : BlockResultTarget
    //{

    //}
}
