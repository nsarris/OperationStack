using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    //[DebuggerNonUserCode]

    internal class OperationStackBlock
    {
        public IOperation Operation { get; private set; }
        public bool Finally { get; private set; }
        public string Name { get; internal set; }
        public int Index { get; private set; }
        public OperationStackBlock(IOperation operation, int index, string name = null, bool isFinally = false)
        {
            this.Operation = operation;
            this.Index = index;
            this.Name = name;
            this.Finally = isFinally;
        }
        
    }

    

    public class OperationStack 
    {
        
        private List<OperationStackBlock> operations = new List<OperationStackBlock>();
        
        internal OperationStack(OperationStack operationStack)
        {
            this.operations = operationStack.operations;
            
        }

        
        public OperationStack(Func<CommandOperation, OperationFlowResultVoid> command)
        {
            Then(command);
        }

        public OperationStack(Action<CommandOperation> command)
        {
            Then(command);
        }

        public OperationStack(string name, Func<CommandOperation, OperationFlowResultVoid> command)
        {
            Then(name, command);
        }

        public OperationStack(string name, Action<CommandOperation> command)
        {
            Then(command);
        }



        protected void AddOperation(IOperation operation, string name = null, bool isFinally = false)
        {
            if (!string.IsNullOrEmpty(name) && operations.Any(x => x.Name == name))
                throw new Exception("Duplicate block name '" +  name +"' in stack.");

            if (!operations.Any() && isFinally)
                throw new Exception("A finally block cannot be the first block of a stack.");

            if (operations.Any() && operations.Last().Finally)
                throw new Exception("No blocks can be added after a finally block");

            

            var part = new OperationStackBlock(operation, operations.Count,isFinally: isFinally);
            operations.Add(part);
            part.Name = string.IsNullOrEmpty(name) ? "Part" + part.Index : name;
        }

        protected OperationStack ThenAndReturnNew(ICommand operation, string name = null, bool isFinally = false)
        {
            AddOperation(operation, name, isFinally);
            return new OperationStack(this);
        }

        protected OperationStack<T> ThenAndReturnNew<T>(IQuery<T> query, string name = null, bool isFinally = false, bool isAppend = false)
        {
            //if (!isAppend && typeof(IOperationResult).IsAssignableFrom(typeof(T)))
            //    throw new Exception("Cannot return IOperationResult from block, if you want to append a result use ThenAppend().");

            AddOperation(query, name, isFinally);
            return new OperationStack<T>(this);
        }


        #region Then Commands

        public OperationStack Then(Func<CommandOperation, OperationFlowResultVoid> command)
        {
            return ThenAndReturnNew(new Command(command));
        }

        public OperationStack Then(Action<CommandOperation> command)
        {
            return ThenAndReturnNew(new Command(command));
        }

        public OperationStack Then(string name,Func<CommandOperation, OperationFlowResultVoid> command)
        {
            return ThenAndReturnNew(new Command(command),name);
        }

        public OperationStack Then(string name, Action<CommandOperation> command)
        {
            return ThenAndReturnNew(new Command(command),name);
        }

        #endregion Then Commands

        #region Then Queries

        public OperationStack<T> Then<T>(Func<QueryOperation<T>, OperationFlowResult<T>> query)
        {
            return ThenAndReturnNew(new Query<T>(query));
        }

        public OperationStack<T> Then<T>(string name, Func<QueryOperation<T>, OperationFlowResult<T>> query)
        {
            return ThenAndReturnNew(new Query<T>(query),name);
        }

        //public OperationStack<T> ThenAppend<T>(Func<IOperationResult<T>> query)
        //{
        //    //return ThenAndReturnNew(new Query<T>(query), isAppend: true);
        //    throw new NotImplementedException();
        //}

        #endregion Then Queries

        #region Finally

        public OperationStack Finally(Func<CommandOperation, OperationFlowResultVoid> command)
        {
            return ThenAndReturnNew(new Command(command),isFinally: true);
        }

        public OperationStack Finally(Action<CommandOperation> command)
        {
            return ThenAndReturnNew(new Command(command), isFinally: true);
        }

        public OperationStack<T> Finally<T>(Func<QueryOperation<T>, OperationFlowResult<T>> query)
        {
            return ThenAndReturnNew(new Query<T>(query), isFinally: true);
        }


        #endregion Finally

        protected static readonly object Undefined = new object();

        public OperationStackResult ToResult()
        {
            return (OperationStackResult)GetResult<object>();
        }

        protected OperationStackResult<T> GetResult<T>()
        {
            IEmptyable data = new Emptyable<object>();
            Emptyable<T> result = new Emptyable<T>();
            var stackEventLog = new OperationEventCollection();
            
            var stackTrace = new List<OperationResult>();
            var stackStart = DateTime.Now;

            var o = operations.FirstOrDefault();
            var last = operations.LastOrDefault();
            var sw = new Stopwatch();
            var stackSw = new Stopwatch();
            stackSw.Start();
            while (o != null)
            {
                var isLast = o == last;
                Operation op = o.Operation.CreateOperation(o.Name, data, stackEventLog);
                OperationResult opResult;
                IOperationFlowResult flowResult = null;
                DateTime start = DateTime.Now;

                sw.Reset();
                sw.Start();
                try
                {
                    flowResult = op.Execute();
                    opResult = new OperationResult(op, data, flowResult.Result);
                    stackEventLog.AddRange(opResult.EventLog);
                    data = flowResult.Result;
                    var fresult = ((OperationFlowResultBase)flowResult);
                    
                    if (fresult.RedirectResult.HasValue)
                        data = fresult.RedirectResult;

                    o = fresult.Target.GetNext(operations, o.Index);
                }
                catch(Exception e)
                {
                    op.EventLog.AddException(op.Name, e);
                    data = new Emptyable<object>();
                    opResult = new OperationResult(op, data);
                    stackEventLog.AddRange(op.EventLog);
                    //use break on exception flag to stop or go to unhandled and finally
                    o = new OperationFlowNext().GetNext(operations, o.Index);
                }
                sw.Stop();

                opResult.Timer.Set(start, DateTime.Now, sw.Elapsed);
                stackTrace.Add(opResult);
                //add all results to inner collection
                
                if (o == null && isLast && data is Emptyable<T>)
                    result = (Emptyable<T>)data;
            }
            stackSw.Stop();

            var r = new OperationStackResult<T>(stackTrace, result);
            r.Timer.Set(stackStart, DateTime.Now, stackSw.Elapsed);
            
            return r;
        }

    }

    public class OperationStack<T> : OperationStack
    {
        internal OperationStack(OperationStack operationStack)
            :base(operationStack)
        {

        }

        #region Then Commands

        public OperationStack Then(Func<CommandOperation<T>, OperationFlowResultVoid> command)
        {
            return ThenAndReturnNew(new Command<T>(command));
        }

        public OperationStack Then(Action<CommandOperation<T>> command)
        {
            return ThenAndReturnNew(new Command<T>(command));
        }

        public OperationStack Then(string name, Func<CommandOperation<T>, OperationFlowResultVoid> command)
        {
            return ThenAndReturnNew(new Command<T>(command), name);
        }

        public OperationStack Then(string name, Action<CommandOperation<T>> command)
        {
            return ThenAndReturnNew(new Command<T>(command), name);
        }

        #endregion Then Commands

        #region Then Queries

        public OperationStack<Tout> Then<Tout>(Func<QueryOperation<T,Tout>, OperationFlowResult<Tout>> query)
        {
            return ThenAndReturnNew(new Query<T,Tout>(query));
        }

        public OperationStack<Tout> Then<Tout>(string name, Func<QueryOperation<T, Tout>, OperationFlowResult<Tout>> query)
        {
            return ThenAndReturnNew(new Query<T, Tout>(query),name);
        }

        //public OperationStack<T> ThenAppend<T>(Func<IOperationResult<T>> query)
        //{
        //    //return ThenAndReturnNew(new Query<T>(query), isAppend: true);
        //    throw new NotImplementedException();
        //}

        #endregion Then Queries

        #region Finally

        public OperationStack Finally(Func<CommandOperation<T>, OperationFlowResultVoid> command)
        {
            return ThenAndReturnNew(new Command<T>(command), isFinally: true);
        }

        public OperationStack Finally(Action<CommandOperation<T>> command)
        {
            return ThenAndReturnNew(new Command<T>(command), isFinally: true);
        }

        public OperationStack<Tout> Finally<Tout>(Func<QueryOperation<T,Tout>, OperationFlowResult<Tout>> query)
        {
            return ThenAndReturnNew(new Query<T,Tout>(query), isFinally: true);
        }


        #endregion Finally

        public new OperationStackResult<T> ToResult()
        {
            return GetResult<T>();
        }
    }
    

}
