using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    

    public interface IOperationResult
    {
        List<OperationEvent> Errors { get; }
        object Result { get; }
        IOperationResult Append(IOperationResult result);
        void AppendTo(IOperationResult result);
    }

    public interface IOperationResult<T> : IOperationResult
    {
        new T Result { get; }
    }




    public abstract class OperationResult 
    {
        public List<OperationEvent> Errors { get; private set; } = new List<OperationEvent>();

        
        internal OperationResult()
        {

        }
        internal OperationResult(IOperationResult fromResult)
        {
            AppendResult(fromResult);
        }

        protected void AppendResult(IOperationResult result)
        {
            if (result != null)
                foreach(var r in result.Errors)
                    if (!this.Errors.Contains(r))
                        this.Errors.Add(r);
        }
    }
    
    //[DebuggerNonUserCode]
    public class CommandResult : OperationResult, IOperationResult
    {
        public CommandResult()
        {

        }
        public CommandResult(IOperationResult fromResult)
            :base(fromResult)
        {
            
        }

        object IOperationResult.Result => null;

        public IOperationResult Append(IOperationResult result)
        {
            AppendResult(result);
            return this;
        }

        public void AppendTo(IOperationResult result)
        {
            result.Append(this);
        }
    }

    public class QueryResult<T> : OperationResult, IOperationResult<T>
    {
        public QueryResult()
        {

        }
        public QueryResult(IOperationResult fromResult)
            :base(fromResult)
        {
            
        }

        internal QueryResult(IOperationResult fromResult, T result)
            : this(fromResult)
        {
            if (result != null)
                this.Result = result;
        }
        
        public T Result { get; set; }

        object IOperationResult.Result => Result;

        public IOperationResult Append(IOperationResult result)
        {
            AppendResult(result);
            return this;
        }

        public void AppendTo(IOperationResult result)
        {
            result.Append(this);
        }
    }
}
