namespace DomainObjects.Operations
{
    public enum IncompatibleInputTypeBehaviour
    {
        Exception,
        Fail,
        AssumeEmpty
    }
    public class OperationStackOptions
    {
        public bool TimeMeasurement { get; set; } = true;
        public bool FailOnException { get; set; } = false;
        public bool SupportsSync { get; set; } = true;
        public bool SupportsAsync { get; set; } = true;
        public bool PreferAsync { get; set; } = true;
        public IncompatibleInputTypeBehaviour IncompatibleInputTypeBehaviour { get; set; } = IncompatibleInputTypeBehaviour.Exception;
    }
}
