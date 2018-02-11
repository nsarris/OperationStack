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

    public class OperationEvent : IOperationEvent
    {
        public bool IsHandled { get; set; }

        public bool IsError { get; protected set; }

        public bool IsException => Exception != null;

        public Exception Exception { get; private set; }

        //public bool UnhandledException { get; private set; }

        public string Message { get; protected set; }

        public string UserMessage { get; set; }

        public OperationEvent(Exception exception, bool unhandled = false)
        {
            Exception = exception;
            IsError = true;
            IsHandled = !unhandled;
        }

        public OperationEvent(string message)
        {
            this.Message = message;
            this.UserMessage = message;
        }
    }
}
