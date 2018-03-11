namespace DomainObjects.Operations
{
    public interface IQueryResultProxy<T,TState> : IResultDispatcher<T,TState>
    {
        T Result { get; set; }
    }

  

   




}
