namespace DomainObjects.Operations
{
    public class EventHandlerBlockBase<TState, TOperationEvent> : StackBlockBase<TState, TOperationEvent>, IResultVoidDispatcher<TState>
        where TOperationEvent : IOperationEvent
    {
        ResultVoidDispatcher<TState> resultDispather = new ResultVoidDispatcher<TState>();
        public EventHandlerBlockBase(string tag, TState state, IStackEvents<TOperationEvent> stackEvents) : base(tag, state, stackEvents)
        {

        }

        public BlockResultVoid Break(bool success)
        {
            return resultDispather.Break(success);
        }

        public BlockResultVoid End(bool success = true)
        {
            return resultDispather.End(success);
        }

        public BlockResultVoid End(bool success, object overrideResult)
        {
            return resultDispather.End(success, overrideResult);
        }

        public BlockResultVoid Reset()
        {
            return resultDispather.Reset();
        }

        public BlockResultVoid Reset(TState state)
        {
            return resultDispather.Reset(state);
        }

        public BlockResultVoid Restart()
        {
            return resultDispather.Restart();
        }

        public BlockResultVoid Return(bool success)
        {
            return resultDispather.Return(success);
        }

        public BlockResultVoid Return()
        {
            return resultDispather.Return();
        }

        public BlockResultVoid Goto(string tag, bool success)
        {
            return resultDispather.Goto(tag, success);
        }

        public BlockResultVoid Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        public BlockResultVoid Goto(string tag, object overrideInput, bool success)
        {
            return resultDispather.Goto(tag, overrideInput, success);
        }

        public BlockResultVoid Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
        }

        public BlockResultVoid Skip(int i, bool success)
        {
            return resultDispather.Skip(i, success);
        }

        public BlockResultVoid Skip(int i)
        {
            return resultDispather.Skip(i);
        }

        public BlockResultVoid Skip(int i, object overrideInput, bool success)
        {
            return resultDispather.Skip(i, overrideInput, success);
        }

        public BlockResultVoid Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i, overrideInput);
        }

    }

}
