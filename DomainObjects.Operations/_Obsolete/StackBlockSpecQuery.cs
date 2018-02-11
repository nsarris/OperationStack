//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DomainObjects.Operations
//{
//    internal class StackBlockSpecQuery<TState, TOperationEvent, TResult> : StackBlockSpecBase<TState, TOperationEvent>
//        where TOperationEvent : IOperationEvent
//    {
//        Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func;
//        Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> typedFunc;
//        Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent, TResult>> funcWithResult;

//        Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> funcAsync;
//        Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> typedFuncAsync;
//        Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, TResult>>> funcWithResultAsync;

//        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> typedFunc, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.typedFunc = typedFunc;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent, TResult>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecQuery(int index, Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.typedFunc = func;
//        }

//        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent, TResult>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResult = func;
//        }



//        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> typedFunc, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.typedFuncAsync = typedFunc;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, TResult>>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecQuery(int index, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.typedFuncAsync = func;
//        }

//        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent, TResult>>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }



//        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
//        {
//            if (func != null)
//                return new QueryBlock<TState, TOperationEvent, TResult>(Tag, state, stackEvents, func);
//            else if (typedFunc != null)
//                return new QueryBlock<TState, TOperationEvent, TResult>(Tag, state, stackEvents, typedFunc);
//            else if (funcWithResult != null)
//                return new QueryBlock<TState, TOperationEvent, TResult>(Tag, state, stackEvents, funcWithResult);
//            else if (funcAsync != null)
//                return new QueryBlock<TState, TOperationEvent, TResult>(Tag, state, stackEvents, funcAsync);
//            else if (typedFuncAsync != null)
//                return new QueryBlock<TState, TOperationEvent, TResult>(Tag, state, stackEvents, typedFuncAsync);
//            else //if (funcWithResultAsync != null)
//                return new QueryBlock<TState, TOperationEvent, TResult>(Tag, state, stackEvents, funcWithResultAsync);

//        }
//    }

//    internal class StackBlockSpecQuery<TState, TOperationEvent, Tin, TResult> : StackBlockSpecBase<TState, TOperationEvent>
//        where TOperationEvent : IOperationEvent
//    {
//        Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func;
//        Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, BlockResult<TResult>> typedFunc;
//        Func<IOperationBlock<TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent, TResult>> funcWithResult;

//        Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> funcAsync;
//        Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> typedFuncAsync;
//        Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent, TResult>>> funcWithResultAsync;

//        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, BlockResult<TResult>> typedFunc, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.typedFunc = typedFunc;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent, TResult>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecQuery(int index, Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, BlockResult<TResult>> typedFunc, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.typedFunc = typedFunc;
//        }

//        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent, TResult>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResult = func;
//        }



//        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> typedFunc, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.typedFuncAsync = typedFunc;
//        }

//        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent, TResult>>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecQuery(int index, Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> typedFunc, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.typedFuncAsync = typedFunc;
//        }

//        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent, TResult>>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }



//        public override StackBlockBase<TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
//        {
//            var typedInput = input.ConvertTo<Tin>();

//            if (func != null)
//                return new QueryBlock<TState, TOperationEvent, Tin, TResult>(Tag, state, stackEvents, typedInput, func);
//            else if (typedFunc != null)
//                return new QueryBlock<TState, TOperationEvent, Tin, TResult>(Tag, state, stackEvents, typedInput, typedFunc);
//            else if (funcWithResult != null)
//                return new QueryBlock<TState, TOperationEvent, Tin, TResult>(Tag, state, stackEvents, typedInput, funcWithResult);
//            else if (funcAsync != null)
//                return new QueryBlock<TState, TOperationEvent, Tin, TResult>(Tag, state, stackEvents, typedInput, funcAsync);
//            else if (typedFuncAsync != null)
//                return new QueryBlock<TState, TOperationEvent, Tin, TResult>(Tag, state, stackEvents, typedInput, typedFuncAsync);
//            else //if (funcWithResultAsync != null)
//                return new QueryBlock<TState, TOperationEvent, Tin, TResult>(Tag, state, stackEvents, typedInput, funcWithResultAsync);
//        }
//    }
//}
