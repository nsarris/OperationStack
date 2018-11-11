using System;

namespace DomainObjects.Operations
{
    internal interface IStackBlockSpec
    {
        BlockSpecTypes BlockType { get; }
        int Index { get; }
        Type InputType { get; }
        string Tag { get; }
    }

    internal interface IStackBlockSpec<in TInput, in TState> : IStackBlockSpec
    {
        IStackBlock CreateBlock(TInput stackInput, TState state, IStackEvents stackEvents, IEmptyable input);
    }
}