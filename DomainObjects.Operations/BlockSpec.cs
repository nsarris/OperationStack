using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal interface IStackBlockSpec
    {
        string Tag { get; }
        int Index { get; }
    }
    internal abstract class StackBlockSpecBase<TState> : IStackBlockSpec
    {
        public string Tag { get; private set; }
        public int Index { get; private set; }
        public abstract StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, object input);

        public StackBlockSpecBase(int index)
        {
            Tag = "Block " + index;
            Index = index;
        }
        public StackBlockSpecBase(string tag, int index)
        {
            Tag = tag;
            Index = index;
        }
    }

    internal class StackBlockSpecCommand<TState> : StackBlockSpecBase<TState>
    {
        Func<ICommand<TState>, BlockResultVoid> func;
        Action<ICommand<TState>> action;
        Func<IOperationBlock<TState>, ICommandResult> funcWithResult;
        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TState>, ICommandResult> func)
            : base(tag, index)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState>, BlockResultVoid> func)
            : base(tag, index)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TState>> action)
            : base(tag, index)
        {
            this.action = action;
        }

        public StackBlockSpecCommand(int index, Func<IOperationBlock<TState>, ICommandResult> func)
            : base(index)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState>, BlockResultVoid> func)
            : base(index)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(int index, Action<ICommand<TState>> action)
            : base(index)
        {
            this.action = action;
        }

        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, object input)
        {
            if (func != null)
                return new Command<TState>(Tag, state, stackEvents, func);
            else if (action != null)
                return new Command<TState>(Tag, state, stackEvents, action);
            else
                return new Command<TState>(Tag, state, stackEvents, funcWithResult);
        }
    }

    internal class StackBlockSpecCommand<TState, Tin> : StackBlockSpecBase<TState>
    {
        Func<ICommand<TState, Tin>, BlockResultVoid> func;
        Action<ICommand<TState, Tin>> action;
        Func<IOperationBlock<TState, Tin>, ICommandResult> funcWithResult;

        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState, Tin>, BlockResultVoid> func)
            : base(tag, index)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TState, Tin>> action)
            : base(tag, index)
        {
            this.action = action;
        }

        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TState, Tin>, ICommandResult> func)
            : base(tag, index)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState, Tin>, BlockResultVoid> func)
            : base(index)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(int index, Action<ICommand<TState, Tin>> action)
            : base(index)
        {
            this.action = action;
        }

        public StackBlockSpecCommand(int index, Func<IOperationBlock<TState, Tin>, ICommandResult> func)
            : base(index)
        {
            this.funcWithResult = func;
        }

        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, object input)
        {
            if (func != null)
                return new Command<TState, Tin>(Tag, state, stackEvents, (Tin)input, func);
            else if (action != null)
                return new Command<TState, Tin>(Tag, state, stackEvents, (Tin)input, action);
            else
                return new Command<TState, Tin>(Tag, state, stackEvents, (Tin)input, funcWithResult);
        }
    }

    internal class StackBlockSpecQuery<TState, TResult> : StackBlockSpecBase<TState>
    {
        Func<IQuery<TState>, BlockResult<TResult>> func;
        Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> typedFunc;
        Func<IOperationBlock<TState>, IQueryResult<TResult>> funcWithResult;

        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState>, BlockResult<TResult>> func)
            : base(tag, index)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> typedFunc)
            : base(tag, index)
        {
            this.typedFunc = typedFunc;
        }

        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState>, IQueryResult<TResult>> func)
            : base(tag, index)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecQuery(int index, Func<IQuery<TState>, BlockResult<TResult>> func)
            : base(index)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> func)
            : base(index)
        {
            this.typedFunc = func;
        }

        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState>, IQueryResult<TResult>> func)
            : base(index)
        {
            this.funcWithResult = func;
        }

        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, object input)
        {
            if (func != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, func);
            else if (typedFunc != null)
                return new Query<TState, TResult>(Tag, state, stackEvents, typedFunc);
            else
                return new Query<TState, TResult>(Tag, state, stackEvents, funcWithResult);

        }
    }

    internal class StackBlockSpecQuery<TState, Tin, TResult> : StackBlockSpecBase<TState>
    {
        Func<IQuery<TState, Tin>, BlockResult<TResult>> func;
        Func<ITypedQuery<TState, Tin, TResult>, BlockResult<TResult>> typedFunc;
        Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> funcWithResult;

        public StackBlockSpecQuery(string tag, int index, Func<IQuery<TState, Tin>, BlockResult<TResult>> func)
            : base(tag, index)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(string tag, int index, Func<ITypedQuery<TState, Tin, TResult>, BlockResult<TResult>> typedFunc)
            : base(tag, index)
        {
            this.typedFunc = typedFunc;
        }

        public StackBlockSpecQuery(string tag, int index, Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> func)
            : base(tag, index)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecQuery(int index, Func<IQuery<TState, Tin>, BlockResult<TResult>> func)
            : base(index)
        {
            this.func = func;
        }

        public StackBlockSpecQuery(int index, Func<ITypedQuery<TState, Tin, TResult>, BlockResult<TResult>> typedFunc)
            : base(index)
        {
            this.typedFunc = typedFunc;
        }

        public StackBlockSpecQuery(int index, Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> func)
            : base(index)
        {
            this.funcWithResult = func;
        }

        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, object input)
        {
            if (func != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, (Tin)input,func);
            else if (typedFunc != null)
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, (Tin)input, typedFunc);
            else
                return new Query<TState, Tin, TResult>(Tag, state, stackEvents, (Tin)input, funcWithResult);
        }
    }
}
