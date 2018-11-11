using System;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public partial class OperationStack<TInput, TState, TOutput>
    {
        internal class QueryBlock<Tin, TResult> : QueryBlock<TResult>, IQuery<TInput, TState, Tin>, ITypedQuery<TInput, TState, Tin, TResult>
        {
            public new Emptyable<Tin> Input { get => (Emptyable<Tin>)base.Input; private set => base.Input = value; }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IQuery<TInput, TState, Tin>, BlockResult<TResult>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executor = () => func(this);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TInput, TState, Tin, TResult>, BlockResult<TResult>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executor = () => func(this);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput, TState, Tin>, IQueryResult<TResult>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executor = () => { var r = func(this); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IQuery<TInput, TState, Tin>, Task<BlockResult<TResult>>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executorAsync = async () => await func(this).ConfigureAwait(false);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TInput, TState, Tin, TResult>, Task<BlockResult<TResult>>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executorAsync = async () => await func(this).ConfigureAwait(false);
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TInput, TState, Tin>, Task<IQueryResult<TResult>>> func)
                : base(tag, stackInput, state, stackEvents)
            {
                Input = input;
                executorAsync = async () => { var r = await func(this).ConfigureAwait(false); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
            }

            internal QueryBlock(string tag, TInput stackInput, TState state, IStackEvents stackEvents, Emptyable<Tin> input, IQueryOperation<TResult> queryOperation)
                : base(tag, stackInput, state, stackEvents)
            {
                if (queryOperation.SupportsAsync && queryOperation.PreferAsync)
                    executor = () =>
                    {
                        IQueryResult<TResult> r;
                        if (queryOperation is IQueryOperation<Tin, TState, TResult> queryOperationWithInputAndState)
                            r = queryOperationWithInputAndState.Execute(input.Value, state);
                        else if (queryOperation is IQueryOperationWithInput<Tin, TResult> queryOperationWithInput)
                            r = queryOperationWithInput.Execute(input.Value);
                        else if (queryOperation is IQueryOperationWithState<TState, TResult> queryOperationWithState)
                            r = queryOperationWithState.Execute(state);
                        else
                            r = queryOperation.Execute();

                        this.Append(r);
                        return ((IResultDispatcher<TResult>)this).Return(r.Result.Value);
                    };
                else
                    executorAsync = async () =>
                    {
                        IQueryResult<TResult> r;
                        if (queryOperation is IQueryOperation<Tin, TState, TResult> queryOperationWithInputAndState)
                            r = await queryOperationWithInputAndState.ExecuteAsync(input.Value, state).ConfigureAwait(false);
                        else if (queryOperation is IQueryOperationWithInput<Tin, TResult> queryOperationWithInput)
                            r = await queryOperationWithInput.ExecuteAsync(input.Value).ConfigureAwait(false);
                        else if (queryOperation is IQueryOperationWithState<TState, TResult> queryOperationWithState)
                            r = await queryOperationWithState.ExecuteAsync(state).ConfigureAwait(false);
                        else
                            r = await queryOperation.ExecuteAsync().ConfigureAwait(false);

                        this.Append(r);
                        return ((IResultDispatcher<TResult>)this).Return(r.Result.Value);
                    };
            }
        }
    }
}
