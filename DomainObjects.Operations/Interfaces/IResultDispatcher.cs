namespace DomainObjects.Operations
{
    public interface IResultDispatcher<T, TState>
        
    {
        //BlockResult<T> Return();
        BlockResult<T> Return(T result);
        BlockResult<T> Complete();
        BlockResult<T> Complete(object overrideResult);
        BlockResult<T> Fail();
        BlockResult<T> Fail(OperationEvent error);
        BlockResult<T> Reset();
        BlockResult<T> Reset(TState state);
        BlockResult<T> Restart();
        BlockResult<T> Goto(string tag);
        BlockResult<T> Goto(string tag, object overrideInput);
        BlockResult<T> Goto(int index);
        BlockResult<T> Goto(int index, object overrideInput);
        BlockResult<T> Skip(int i);
        BlockResult<T> Skip(int i, object overrideInput);
        BlockResult<T> Retry();
        BlockResult<T> Retry(object overrideInput);
    }

    public interface IResultVoidDispatcher<in TState>
        
    {
        BlockResultVoid Return();
        BlockResultVoid Complete();
        BlockResultVoid Complete(object overrideResult);
        BlockResultVoid Fail();
        BlockResultVoid Fail(OperationEvent error);
        BlockResultVoid Reset();
        BlockResultVoid Reset(TState state);
        BlockResultVoid Restart();
        BlockResultVoid Goto(string tag);
        BlockResultVoid Goto(string tag, object overrideInput);
        BlockResultVoid Goto(int index);
        BlockResultVoid Goto(int index, object overrideInput);
        BlockResultVoid Skip(int i);
        BlockResultVoid Skip(int i, object overrideInput);
        BlockResultVoid Retry();
        BlockResultVoid Retry(object overrideInput);
    }

    public interface IResultDispatcher<TState>
        
    {
        //BlockResult<T> Return<T>();
        BlockResult<T> Return<T>(T result);
        BlockResult<T> Complete<T>();
        BlockResult<T> Complete<T>(object overrideResult);
        BlockResult<T> Fail<T>();
        BlockResult<T> Fail<T>(OperationEvent error);
        BlockResult<T> Reset<T>();
        BlockResult<T> Reset<T>(TState state);
        BlockResult<T> Restart<T>();
        BlockResult<T> Goto<T>(string tag);
        BlockResult<T> Goto<T>(string tag, object overrideInput);
        BlockResult<T> Goto<T>(int index);
        BlockResult<T> Goto<T>(int index, object overrideInput);
        BlockResult<T> Skip<T>(int i);
        BlockResult<T> Skip<T>(int i, object overrideInput);
        BlockResult<T> Retry<T>();
        BlockResult<T> Retry<T>(object overrideInput);
    }

}
