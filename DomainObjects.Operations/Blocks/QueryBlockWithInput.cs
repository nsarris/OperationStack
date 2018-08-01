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
            executor = () => { var r = func(this); this.Append(r); return ((IResultDispatcher<TResult, TOperationEvent>)this).Return(r.Result.Value); };
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
            executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return ((IResultDispatcher<TResult, TOperationEvent>)this).Return(r.Result.Value); };
        }

        internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, IQueryOperation<TOperationEvent, TResult> queryOperation)
            : base(tag, stackInput, state, stackEvents)
        {
            if (queryOperation.SupportsAsync && queryOperation.PreferAsync)
                executor = () => {
                    IQueryResult<TOperationEvent, TResult> r;
                    if (queryOperation is IQueryOperation<Tin, TState, TOperationEvent, TResult> queryOperationWithInputAndState)
                        r = queryOperationWithInputAndState.Execute(input.Value, state);
                    if (queryOperation is IQueryOperationWithInput<Tin, TOperationEvent, TResult> queryOperationWithInput)
                        r = queryOperationWithInput.Execute(input.Value);
                    if (queryOperation is IQueryOperationWithState<TState, TOperationEvent, TResult> queryOperationWithState)
                        r = queryOperationWithState.Execute(state);
                    else
                        r = queryOperation.Execute();

                    this.Append(r);
                    return ((IResultDispatcher<TResult, TOperationEvent>)this).Return(r.Result.Value);
                };
            else
                executorAsync = async () => {
                    IQueryResult<TOperationEvent, TResult> r;
                    if (queryOperation is IQueryOperation<Tin, TState, TOperationEvent, TResult> queryOperationWithInputAndState)
                        r = await queryOperationWithInputAndState.ExecuteAsync(input.Value, state).ConfigureAwait(false);
                    if (queryOperation is IQueryOperationWithInput<Tin, TOperationEvent, TResult> queryOperationWithInput)
                        r = await queryOperationWithInput.ExecuteAsync(input.Value).ConfigureAwait(false);
                    if (queryOperation is IQueryOperationWithState<TState, TOperationEvent, TResult> queryOperationWithState)
                        r = await queryOperationWithState.ExecuteAsync(state).ConfigureAwait(false);
                    else
                        r = await queryOperation.ExecuteAsync().ConfigureAwait(false);

                    this.Append(r);
                    return ((IResultDispatcher<TResult, TOperationEvent>)this).Return(r.Result.Value);
                };
        }
    }


}
