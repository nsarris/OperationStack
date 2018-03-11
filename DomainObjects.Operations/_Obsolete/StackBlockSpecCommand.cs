//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DomainObjects.Operations
//{
//    internal class StackBlockSpecCommand<TInput,TState, TOperationEvent> : StackBlockSpecBase<TInput,TState, TOperationEvent>
//        where TOperationEvent : IOperationEvent
//    {
//        Func<ICommand<TInput,TState, TOperationEvent>, BlockResultVoid> func;
//        Action<ICommand<TInput,TState, TOperationEvent>> action;
//        Func<IOperationBlock<TInput,TState, TOperationEvent>, ICommandResult<TOperationEvent>> funcWithResult;

//        Func<ICommand<TInput,TState, TOperationEvent>, Task<BlockResultVoid>> funcAsync;
//        Func<ICommand<TInput,TState, TOperationEvent>, Task> actionAsync;
//        Func<IOperationBlock<TInput,TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> funcWithResultAsync;

//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, ICommandResult<TOperationEvent>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TInput,TState, TOperationEvent>> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.action = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, ICommandResult<TOperationEvent>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, TOperationEvent>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(int index, Action<ICommand<TInput,TState, TOperationEvent>> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.action = action;
//        }



//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent>, Task> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, TOperationEvent>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, TOperationEvent>, Task> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public override StackBlockBase<TInput,TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
//        {
//            if (func != null)
//                return new CommandBlock<TInput,TState, TOperationEvent>(Tag, state, stackEvents, func);
//            else if (action != null)
//                return new CommandBlock<TInput,TState, TOperationEvent>(Tag, state, stackEvents, action);
//            else if (funcWithResult != null)
//                return new CommandBlock<TInput,TState, TOperationEvent>(Tag, state, stackEvents, funcWithResult);
//            else if (funcAsync != null)
//                return new CommandBlock<TInput,TState, TOperationEvent>(Tag, state, stackEvents, funcAsync);
//            else if (actionAsync != null)
//                return new CommandBlock<TInput,TState, TOperationEvent>(Tag, state, stackEvents, actionAsync);
//            else
//                return new CommandBlock<TInput,TState, TOperationEvent>(Tag, state, stackEvents, funcWithResultAsync);
//        }
//    }

//    internal class StackBlockSpecCommand<TInput,TState, TOperationEvent, Tin> : StackBlockSpecBase<TInput,TState, TOperationEvent>
//        where TOperationEvent : IOperationEvent
//    {
//        Func<ICommand<TInput,TState, TOperationEvent, Tin>, BlockResultVoid> func;
//        Action<ICommand<TInput,TState, TOperationEvent, Tin>> action;
//        Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, ICommandResult<TOperationEvent>> funcWithResult;

//        Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task<BlockResultVoid>> funcAsync;
//        Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task> actionAsync;
//        Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, Task<ICommandResult<TOperationEvent>>> funcWithResultAsync;

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Action<ICommand<TInput,TState, TOperationEvent, Tin>> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.action = action;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, ICommandResult<TOperationEvent>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResult = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, BlockResultVoid> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.func = func;
//        }

//        public StackBlockSpecCommand(int index, Action<ICommand<TInput,TState, TOperationEvent, Tin>> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.action = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, ICommandResult<TOperationEvent>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResult = func;
//        }




//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task> action, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public StackBlockSpecCommand(string tag, int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, Task<ICommandResult<TOperationEvent>>> func, BlockSpecTypes blockType)
//            : base(tag, index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task<BlockResultVoid>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcAsync = func;
//        }

//        public StackBlockSpecCommand(int index, Func<ICommand<TInput,TState, TOperationEvent, Tin>, Task> action, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.actionAsync = action;
//        }

//        public StackBlockSpecCommand(int index, Func<IOperationBlock<TInput,TState, TOperationEvent, Tin>, Task<ICommandResult<TOperationEvent>>> func, BlockSpecTypes blockType)
//            : base(index, blockType)
//        {
//            this.funcWithResultAsync = func;
//        }


//        public override StackBlockBase<TInput,TState, TOperationEvent> CreateBlock(TState state, IStackEvents<TOperationEvent> stackEvents, IEmptyable input)
//        {
//            var typedInput = input.ConvertTo<Tin>();

//            if (func != null)
//                return new CommandBlock<TInput,TState, TOperationEvent, Tin>(Tag, state, stackEvents, typedInput, func);
//            else if (action != null)
//                return new CommandBlock<TInput,TState, TOperationEvent, Tin>(Tag, state, stackEvents, typedInput, action);
//            else if (funcWithResult != null)
//                return new CommandBlock<TInput,TState, TOperationEvent, Tin>(Tag, state, stackEvents, typedInput, funcWithResult);
//            else if (funcAsync != null)
//                return new CommandBlock<TInput,TState, TOperationEvent, Tin>(Tag, state, stackEvents, typedInput, funcAsync);
//            else if (actionAsync != null)
//                return new CommandBlock<TInput,TState, TOperationEvent, Tin>(Tag, state, stackEvents, typedInput, actionAsync);
//            else
//                return new CommandBlock<TInput,TState, TOperationEvent, Tin>(Tag, state, stackEvents, typedInput, funcWithResultAsync);
//        }
//    }
//}
