namespace DomainObjects.Operations
{
    internal class EventHandlerBlockBase<TInput, TState, TOperationEvent> : StackBlockBase<TInput, TState, TOperationEvent>, IResultVoidDispatcher<TState>
        where TOperationEvent : OperationEvent
    {
        ResultVoidDispatcher<TState> resultDispather = new ResultVoidDispatcher<TState>();
        public EventHandlerBlockBase(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents) 
            : base(tag, stackInput, state, stackEvents)
        {

        }

        public BlockResultVoid Fail()
        {
            return resultDispather.Fail();
        }

        public BlockResultVoid Fail(OperationEvent error)
        {
            return resultDispather.Fail(error);
        }

        public BlockResultVoid Complete()
        {
            return resultDispather.Complete();
        }

        public BlockResultVoid Complete(object overrideResult)
        {
            return resultDispather.Complete(overrideResult);
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

        
        public BlockResultVoid Return()
        {
            return resultDispather.Return();
        }

        
        public BlockResultVoid Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        
        public BlockResultVoid Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
        }

        public BlockResultVoid Goto(int index)
        {
            return resultDispather.Goto(index);
        }


        public BlockResultVoid Goto(int index, object overrideInput)
        {
            return resultDispather.Goto(index, overrideInput);
        }

        
        public BlockResultVoid Skip(int i)
        {
            return resultDispather.Skip(i);
        }

        
        public BlockResultVoid Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i, overrideInput);
        }

        public BlockResultVoid Retry()
        {
            return resultDispather.Retry();
        }


        public BlockResultVoid Retry(object overrideInput)
        {
            return resultDispather.Retry(overrideInput);
        }

    }

}
