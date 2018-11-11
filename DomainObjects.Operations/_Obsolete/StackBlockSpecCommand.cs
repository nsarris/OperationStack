//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DomainObjects.Operations
//{
//    internal class StackBlockSpecCommand<TInput,TState> : StackBlockSpecBase<TInput,TState>
//        where TOperationEvent : IOperationEvent
//    {
//        Func<ICommand<TInput,TState>, BlockResultVoid> func;
//        Action<ICommand<TInput,TState>> action;
//        Func<IOperationBlock<TInput,TState>, ICommandResult> funcWithResult;

//        Func<ICommand<TInput,TState>, Task<BlockResultVoid>> funcAsync;
//        Func<ICommand<TInput,TState>, Task> actionAsync;
//        Func<IOperationBlock<TInput,TState>, Task<ICommandResult>> funcWithResultAsync;

//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState>, ICommandResult> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TInput,TState>> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.action = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState>, ICommandResult> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(int index, Action<ICommand<TInput,TState>> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.action = action;
//        }



//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState>, Task<ICommandResult>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState>, Task> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState>, Task<ICommandResult>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState>, Task> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public override StackBlockBase<TInput,TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
//        {
//            if (func != null)
//                return new CommandBlock<TInput,TState>(Tag, state, stackEvents, func);
//            else if (action != null)
//                return new CommandBlock<TInput,TState>(Tag, state, stackEvents, action);
//            else if (funcWithResult != null)
//                return new CommandBlock<TInput,TState>(Tag, state, stackEvents, funcWithResult);
//            else if (funcAsync != null)
//                return new CommandBlock<TInput,TState>(Tag, state, stackEvents, funcAsync);
//            else if (actionAsync != null)
//                return new CommandBlock<TInput,TState>(Tag, state, stackEvents, actionAsync);
//            else
//                return new CommandBlock<TInput,TState>(Tag, state, stackEvents, funcWithResultAsync);
//        }
//    }

//    internal class StackBlockSpecCommand<TInput,TState, Tin> : StackBlockSpecBase<TInput,TState>
//        where TOperationEvent : IOperationEvent
//    {
//        Func<ICommand<TInput,TState, Tin>, BlockResultVoid> func;
//        Action<ICommand<TInput,TState, Tin>> action;
//        Func<IOperationBlock<TInput,TState, Tin>, ICommandResult> funcWithResult;

//        Func<ICommand<TInput,TState, Tin>, Task<BlockResultVoid>> funcAsync;
//        Func<ICommand<TInput,TState, Tin>, Task> actionAsync;
//        Func<IOperationBlock<TInput,TState, Tin>, Task<ICommandResult>> funcWithResultAsync;

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, Tin>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TInput,TState, Tin>> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.action = action;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState, Tin>, ICommandResult> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, Tin>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(int index, Action<ICommand<TInput,TState, Tin>> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.action = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState, Tin>, ICommandResult> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResult = func;
//        }




//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, Tin>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, Tin>, Task> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState, Tin>, Task<ICommandResult>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, Tin>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, Tin>, Task> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState, Tin>, Task<ICommandResult>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }


//        public override StackBlockBase<TInput,TState> CreateBlock(TState state, IStackEvents stackEvents, IEmptyable input)
//        {
//            var typedInput = input.ConvertTo<Tin>();

//            if (func != null)
//                return new CommandBlock<TInput,TState, Tin>(Tag, state, stackEvents, typedInput, func);
//            else if (action != null)
//                return new CommandBlock<TInput,TState, Tin>(Tag, state, stackEvents, typedInput, action);
//            else if (funcWithResult != null)
//                return new CommandBlock<TInput,TState, Tin>(Tag, state, stackEvents, typedInput, funcWithResult);
//            else if (funcAsync != null)
//                return new CommandBlock<TInput,TState, Tin>(Tag, state, stackEvents, typedInput, funcAsync);
//            else if (actionAsync != null)
//                return new CommandBlock<TInput,TState, Tin>(Tag, state, stackEvents, typedInput, actionAsync);
//            else
//                return new CommandBlock<TInput,TState, Tin>(Tag, state, stackEvents, typedInput, funcWithResultAsync);
//        }
//    }
//}
