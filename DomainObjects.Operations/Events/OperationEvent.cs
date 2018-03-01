using System;

namespace DomainObjects.Operations
{
    public enum OperationEventStatusEnum
    {
        Initial,
        Unhandled,
        Handled,
        Swallowed,
        Replaced
    }

    public enum OperationEventSeverity
    {
        Message,
        Warning,
        Error
    }

    public class OperationEvent 
    {
        public bool IsHandled { get; private set; }

        public bool IsError { get; protected set; }

        public bool IsException => Exception != null;

        public Exception Exception { get; private set; }

        //public bool UnhandledException { get; private set; }

        public string Message { get; protected set; }

        public string UserMessage { get; set; }

        public OperationEvent(Exception exception)
        {
            //IsHandled = false;
            IsError = true;
            Exception = exception;
            Message = exception.Message;
            UserMessage = exception.Message;
        }

        public OperationEvent(string message)
        {
            Message = message;
            UserMessage = message;
        }

        public void Handle()
        {
            IsHandled = true;
        }

        public void Throw()
        {
            if (!IsError)
                throw new Exception("Cannot throw an OperationEvent that is not an error");
            IsHandled = false;
        }

        public virtual void FromException(Exception e)
        {
            this.Exception = e;
        }
    }
}
