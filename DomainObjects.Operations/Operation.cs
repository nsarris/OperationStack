using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    
    public abstract class StackBlockBase<TState, TOperationEvent> : IStackBlock<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent

    {
        public string Tag { get; private set; }
        public TState StackState { get; private set; }
        public IStackEvents<TOperationEvent> StackEvents { get; set; }
        public IOperationEvents<TOperationEvent> Events { get; private set; } = new OperationEvents<TOperationEvent>();
        public bool IsEmptyEventBlock { get; protected set; }
        public bool IsAsync => executorAsync != null;
        public IEnumerable<BlockTraceResult<TOperationEvent>> InnerStackTrace { get; private set; } = new List<BlockTraceResult<TOperationEvent>>();
        private Func<Exception, TOperationEvent> unhandledExceptionEventBuilder;
        public StackBlockBase(string tag, TState state, IStackEvents<TOperationEvent> stackEvents)
        {
            Tag = tag;
            StackState = state;
            StackEvents = stackEvents;

            var ctor = typeof(TOperationEvent).GetConstructor(new Type[] { typeof(Exception), typeof(bool) });
            var param1 = Expression.Parameter(typeof(Exception));
            var l = Expression.Lambda<Func<Exception, TOperationEvent>>(Expression.New(ctor, param1, Expression.Constant(true)), param1);
            unhandledExceptionEventBuilder = l.Compile();
        }
        
        protected Func<IBlockResult> executor;
        protected Func<Task<IBlockResult>> executorAsync;

        public void Append(IOperationResult<TOperationEvent> result)
        {
            this.Events.Append(result.Events);
            ((List<BlockTraceResult<TOperationEvent>>)this.InnerStackTrace).AddRange(result.StackTrace);
        }
        
        internal IBlockResult Execute(bool measureTime = true)
        {
            System.Diagnostics.Stopwatch sw = null;
            if (measureTime)
            {
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
            }

            try
            {
                var result = IsAsync ? executorAsync().Result : executor();
                if (measureTime)
                {
                    sw.Stop();
                    ((BlockResultBase)result).ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
                }
                return result;
            }
            catch(Exception e)
            {
                this.Events.Add(unhandledExceptionEventBuilder(e));
                var result = new BlockResultVoid()
                {
                    Target = new BlockResultTarget
                    {
                        FlowTarget = BlockFlowTargets.Return
                    },
                };
                if (measureTime)
                    result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);

                return result;
            }
        }

        internal async Task<IBlockResult> ExecuteAsync(bool measureTime = true)
        {
            System.Diagnostics.Stopwatch sw = null;
            if (measureTime)
            {
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
            }

            try
            {
                var result = IsAsync ? await executorAsync() : executor();
                if (measureTime)
                {
                    sw.Stop();
                    ((BlockResultBase)result).ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);
                }
                return result;
            }
            catch (Exception e)
            {
                this.Events.Add(unhandledExceptionEventBuilder(e));
                var result = new BlockResultVoid()
                {
                    Target = new BlockResultTarget
                    {
                        FlowTarget = BlockFlowTargets.Return
                    },
                };
                if (measureTime)
                    result.ExecutionTime = new ExecutionTime(DateTime.Now, sw.Elapsed);

                return result;
            }
        }
    }

    //public abstract class OperationBlockBase<TState> : StackBlockBase<TState>, IOperationBlock<TState>
    //{
    //    public IOperationEvents Events { get; private set; } = new OperationEvents();

    //    public OperationBlockBase(string tag, TState state, IStackEvents stackEvents)
    //        : base(tag, state, stackEvents)
    //    {

    //    }
    //}

    public class Command<TState, TOperationEvent> : StackBlockBase<TState, TOperationEvent>, ICommand<TState, TOperationEvent>
        where TOperationEvent : IOperationEvent
    {
        private ResultVoidDispatcher resultDispacther = new ResultVoidDispatcher();
        
        protected Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents)
            : base(tag, state, stackEvents)
        {

        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, BlockResultVoid> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }
        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Action<ICommand<TState, TOperationEvent>> action)
            : base(tag, state, stackEvents)
        {
            executor = () => { action(this); return resultDispacther.Return(); };
        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, ICommandResult<TOperationEvent>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispacther.Return(); };
        }


        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, Task<BlockResultVoid>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this);
        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ICommand<TState, TOperationEvent>, Task> actionAsync)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { await actionAsync(this); return resultDispacther.Return(); };
        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, Task<ICommandResult<TOperationEvent>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this); this.Append(r); return resultDispacther.Return(); };
        }

        BlockResultVoid IResultVoidDispatcher.Break(bool success)
        {
            return resultDispacther.Break(success);
        }

        BlockResultVoid IResultVoidDispatcher.End(bool success)
        {
            return resultDispacther.End(success);
        }

        BlockResultVoid IResultVoidDispatcher.End(bool success , object overrideResult)
        {
            return resultDispacther.End(success, overrideResult);
        }

        BlockResultVoid IResultVoidDispatcher.Reset()
        {
            return resultDispacther.Reset();
        }

        BlockResultVoid IResultVoidDispatcher.Reset(object state)
        {
            return resultDispacther.Reset(state);
        }

        BlockResultVoid IResultVoidDispatcher.Restart()
        {
            return resultDispacther.Restart();
        }

        BlockResultVoid IResultVoidDispatcher.Return(bool success)
        {
            return resultDispacther.Return(success);
        }

        BlockResultVoid IResultVoidDispatcher.Return()
        {
            return resultDispacther.Return();
        }

        BlockResultVoid IResultVoidDispatcher.Goto(string tag, bool success)
        {
            return resultDispacther.Goto(tag, success);
        }

        BlockResultVoid IResultVoidDispatcher.Goto(string tag)
        {
            return resultDispacther.Goto(tag);
        }

        BlockResultVoid IResultVoidDispatcher.Goto(string tag, object overrideInput, bool success)
        {
            return resultDispacther.Goto(tag, overrideInput, success);
        }

        BlockResultVoid IResultVoidDispatcher.Goto(string tag, object overrideInput)
        {
            return resultDispacther.Goto(tag, overrideInput);
        }

        BlockResultVoid IResultVoidDispatcher.Skip(int i, bool success)
        {
            return resultDispacther.Skip(i, success);
        }

        BlockResultVoid IResultVoidDispatcher.Skip(int i)
        {
            return resultDispacther.Skip(i);
        }

        BlockResultVoid IResultVoidDispatcher.Skip(int i, object overrideInput, bool success)
        {
            return resultDispacther.Skip(i, overrideInput,success);
        }

        BlockResultVoid IResultVoidDispatcher.Skip(int i, object overrideInput)
        {
            return resultDispacther.Skip(i, overrideInput);
        }
    }

    public class Command<TState, TOperationEvent, Tin> : Command<TState, TOperationEvent>, ICommand<TState, TOperationEvent,Tin>
        where TOperationEvent : IOperationEvent
    {
        public Emptyable<Tin> Input { get; private set; }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TState, TOperationEvent,Tin>, BlockResultVoid> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Action<ICommand<TState, TOperationEvent,Tin>> action)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { action(this); return ((IResultVoidDispatcher)this).Return(); };
        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent,Tin>, ICommandResult<TOperationEvent>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultVoidDispatcher)this).Return(); };
        }


        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TState, TOperationEvent,Tin>, Task<BlockResultVoid>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this); 
        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ICommand<TState, TOperationEvent,Tin>, Task> action)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { await action(this); return ((IResultVoidDispatcher)this).Return(); };
        }

        internal Command(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent,Tin>, Task<ICommandResult<TOperationEvent>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this); this.Append(r); return ((IResultVoidDispatcher)this).Return(); };
        }


    }

    public class Query<TState, TOperationEvent,TResult> : StackBlockBase<TState, TOperationEvent>, IQuery<TState, TOperationEvent>, ITypedQuery<TState, TOperationEvent, TResult>
        where TOperationEvent : IOperationEvent
    {
        private ResultDispatcher<TResult> resultDispatcher = new ResultDispatcher<TResult>();
                protected Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents)
            : base(tag, state, stackEvents)
        {
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IQuery<TState, TOperationEvent>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ITypedQuery<TState, TOperationEvent, TResult>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, IQueryResult<TOperationEvent,TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IQuery<TState, TOperationEvent>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<ITypedQuery<TState, TOperationEvent, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Func<IOperationBlock<TState, TOperationEvent>, Task<IQueryResult<TOperationEvent,TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }




        IQueryResultProxy<T> IQuery<TState, TOperationEvent>.DefineResult<T>()
        {
            return new QueryResultProxy<T>();
        }

        IQueryResultProxy<T> IQuery<TState, TOperationEvent>.DefineResult<T>(T result)
        {
            return new QueryResultProxy<T>() { Result = result };
        }

        IQueryResultProxy<T> IQuery<TState, TOperationEvent>.DefineResult<T>(Expression<Func<T>> expression)
        {
            return new QueryResultProxy<T>();
        }



        BlockResult<T> IResultDispatcher.Break<T>(bool success)
        {
            return new ResultDispatcher<T>().Break(success);
        }

        BlockResult<T> IResultDispatcher.End<T>(bool success)
        {
            return new ResultDispatcher<T>().End(success);
        }

        BlockResult<T> IResultDispatcher.End<T>(bool success, object overrideResult)
        {
            return new ResultDispatcher<T>().End(success, overrideResult);
        }

        

        BlockResult<T> IResultDispatcher.Reset<T>()
        {
            return new ResultDispatcher<T>().Reset();
        }

        BlockResult<T> IResultDispatcher.Reset<T>(object state)
        {
            return new ResultDispatcher<T>().Reset(state);
        }

        BlockResult<T> IResultDispatcher.Restart<T>()
        {
            return new ResultDispatcher<T>().Restart();
        }

        BlockResult<T> IResultDispatcher.Return<T>(T result, bool success)
        {
            return new ResultDispatcher<T>().Return(result, success);
        }

        BlockResult<T> IResultDispatcher.Return<T>(T result)
        {
            return new ResultDispatcher<T>().Return(result);
        }

        BlockResult<T> IResultDispatcher.Goto<T>(string tag, bool success)
        {
            return new ResultDispatcher<T>().Goto(tag, success);
        }

        BlockResult<T> IResultDispatcher.Goto<T>(string tag)
        {
            return new ResultDispatcher<T>().Goto(tag);
        }

        BlockResult<T> IResultDispatcher.Goto<T>(string tag, object overrideInput, bool success)
        {
            return new ResultDispatcher<T>().Goto(tag, overrideInput, success);
        }

        BlockResult<T> IResultDispatcher.Goto<T>(string tag, object overrideInput)
        {
            return new ResultDispatcher<T>().Goto(tag, overrideInput);
        }

        BlockResult<T> IResultDispatcher.Skip<T>(int i, bool success)
        {
            return new ResultDispatcher<T>().Skip(i, success);
        }

        BlockResult<T> IResultDispatcher.Skip<T>(int i)
        {
            return new ResultDispatcher<T>().Skip(i);
        }

        BlockResult<T> IResultDispatcher.Skip<T>(int i, object overrideInput, bool success)
        {
            return new ResultDispatcher<T>().Skip(i, overrideInput, success);
        }

        BlockResult<T> IResultDispatcher.Skip<T>(int i, object overrideInput)
        {
            return new ResultDispatcher<T>().Skip(i, overrideInput);
        }



        BlockResult<TResult> IResultDispatcher<TResult>.Return(TResult result, bool success)
        {
            return resultDispatcher.Return(result, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Return(TResult result)
        {
            return resultDispatcher.Return(result);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.End(bool success)
        {
            return resultDispatcher.End(success);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.End(bool success, object overrideResult)
        {
            return resultDispatcher.End(success, overrideResult);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Break(bool success)
        {
            return resultDispatcher.Break(success);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Reset()
        {
            return resultDispatcher.Reset();
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Reset(object state)
        {
            return resultDispatcher.Reset(state);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Restart()
        {
            return resultDispatcher.Restart();
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Goto(string tag, bool success)
        {
            return resultDispatcher.Goto(tag, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Goto(string tag, object overrideInput, bool success)
        {
            return resultDispatcher.Goto(tag, overrideInput, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Skip(int i, bool success)
        {
            return resultDispatcher.Skip(i, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Skip(int i, object overrideInput, bool success)
        {
            return resultDispatcher.Skip(i, overrideInput, success);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }
    }

    public class Query<TState, TOperationEvent, Tin, TResult> : Query<TState, TOperationEvent, TResult>, IQuery<TState, TOperationEvent, Tin>, ITypedQuery<TState, TOperationEvent, Tin,TResult>
        where TOperationEvent : IOperationEvent
    {
        public Emptyable<Tin> Input { get; private set; }
        
        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IQuery<TState, TOperationEvent, Tin>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TState, TOperationEvent, Tin,TResult>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent, Tin>, IQueryResult<TOperationEvent,TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IQuery<TState, TOperationEvent, Tin>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TState, TOperationEvent, Tin, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents<TOperationEvent> stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, TOperationEvent, Tin>, Task<IQueryResult<TOperationEvent,TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }
    }


}
