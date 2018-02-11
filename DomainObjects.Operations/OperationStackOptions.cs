namespace DomainObjects.Operations
{
    public class OperationStackOptions
    {
        public bool TimeMeasurement { get; set; } = true;
        public bool EndOnException { get; set; } = false;
        public bool SupportsSync { get; set; } = true;
        public bool SupportsAsync { get; set; } = true;
        public bool PreferAsync { get; set; } = true;
    }
}
