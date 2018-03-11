using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class QueryBlock<TInput, TState, TOperationEvent, Tin, TResult> : QueryBlock<TInput, TState, TOperationEvent, TResult>, IQuery<TInput,TState, TOperationEvent, Tin>, ITypedQuery<TInput,TState, TOperationEvent, Tin,TResult>
        where TOperationEvent : OperationEvent
    {
        public new Emptyable<Tin> Input { get => (Emptyable<Tin>)base.Input; private set => base.Input = value; }
        
        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IQuery<TInput,TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TInput,TState, TOperationEvent, Tin,TResult>, BlockResult<TResult>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent,TResult>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IQuery<TInput,TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TInput,TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this).ConfigureAwait(false);
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent,TResult>>> func)
            : base(tag, stackInput, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }
    }


}
