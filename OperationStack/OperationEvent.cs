using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public abstract class OperationEvent
    {
        public string OperationTag { get; protected set; }
        public string Code { get; protected set; }
        public string Message { get; protected set; }
        
        public virtual string GetFullDescription()
        {
            if (!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Message))
                return Code + " - " + Message;
            else
                return (Code ?? "") + (Message ?? "");
        }

        protected OperationEvent(string operationTag, string code, string message)
        {
            this.OperationTag = operationTag;
            this.Code = Code;
            this.Message = message;
        }

        protected OperationEvent(string operationTag, string message)
        {
            this.OperationTag = operationTag;
            this.Code = "";
            this.Message = message;
        }
    }

    public enum OperationErrorSeverity
    {
        Message,
        Warning,
        NonCritical,
        Critical,
        Fatal
    }

    public enum OperationErrorType
    {
        User,
        Business,
        Validation,
        Security,
        Exception
    }

    public class OperationTraceMessage
    {
        public int Verbosity { get; set; }
    }

    public class OperationError : OperationEvent
    {
        public OperationErrorType ErrorType { get; protected set; }
        public Exception Exception { get; private set; }
        public bool Handled { get; set; }
        public OperationError(string operationTag, string code, string message, OperationErrorType errorType)
            :base(operationTag, code,message)
        {
            this.ErrorType = errorType;
        }

        public OperationError(string operationTag, string message, OperationErrorType errorType)
            : base(operationTag, message)
        {
            this.ErrorType = errorType;
        }
    
        public OperationError(string operationTag, Exception exception, bool handled = false)
            : base(operationTag, exception.Message)
        {
            this.Exception = exception;
            this.Message = exception.Message; //Recursive
            this.Handled = handled;
            this.ErrorType = OperationErrorType.Exception;
        }

        public OperationError(string operationTag, string message, Exception exception, bool handled = false)
            : base(operationTag, message)
        {
            this.Exception = exception;
            this.Message = exception.Message; //Recursive
            this.Handled = handled;
            this.ErrorType = OperationErrorType.Exception;
        }

        public OperationError(string operationTag, string code, string message, Exception exception, bool handled = false)
            :base(operationTag, code,message)
        {
            this.Exception = exception;
            this.Message = exception.Message; //Recursive
            this.Handled = handled;
            this.ErrorType = OperationErrorType.Exception;
        }
    }
}
