using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    
    public abstract class StackBlockBase<TState> : IStackBlock<TState>
    {
        public string Tag { get; private set; }
        public TState StackState { get; private set; }
        public IStackEvents StackEvents { get; set; }
        public IOperationEvents Events { get; private set; } = new OperationEvents();
        public bool IsEmptyEventBlock { get; protected set; }
        public bool IsAsync => executorAsync != null;
        public IEnumerable<BlockTraceResult> InnerStackTrace { get; private set; } = new List<BlockTraceResult>();
        public StackBlockBase(string tag, TState state, IStackEvents stackEvents)
        {
            Tag = tag;
            StackState = state;
            StackEvents = stackEvents;
        }
        
        protected Func<IBlockResult> executor;
        protected Func<Task<IBlockResult>> executorAsync;

        public void Append(IOperationResult result)
        {
            this.Events.Append(result.Events);
            ((List<BlockTraceResult>)this.InnerStackTrace).AddRange(result.StackTrace);
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
                this.Events.Add(OperationError.FromException(e, true));
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
                this.Events.Add(OperationError.FromException(e, true));
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

    public class Command<TState> : StackBlockBase<TState>, ICommand<TState>
    {
        private ResultVoidDispatcher resultDispacther = new ResultVoidDispatcher();
        
        protected Command(string tag, TState state, IStackEvents stackEvents)
            : base(tag, state, stackEvents)
        {

        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Func<ICommand<TState>, BlockResultVoid> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }
        internal Command(string tag, TState state, IStackEvents stackEvents, Action<ICommand<TState>> action)
            : base(tag, state, stackEvents)
        {
            executor = () => { action(this); return resultDispacther.Return(); };
        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Func<IOperationBlock<TState>, ICommandResult> func)
            : base(tag, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispacther.Return(); };
        }


        internal Command(string tag, TState state, IStackEvents stackEvents, Func<ICommand<TState>, Task<BlockResultVoid>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this);
        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Func<ICommand<TState>, Task> actionAsync)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { await actionAsync(this); return resultDispacther.Return(); };
        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Func<IOperationBlock<TState>, Task<ICommandResult>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this); this.Append(r); return resultDispacther.Return(); };
        }

        BlockResultVoid IResultVoidDispatcher.Break()
        {
            return resultDispacther.Break();
        }

        BlockResultVoid IResultVoidDispatcher.End()
        {
            return resultDispacther.End();
        }

        BlockResultVoid IResultVoidDispatcher.End(object overrideResult)
        {
            return resultDispacther.End(overrideResult);
        }

        BlockResultVoid IResultVoidDispatcher.Goto(string tag)
        {
            return resultDispacther.Goto(tag);
        }

        BlockResultVoid IResultVoidDispatcher.Goto(string tag, object overrideInput)
        {
            return resultDispacther.Goto(tag, overrideInput);
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

        BlockResultVoid IResultVoidDispatcher.Return()
        {
            return resultDispacther.Return();
        }

        BlockResultVoid IResultVoidDispatcher.Skip(int i)
        {
            return resultDispacther.Skip(i);
        }

        BlockResultVoid IResultVoidDispatcher.Skip(int i, object overrideInput)
        {
            return resultDispacther.Skip(i, overrideInput);
        }
    }

    public class Command<TState, Tin> : Command<TState>, ICommand<TState, Tin>
    {
        public Emptyable<Tin> Input { get; private set; }

        internal Command(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ICommand<TState, Tin>, BlockResultVoid> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Action<ICommand<TState, Tin>> action)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { action(this); return ((IResultVoidDispatcher)this).Return(); };
        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState,Tin>, ICommandResult> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultVoidDispatcher)this).Return(); };
        }


        internal Command(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ICommand<TState, Tin>, Task<BlockResultVoid>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this); 
        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ICommand<TState, Tin>, Task> action)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { await action(this); return ((IResultVoidDispatcher)this).Return(); };
        }

        internal Command(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, Tin>, Task<ICommandResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this); this.Append(r); return ((IResultVoidDispatcher)this).Return(); };
        }


    }

    public class Query<TState, TResult> : StackBlockBase<TState>, IQuery<TState>, ITypedQuery<TState,TResult>
    {
        private ResultDispatcher<TResult> resultDispatcher = new ResultDispatcher<TResult>();
                protected Query(string tag, TState state, IStackEvents stackEvents)
            : base(tag, state, stackEvents)
        {
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Func<IQuery<TState>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Func<ITypedQuery<TState, TResult>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Func<IOperationBlock<TState>, IQueryResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            executor = () => { var r = func(this); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Func<IQuery<TState>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Func<ITypedQuery<TState, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Func<IOperationBlock<TState>, Task<IQueryResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            executorAsync = async () => { var r = await func(this); this.Append(r); return resultDispatcher.Return(r.Result.Value); };
        }




        IQueryResultProxy<T> IQuery<TState>.DefineResult<T>()
        {
            return new QueryResultProxy<T>();
        }

        IQueryResultProxy<T> IQuery<TState>.DefineResult<T>(T result)
        {
            return new QueryResultProxy<T>() { Result = result };
        }

        IQueryResultProxy<T> IQuery<TState>.DefineResult<T>(Expression<Func<T>> expression)
        {
            return new QueryResultProxy<T>();
        }



        BlockResult<T> IResultDispatcher.Break<T>()
        {
            return new ResultDispatcher<T>().Break();
        }

        BlockResult<T> IResultDispatcher.End<T>()
        {
            return new ResultDispatcher<T>().End();
        }

        BlockResult<T> IResultDispatcher.End<T>(object overrideResult)
        {
            return new ResultDispatcher<T>().End(overrideResult);
        }

        BlockResult<T> IResultDispatcher.Goto<T>(string tag)
        {
            return new ResultDispatcher<T>().Goto(tag);
        }

        BlockResult<T> IResultDispatcher.Goto<T>(string tag, object overrideInput)
        {
            return new ResultDispatcher<T>().Goto(tag, overrideInput);
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

        BlockResult<T> IResultDispatcher.Return<T>(T result)
        {
            return new ResultDispatcher<T>().Return(result);
        }

        BlockResult<T> IResultDispatcher.Skip<T>(int i)
        {
            return new ResultDispatcher<T>().Skip(i);
        }

        BlockResult<T> IResultDispatcher.Skip<T>(int i, object overrideInput)
        {
            return new ResultDispatcher<T>().Skip(i, overrideInput);
        }

        

        BlockResult<TResult> IResultDispatcher<TResult>.Return(TResult result)
        {
            return resultDispatcher.Return(result);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.End()
        {
            return resultDispatcher.End();
        }

        BlockResult<TResult> IResultDispatcher<TResult>.End(object overrideResult)
        {
            return resultDispatcher.End(overrideResult);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Break()
        {
            return resultDispatcher.Break();
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

        BlockResult<TResult> IResultDispatcher<TResult>.Goto(string tag)
        {
            return resultDispatcher.Goto(tag);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Goto(string tag, object overrideInput)
        {
            return resultDispatcher.Goto(tag, overrideInput);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Skip(int i)
        {
            return resultDispatcher.Skip(i);
        }

        BlockResult<TResult> IResultDispatcher<TResult>.Skip(int i, object overrideInput)
        {
            return resultDispatcher.Skip(i, overrideInput);
        }
    }

    public class Query<TState, Tin, TResult> : Query<TState, TResult>, IQuery<TState, Tin>, ITypedQuery<TState,Tin,TResult>
    {
        public Emptyable<Tin> Input { get; private set; }
        
        internal Query(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IQuery<TState,Tin>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TState,Tin,TResult>, BlockResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, Tin>, IQueryResult<TResult>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executor = () => { var r = func(this); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IQuery<TState, Tin>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<ITypedQuery<TState, Tin, TResult>, Task<BlockResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => await func(this);
        }

        internal Query(string tag, TState state, IStackEvents stackEvents, Emptyable<Tin> input, Func<IOperationBlock<TState, Tin>, Task<IQueryResult<TResult>>> func)
            : base(tag, state, stackEvents)
        {
            Input = input;
            executorAsync = async () => { var r = await func(this); this.Append(r); return ((IResultDispatcher<TResult>)this).Return(r.Result.Value); };
        }
    }


}
