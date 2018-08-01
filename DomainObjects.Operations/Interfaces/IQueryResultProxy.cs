namespace DomainObjects.Operations
{
    public interface IQueryResultProxy<T,TState, TOperationEvent> : IResultDispatcher<T,TState, TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        T Result { get; set; }
    }
}
