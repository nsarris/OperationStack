using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlockSpecQuery<TState, TResult> : StackBlockSpecBase<TState>
    {
        Func<IQuery<TState>, BlockResult<TResult>> func;
        Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> typedFunc;
        Func<IOperationBlock<TState>, IQueryResult<TResult>> funcWithResult;

        Func<IQuery<TState>, Task<BlockResult<TResult>>> funcAsync;
        Func<ITypedQuery<TState, TResult>, Task<BlockResult<TResult>>> typedFuncAsync;
        Func<IOperationBlock<TState>, Task<IQueryResult<TResult>>> funcWithResultAsync;

        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState>, BlockResult<TResult>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> typedFunc, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.typedFunc = typedFunc;
        }

        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState>, IQueryResult<TResult>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecQuery(int index, Func<IQuery<TState>, BlockResult<TResult>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.typedFunc = func;
        }

        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState>, IQueryResult<TResult>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResult = func;
        }



        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, TResult>, Task<BlockResult<TResult>>> typedFunc, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.typedFuncAsync = typedFunc;
        }

        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState>, Task<IQueryResult<TResult>>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResultAsync = func;
        }

        public StackBlockSpecQuery(int index, Func<IQuery<TState>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, TResult>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.typedFuncAsync = func;
        }

        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState>, Task<IQueryResult<TResult>>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResultAsync = func;
        }



        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
        {
            if (func != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, func);
            else if (typedFunc != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, typedFunc);
            else if (funcWithResult != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, funcWithResult);
            else if (funcAsync != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, funcAsync);
            else if (typedFuncAsync != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, typedFuncAsync);
            else //if (funcWithResultAsync != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, funcWithResultAsync);

        }
    }

    internal class StackBlockSpecQuery<TState, Tin, TResult> : StackBlockSpecBase<TState>
    {
        Func<IQuery<TState, Tin>, BlockResult<TResult>> func;
        Func<ITypedQuery<TState, Tin, TResult>, BlockResult<TResult>> typedFunc;
        Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> funcWithResult;

        Func<IQuery<TState, Tin>, Task<BlockResult<TResult>>> funcAsync;
        Func<ITypedQuery<TState, Tin, TResult>, Task<BlockResult<TResult>>> typedFuncAsync;
        Func<IOperationBlock<TState, Tin>, Task<IQueryResult<TResult>>> funcWithResultAsync;

        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState, Tin>, BlockResult<TResult>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, Tin, TResult>, BlockResult<TResult>> typedFunc, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.typedFunc = typedFunc;
        }

        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecQuery(int index, Func<IQuery<TState, Tin>, BlockResult<TResult>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, Tin, TResult>, BlockResult<TResult>> typedFunc, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.typedFunc = typedFunc;
        }

        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResult = func;
        }



        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState, Tin>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, Tin, TResult>, Task<BlockResult<TResult>>> typedFunc, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.typedFuncAsync = typedFunc;
        }

        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState, Tin>, Task<IQueryResult<TResult>>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResultAsync = func;
        }

        public StackBlockSpecQuery(int index, Func<IQuery<TState, Tin>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, Tin, TResult>, Task<BlockResult<TResult>>> typedFunc, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.typedFuncAsync = typedFunc;
        }

        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState, Tin>, Task<IQueryResult<TResult>>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResultAsync = func;
        }



        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
        {
            var typedInput = input.ConvertTo<Tin>();

            if (func != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, typedInput, func);
            else if (typedFunc != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, typedInput, typedFunc);
            else if (funcWithResult != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, typedInput, funcWithResult);
            else if (funcAsync != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, typedInput, funcAsync);
            else if (typedFuncAsync != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, typedInput, typedFuncAsync);
            else //if (funcWithResultAsync != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, typedInput, funcWithResultAsync);
        }
    }
}
