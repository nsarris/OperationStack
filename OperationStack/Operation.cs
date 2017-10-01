using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public interface IOperation
    {
        //IOperationFlowResult Execute(string name, object inputData, IOperationEventLog stackEventLog);
        Operation CreateOperation(string name, IEmptyable inputData, IStackEventLog stackEventLog);
    }

    public interface ICommand : IOperation
    {

    }

    public interface IQuery<T> : IOperation
    {

    }

    
    public abstract class Operation
    {
        //public OperationFlow Flow { get; private set; }
        public string Name { get; set; }
        //public object StackData { get; set; }
        public IStackEventLog StackEventLog { get; private set; }
        public IOperationEventLog EventLog { get; private set; }

        public abstract IOperationFlowResult Execute();

        internal Operation(string name, IStackEventLog stackEventLog)
        {
            Name = name;
            StackEventLog = stackEventLog;
            EventLog = new OperationEventCollection();
        }
    }

    public class CommandOperation : Operation
    {
        //public OperationFlow Flow { get; private set; }
        private Func<CommandOperation, OperationFlowResultVoid> func;
        private Action<CommandOperation> action;

        protected CommandOperation(string name, IStackEventLog stackEventLog)
            : base(name, stackEventLog)
        {
            
        }

        internal CommandOperation(string name, Func<CommandOperation, OperationFlowResultVoid> func, IStackEventLog stackEventLog)
            : base(name, stackEventLog)
        {
            this.func = func;
        }

        internal CommandOperation(string name, Action<CommandOperation> action, IStackEventLog stackEventLog)
            : base(name, stackEventLog)
        {
            this.action = action;
        }

        public override IOperationFlowResult Execute()
        {
            if (func != null)
                return func(this);
            else
            {
                action(this);
                return new OperationFlowResultVoid(this, new OperationFlowNext());
            }
        }

        public OperationFlowResultVoid Return()
        {
            return new OperationFlowResultVoid(this, new OperationFlowNext());
        }

        public OperationFlowResultVoid Skip(int i)
        {
            return new OperationFlowResultVoid(this, new OperationFlowSkip(i));
        }

        //public IOperationFlowResult Skip(int i, object overrideResult)
        //{
        //    return new OperationFlowResultVoid(new OperationFlowSkip(i), overrideResult);
        //}
        
        public virtual OperationFlowResultVoid Retry()
        {
            return new OperationFlowResultVoid(this, new OperationFlowRetry(1));
        }

        //public OperationFlowResultVoid Retry(object overrideResult)
        //{
        //    return new OperationFlowResultVoid(new OperationFlowRetry(1), overrideResult);
        //}

        public OperationFlowResultVoid Goto(string tag, object overrideResult)
        {
            return new OperationFlowResultVoid(this, new OperationFlowTag(tag), overrideResult);
        }

        public OperationFlowResultVoid Goto(string tag)
        {
            return new OperationFlowResultVoid(this, new OperationFlowTag(tag));
        }

        public OperationFlowResultVoid End()
        {
            return new OperationFlowResultVoid(this, new OperationFlowEnd());
        }

        public OperationFlowResultVoid End(object overrideResult)
        {
            return new OperationFlowResultVoid(this, new OperationFlowEnd(), overrideResult);
        }

        public OperationFlowResultVoid Break()
        {
            return new OperationFlowResultVoid(this, new OperationFlowBreak());
        }
    }

    public class CommandOperation<Tin> : CommandOperation
    {
        private Func<CommandOperation<Tin>, OperationFlowResultVoid> func;
        private Action<CommandOperation<Tin>> action;

        public Emptyable<Tin> Input { get; private set; }
        internal CommandOperation(string name, Func<CommandOperation<Tin>, OperationFlowResultVoid> func, IStackEventLog stackEventLog, Emptyable<Tin> input)
            : base(name, stackEventLog)
        {
            this.func = func;
            Input = input;
        }

        internal CommandOperation(string name, Action<CommandOperation<Tin>> action, IStackEventLog stackEventLog, Emptyable<Tin> input)
            : base(name, stackEventLog)
        {
            this.action = action;
            Input = input;
        }

        public override IOperationFlowResult Execute()
        {
            if (func != null)
                return func(this);
            else
            {
                action(this);
                return new OperationFlowResultVoid(this, new OperationFlowNext());
            }
        }

        public override OperationFlowResultVoid Retry()
        {
            return new OperationFlowResultVoid(this, new OperationFlowRetry(1), Input);
        }

        public OperationFlowResultVoid Retry(Tin overrideResult)
        {
            return new OperationFlowResultVoid(this, new OperationFlowRetry(1), overrideResult);
        }
    }

    public class QueryOperation<Tout> : Operation
    {
        private Func<QueryOperation<Tout>, OperationFlowResult<Tout>> func;
        //public OperationFlow<Tout> Flow { get; private set; }
        //public Tout Result { get; set; }
        protected QueryOperation(string name, IStackEventLog stackEventLog)
            : base(name, stackEventLog)
        {

        }

        internal QueryOperation(string name, Func<QueryOperation<Tout>, OperationFlowResult<Tout>> func, IStackEventLog stackEventLog)
            :base(name, stackEventLog)
        {
            this.func = func;
        }

        public override IOperationFlowResult Execute()
        {
            return func(this);
        }
        

        public OperationFlowResult<Tout> Return(Tout result)
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowNext(),result: result);
        }

        public OperationFlowResult<Tout> Next()
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowNext());
        }

        public OperationFlowResult<Tout> Skip(int i)
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowSkip(i));
        }

        public OperationFlowResult<Tout> Skip(int i, object overrideResult)
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowSkip(i), redirectResult: overrideResult);
        }

        public virtual OperationFlowResult<Tout> Retry()
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowRetry(1));
        }

        //public OperationFlowResult<Tout> Retry(object overrideResult)
        //{
        //    return new OperationFlowResult<Tout>(new OperationFlowRetry(1), overrideResult);
        //}

        public OperationFlowResult<Tout> Goto(string tag, object overrideResult)
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowTag(tag), redirectResult: overrideResult);
        }

        public OperationFlowResult<Tout> Goto(string tag)
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowTag(tag));
        }

        public OperationFlowResult<Tout> End()
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowEnd());
        }

        public OperationFlowResult<Tout> End(object overrideResult)
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowEnd(), redirectResult: overrideResult);
        }

        public OperationFlowResult<Tout> Break()
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowBreak());
        }
    }

    public class QueryOperation<Tin,Tout> : QueryOperation<Tout>
    {
        private Func<QueryOperation<Tin,Tout>, OperationFlowResult<Tout>> func;
        public Emptyable<Tin> Input { get; set; }
        internal QueryOperation(string name, Func<QueryOperation<Tin, Tout>, OperationFlowResult<Tout>> func, IStackEventLog stackEventLog, Emptyable<Tin> input)
            : base(name, stackEventLog)
        {
            this.func = func;
            Input = input;
            //Flow = new OperationFlow<Tout>(input);
        }

        public override IOperationFlowResult Execute()
        {
            return func(this);
        }

        public override OperationFlowResult<Tout> Retry()
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowRetry(1), redirectResult: Input);
        }

        public OperationFlowResult<Tout> Retry(Tin overrideResult)
        {
            return new OperationFlowResult<Tout>(this, new OperationFlowRetry(1), redirectResult: overrideResult);
        }
    }



    //[DebuggerNonUserCode]
    internal class Command : ICommand
    {
        private Func<CommandOperation, OperationFlowResultVoid> func;
        private Action<CommandOperation> action;

        public Command(Func<CommandOperation, OperationFlowResultVoid> func)
        {
            this.func = func;
        }

        public Command(Action<CommandOperation> action)
        {
            this.action = action;
        }

        public Operation CreateOperation(string name, IEmptyable inputData, IStackEventLog stackEventLog)
        {
            if (func != null)
                return new CommandOperation(name, func, stackEventLog);
            else
                return new CommandOperation(name, action, stackEventLog);
        }

        public OperationFlowResultVoid Execute(CommandOperation op)
        {
            if (func != null)
                return func(op);
            else
            {
                action(op);
                return new OperationFlowResultVoid(op, new OperationFlowNext());
            }
        }
    }

    //[DebuggerNonUserCode]
    internal class Command<T> : ICommand
    {
        private Func<CommandOperation<T>, OperationFlowResultVoid> func;
        private Action<CommandOperation<T>> action;

        public Command(Func<CommandOperation<T>, OperationFlowResultVoid> func)
        {
            this.func = func;
        }

        public Command(Action<CommandOperation<T>> action)
        {
            this.action = action;
        }

        public Operation CreateOperation(string name, IEmptyable inputData, IStackEventLog stackEventLog)
        {
            Emptyable<T> e = inputData.ConvertTo<T>();
            if (func != null)
                return new CommandOperation<T>(name, func, stackEventLog, e);
            else
                return new CommandOperation<T>(name, action, stackEventLog, e);
        }
    }

    internal class Query<T> : IQuery<T>
    {
        private Func<QueryOperation<T>, OperationFlowResult<T>> func;
        

        public Query(Func<QueryOperation<T>, OperationFlowResult<T>> func)
        {
            this.func = func;
        }

        public Operation CreateOperation(string name, IEmptyable inputData, IStackEventLog stackEventLog)
        {
            return new QueryOperation<T>(name, func, stackEventLog);
        }
    }

    internal class Query<Tin, Tout> : IQuery<Tout>
    {
        private Func<QueryOperation<Tin, Tout>, OperationFlowResult<Tout>> func;
        
        public Query(Func<QueryOperation<Tin, Tout>, OperationFlowResult<Tout>> func)
        {
            this.func = func;
        }

        public Operation CreateOperation(string name, IEmptyable inputData, IStackEventLog stackEventLog)
        {
            Emptyable<Tin> e = inputData.ConvertTo<Tin>();
            return new QueryOperation<Tin, Tout>(name,func, stackEventLog, e);
        }
    }
}

