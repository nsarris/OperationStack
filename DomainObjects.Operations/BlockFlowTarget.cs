//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DomainObjects.Operations
//{
//    internal interface IOperationFlowTarget
//    {
//        int GetNext(List<IStackBlockSpec> blocks, IStackBlockSpec current);
//    }



//    internal class OperationFlowThis : IOperationFlowTarget
//    {
//        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
//        {
//            return operations[currentOperation];
//        }
//    }

//    internal class OperationFlowNext : IOperationFlowTarget
//    {
//        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
//        {
//            var nextIndex = currentOperation + 1;
//            if (nextIndex >= operations.Count)
//                return null;
//            else
//                return operations[nextIndex];
//        }
//    }

//    internal class OperationFlowTag : IOperationFlowTarget
//    {
//        public string Tag { get; private set; }
//        public OperationFlowTag(string tag)
//        {
//            this.Tag = tag;
//        }

//        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
//        {
//            var block = operations.Where(x => x.Name == Tag).FirstOrDefault();
//            if (block == null) throw new OperationFlowTagNotFoundException(Tag);
//            return block;
//        }
//    }

//    internal class OperationFlowSkip : IOperationFlowTarget
//    {
//        public int Skip { get; private set; }
//        public OperationFlowSkip(int skip)
//        {
//            this.Skip = skip;
//        }
//        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
//        {
//            var nextIndex = currentOperation + Skip;
//            if (nextIndex >= operations.Count)
//                return null;
//            else
//                return operations[nextIndex];
//        }
//    }

//    internal class OperationFlowRetry : IOperationFlowTarget
//    {
//        public int Times { get; private set; }
//        public IOperationFlowTarget Then { get; private set; }

//        public OperationFlowRetry(int times)
//            : this(times, null)
//        {

//        }
//        public OperationFlowRetry(int times, IOperationFlowTarget then)
//        {
//            Then = then ?? (then = new OperationFlowNext());
//            this.Times = times;
//        }

//        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
//        {
//            if (Times <= 0)
//                return Then.GetNext(operations, currentOperation);
//            else
//                return operations[currentOperation];
//        }
//    }

//    internal class OperationFlowEnd : IOperationFlowTarget
//    {
//        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
//        {
//            return operations.Where(x => x.Finally).FirstOrDefault();
//        }
//    }

//    internal class OperationFlowBreak : IOperationFlowTarget
//    {
//        internal override OperationStackBlock GetNext(List<OperationStackBlock> operations, int currentOperation)
//        {
//            return null;
//        }
//    }
//}
