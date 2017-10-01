using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    

    internal class OperationFlowException : Exception
    {
        public OperationFlowException()
            :base()
        {

        }
        public OperationFlowException(string message)
            :base(message)
        {

        }
    }

    internal class OperationFlowTagNotFoundException : OperationFlowException
    {
        public OperationFlowTagNotFoundException(string tag)
            :base("Block with tag " + tag + "not found")
        {

        }
    }

    public interface IOperationFlowResult
    {
        //internal OperationFlowTarget Target { get; set; }
        IEmptyable Result { get; }
    }

    public abstract class OperationFlowResultBase
    {
        internal OperationFlowTarget Target { get; set; }
        internal IEmptyable RedirectResult { get; set; }
        public Operation Operation { get; set; }
    }

    public class OperationFlowResultVoid : OperationFlowResultBase,IOperationFlowResult
    {
        //internal OperationFlowTarget Target { get; private set; }
        
        IEmptyable IOperationFlowResult.Result => new Emptyable<object>();

        internal OperationFlowResultVoid(Operation operation, OperationFlowTarget target, object redirectResult)
        {
            Target = target;
            RedirectResult = new Emptyable<object>(redirectResult);
            Operation = operation;
        }

        internal OperationFlowResultVoid(Operation operation, OperationFlowTarget target)
        {
            Target = target;
            RedirectResult = new Emptyable<object>();
            Operation = operation;
        }
    }

   
    public class OperationFlowResult<T> : OperationFlowResultBase, IOperationFlowResult
    {
        //internal OperationFlowTarget Target { get; private set; }
        public Emptyable<T> Result { get; private set; }

        IEmptyable IOperationFlowResult.Result => Result;

        internal OperationFlowResult(Operation operation, OperationFlowTarget target)
        {
            this.Target = target;
            Operation = operation;
            this.RedirectResult = new Emptyable<object>();
        }

        internal OperationFlowResult(Operation operation, OperationFlowTarget target, T result)
        {
            this.Target = target;
            this.Result = result;
            this.RedirectResult = new Emptyable<object>();
            Operation = operation;
        }

        internal OperationFlowResult(Operation operation, OperationFlowTarget target, object redirectResult)
        {
            this.Target = target;
            this.RedirectResult  = new Emptyable<object>(redirectResult);
            Operation = operation;
        }
    }

    //public class OperationFlowResult: OperationFlowResult<object>
    //{
    //    internal OperationFlowResult(Operation operation,OperationFlowTarget target, object result)
    //        :base(operation,target, result)
    //    {

    //    }
    //}
    //public class OperationFlow 
    //{
    //    //private OperationFlowTarget Target { get; set; } = new OperationFlowNext();
    //    protected object InputData { get; set; }
    //    //internal object OutputData { get; set; }
    //    //TODO: unset/default inputdata

    //    public OperationFlow()
    //    {

    //    }

    //    public OperationFlow(object inputData)
    //    {
    //        this.InputData = inputData;
    //    }

        
    //    public OperationFlowResultVoid Return()
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowNext());
    //    }

    //    public OperationFlowResultVoid Skip(int i)
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowSkip(i));
    //    }

    //    //public IOperationFlowResult Skip(int i, object overrideResult)
    //    //{
    //    //    return new OperationFlowResultVoid(new OperationFlowSkip(i), overrideResult);
    //    //}

    //    //internal void Reset()
    //    //{
    //    //    Next();
    //    //    OutputData = null;
    //    //    OutputDataSet = false;
    //    //}
        
    //    public OperationFlowResultVoid Retry()
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowRetry(1), InputData);
    //    }

    //    public OperationFlowResultVoid Retry(object overrideResult)
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowRetry(1), overrideResult);
    //    }

    //    public OperationFlowResultVoid Goto(string tag, object overrideResult)
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowTag(tag), overrideResult);
    //    }

    //    public OperationFlowResultVoid Goto(string tag)
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowTag(tag));
    //    }

    //    public OperationFlowResultVoid End()
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowEnd());
    //    }

    //    public OperationFlowResultVoid End(object overrideResult)
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowEnd(), overrideResult);
    //    }

    //    public OperationFlowResultVoid Break()
    //    {
    //        return new OperationFlowResultVoid(new OperationFlowBreak());
    //    }
    //}

    //public class OperationFlow<T>
    //{
        
    //    protected object InputData { get; set; }
    //    //internal object OutputData { get; set; }
    //    //TODO: unset/default inputdata

    //    public OperationFlow()
    //    {

    //    }
    //    public OperationFlow(object inputData)
    //    {
    //        this.InputData = inputData;
    //    }

    //    public OperationFlowResult<T> Return(T result)
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowNext(), result);
    //    }
        
    //    public OperationFlowResult<T> Next()
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowNext());
    //    }

    //    public OperationFlowResult<T> Skip(int i)
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowSkip(i));
    //    }

    //    public OperationFlowResult<T> Skip(int i, object overrideResult)
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowSkip(i), overrideResult);
    //    }

    //    public OperationFlowResult<T> Retry()
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowRetry(1), InputData);
    //    }

    //    public OperationFlowResult<T> Retry(object overrideResult)
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowRetry(1), overrideResult);
    //    }

    //    public OperationFlowResult<T> Goto(string tag, object overrideResult)
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowTag(tag), overrideResult);
    //    }

    //    public OperationFlowResult<T> Goto(string tag)
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowTag(tag));
    //    }

    //    public OperationFlowResult<T> End()
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowEnd());
    //    }

    //    public OperationFlowResult<T> End(object overrideResult)
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowEnd(), overrideResult);
    //    }

    //    public OperationFlowResult<T> Break()
    //    {
    //        return new OperationFlowResult<T>(new OperationFlowBreak());
    //    }

    //    //internal OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
    //    //{
    //    //    var o = Target.GetNext(operations, currentOperation);
    //    //    //Reset();
    //    //    return o;
    //    //}
    //}

    internal abstract class OperationFlowTarget
    {
        internal abstract OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation);
    }

   

    internal class OperationFlowThis : OperationFlowTarget
    {
        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
        {
            return operations[currentOperation];
        }
    }

    internal class OperationFlowNext : OperationFlowTarget
    {
        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
        {
            var nextIndex = currentOperation + 1;
            if (nextIndex >= operations.Count)
                return null;
            else
                return operations[nextIndex];
        }
    }

    internal class OperationFlowTag : OperationFlowTarget
    {
        public string Tag { get; private set; }
        public OperationFlowTag(string tag)
        {
            this.Tag = tag;
        }

        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
        {
            var block = operations.Where(x => x.Name == Tag).FirstOrDefault();
            if (block == null) throw new OperationFlowTagNotFoundException(Tag);
            return block;
        }
    }

    internal class OperationFlowSkip : OperationFlowTarget
    {
        public int Skip { get; private set; }
        public OperationFlowSkip(int skip)
        {
            this.Skip = skip;
        }
        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
        {
            var nextIndex = currentOperation + Skip;
            if (nextIndex >= operations.Count)
                return null;
            else
                return operations[nextIndex];
        }
    }

    internal class OperationFlowRetry : OperationFlowTarget
    {
        public int Times { get; private set; }
        public OperationFlowTarget Then { get; private set; }

        public OperationFlowRetry(int times)
            : this(times, null)
        {

        }
        public OperationFlowRetry(int times, OperationFlowTarget then)
        {
            Then = then ?? (then = new OperationFlowNext());
            this.Times = times;
        }

        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
        {
            if (Times <= 0)
                return Then.GetNext(operations, currentOperation);
            else
                return operations[currentOperation];
        }
    }

    internal class OperationFlowEnd : OperationFlowTarget
    {
        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
        {
            return operations.Where(x => x.Finally).FirstOrDefault();
        }
    }

    internal class OperationFlowBreak : OperationFlowTarget
    {
        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
        {
            return null;
        }
    }
}
