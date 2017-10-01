using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    internal class StackBlockSpecCommand<TState> : StackBlockSpecBase<TState>
    {
        Func<ICommand<TState>, BlockResultVoid> func;
        Action<ICommand<TState>> action;
        Func<IOperationBlock<TState>, ICommandResult> funcWithResult;

        Func<ICommand<TState>, Task<BlockResultVoid>> funcAsync;
        Func<ICommand<TState>, Task> actionAsync;
        Func<IOperationBlock<TState>, Task<ICommandResult>> funcWithResultAsync;

        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TState>, ICommandResult> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState>, BlockResultVoid> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TState>> action, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.action = action;
        }

        public StackBlockSpecCommand(int index, Func<IOperationBlock<TState>, ICommandResult> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState>, BlockResultVoid> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(int index, Action<ICommand<TState>> action, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.action = action;
        }



        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TState>, Task<ICommandResult>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResultAsync = func;
        }

        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState>, Task> action, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.actionAsync = action;
        }

        public StackBlockSpecCommand(int index, Func<IOperationBlock<TState>, Task<ICommandResult>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResultAsync = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState>, Task> action, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.actionAsync = action;
        }

        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
        {
            if (func != null)
                return new Command<TState>(Tag, state, stackEvents, func);
            else if (action != null)
                return new Command<TState>(Tag, state, stackEvents, action);
            else if (funcWithResult != null)
                return new Command<TState>(Tag, state, stackEvents, funcWithResult);
            else if (funcAsync != null)
                return new Command<TState>(Tag, state, stackEvents, funcAsync);
            else if (actionAsync != null)
                return new Command<TState>(Tag, state, stackEvents, actionAsync);
            else
                return new Command<TState>(Tag, state, stackEvents, funcWithResultAsync);
        }
    }

    internal class StackBlockSpecCommand<TState, Tin> : StackBlockSpecBase<TState>
    {
        Func<ICommand<TState, Tin>, BlockResultVoid> func;
        Action<ICommand<TState, Tin>> action;
        Func<IOperationBlock<TState, Tin>, ICommandResult> funcWithResult;

        Func<ICommand<TState, Tin>, Task<BlockResultVoid>> funcAsync;
        Func<ICommand<TState, Tin>, Task> actionAsync;
        Func<IOperationBlock<TState, Tin>, Task<ICommandResult>> funcWithResultAsync;

        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState, Tin>, BlockResultVoid> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TState, Tin>> action, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.action = action;
        }

        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TState, Tin>, ICommandResult> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResult = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState, Tin>, BlockResultVoid> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.func = func;
        }

        public StackBlockSpecCommand(int index, Action<ICommand<TState, Tin>> action, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.action = action;
        }

        public StackBlockSpecCommand(int index, Func<IOperationBlock<TState, Tin>, ICommandResult> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResult = func;
        }




        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState, Tin>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TState, Tin>, Task> action, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.actionAsync = action;
        }

        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TState, Tin>, Task<ICommandResult>> func, BlockSpecTypes blockType)
            : base(tag, index, blockType)
        {
            this.funcWithResultAsync = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState, Tin>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcAsync = func;
        }

        public StackBlockSpecCommand(int index, Func<ICommand<TState, Tin>, Task> action, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.actionAsync = action;
        }

        public StackBlockSpecCommand(int index, Func<IOperationBlock<TState, Tin>, Task<ICommandResult>> func, BlockSpecTypes blockType)
            : base(index, blockType)
        {
            this.funcWithResultAsync = func;
        }


        public override StackBlockBase<TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
        {
            var typedInput = input.ConvertTo<Tin>();

            if (func != null)
                return new Command<TState, Tin>(Tag, state, stackEvents, typedInput, func);
            else if (action != null)
                return new Command<TState, Tin>(Tag, state, stackEvents, typedInput, action);
            else if (funcWithResult != null)
                return new Command<TState, Tin>(Tag, state, stackEvents, typedInput, funcWithResult);
            else if (funcAsync != null)
                return new Command<TState, Tin>(Tag, state, stackEvents, typedInput, funcAsync);
            else if (actionAsync != null)
                return new Command<TState, Tin>(Tag, state, stackEvents, typedInput, actionAsync);
            else
                return new Command<TState, Tin>(Tag, state, stackEvents, typedInput, funcWithResultAsync);
        }
    }
}
