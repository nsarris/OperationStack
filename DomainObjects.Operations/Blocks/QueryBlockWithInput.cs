using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class QueryBlock<TState, TOperationEvent, Tin, TResult> : QueryBlock<TState, TOperationEvent, TResult>, IQuery<TState, TOperationEvent, Tin>, ITypedQuery<TState, TOperationEvent, Tin,TResult>
        where TOperationEvent : IOperationEvent
    {
        public new Emptyable<Tin> Input { get => (Emptyable<Tin>)base.Input; private set => base.Input = value; }
        
        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TState, TOperationEvent, Tin,TResult>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent,TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent,TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }
    }


}
