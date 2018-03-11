namespace DomainObjects.Operations
{
    internal class EventHandlerBlockBase<TInput, TState, TOperationEvent, Tin> : StackBlockBase<TInput, TState, TOperationEvent>, IStackBlock<TInput,TState, TOperationEvent, Tin>, IResultDispatcher<Tin, TState>
        where TOperationEvent : OperationEvent
    {
        ResultDispatcher<Tin,TState> resultDispather = new ResultDispatcher<Tin,TState>();
        internal EventHandlerBlockBase(string tag, TInput stackInput, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents) 
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
        }

        public new Emptyable<Tin> Input { get => (Emptyable<Tin>)base.Input; protected set => base.Input = value; }

        public BlockResult<Tin> Fail()
        {
            return resultDispather.Fail();
        }

        public BlockResult<Tin> Fail(OperationEvent error)
        {
            return resultDispather.Fail(error);
        }

        public BlockResult<Tin> Complete()
        {
            return resultDispather.Complete();
        }

        public BlockResult<Tin> Complete(object overrideResult)
        {
            return resultDispather.Complete(overrideResult);
        }

        public BlockResult<Tin> Reset()
        {
            return resultDispather.Reset();
        }

        public BlockResult<Tin> Reset(TState state)
        {
            return resultDispather.Reset(state);
        }

        public BlockResult<Tin> Restart()
        {
            return resultDispather.Restart();
        }
        
        public BlockResult<Tin> Return()
        {
            return resultDispather.Return(Input.Value);
        }
        
        public BlockResult<Tin> Return(Tin result)
        {
            return resultDispather.Return(result);
        }
        
        public BlockResult<Tin> Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        public BlockResult<Tin> Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
        }

        public BlockResult<Tin> Goto(int index)
        {
            return resultDispather.Goto(index);
        }

        public BlockResult<Tin> Goto(int index, object overrideInput)
        {
            return resultDispather.Goto(index, overrideInput);
        }

        public BlockResult<Tin> Skip(int i)
        {
            return resultDispather.Skip(i);
        }
        
        public BlockResult<Tin> Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i, overrideInput);
        }

        public BlockResult<Tin> Retry()
        {
            return resultDispather.Retry();
        }

        public BlockResult<Tin> Retry(Tin overrideInput)
        {
            return resultDispather.Retry(overrideInput);
        }

        BlockResult<Tin> IResultDispatcher<Tin, TState>.Retry(object overrideInput)
        {
            return resultDispather.Retry(overrideInput);
        }
    }

}
