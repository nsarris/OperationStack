namespace DomainObjects.Operations
{
    public class EventHandlerBlockBase<TState, TOperationEvent, Tin> : StackBlockBase<TState, TOperationEvent>, IStackBlock<TState, TOperationEvent, Tin>, IResultDispatcher<Tin, TState>
        where TOperationEvent : IOperationEvent
    {
        ResultDispatcher<Tin,TState> resultDispather = new ResultDispatcher<Tin,TState>();
        internal EventHandlerBlockBase(string tag, TState state, Emptyable<Tin> input, IStackEvents<TOperationEvent> stackEvents) : base(tag, state, stackEvents)
        {
            Input = input;
        }

        public Emptyable<Tin> Input { get; protected set; }

        public BlockResult<Tin> Break(bool success)
        {
            return resultDispather.Break(success);
        }

        public BlockResult<Tin> End(bool success)
        {
            return resultDispather.End(success);
        }

        public BlockResult<Tin> End(bool success, object overrideResult)
        {
            return resultDispather.End(success, overrideResult);
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

        public BlockResult<Tin> Return(bool success)
        {
            return resultDispather.Return(Input.Value, success);
        }

        public BlockResult<Tin> Return()
        {
            return resultDispather.Return(Input.Value);
        }

        public BlockResult<Tin> Return(Tin result, bool success)
        {
            return resultDispather.Return(result, success);
        }

        public BlockResult<Tin> Return(Tin result)
        {
            return resultDispather.Return(result);
        }

        public BlockResult<Tin> Goto(string tag, bool success)
        {
            return resultDispather.Goto(tag, success);
        }

        public BlockResult<Tin> Goto(string tag)
        {
            return resultDispather.Goto(tag);
        }

        public BlockResult<Tin> Goto(string tag, object overrideInput, bool success)
        {
            return resultDispather.Goto(tag, overrideInput, success);
        }

        public BlockResult<Tin> Goto(string tag, object overrideInput)
        {
            return resultDispather.Goto(tag, overrideInput);
        }

        public BlockResult<Tin> Skip(int i, bool success = true)
        {
            return resultDispather.Skip(i, success);
        }

        public BlockResult<Tin> Skip(int i)
        {
            return resultDispather.Skip(i);
        }

        public BlockResult<Tin> Skip(int i, object overrideInput, bool success = true)
        {
            return resultDispather.Skip(i, overrideInput, success);
        }

        public BlockResult<Tin> Skip(int i, object overrideInput)
        {
            return resultDispather.Skip(i, overrideInput);
        }
    }

}
