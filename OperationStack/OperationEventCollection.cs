using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{

    public interface IStackEventLog : IEnumerable<OperationEvent>
    {
        IEnumerable<OperationEvent> Errors { get; }
    }

    public interface IOperationEventLog : IEnumerable<OperationEvent>
    {
        void Add(OperationEvent operationEvent);
        void AddException(string tag, Exception e, bool handled = false);
        IEnumerable<OperationEvent> Errors { get; }
    }


    public class OperationEventCollection : List<OperationEvent>, IOperationEventLog, IStackEventLog
    {
        public OperationEventCollection()
            :base()
        {

        }

        public OperationEventCollection(IEnumerable<OperationEvent> events)
            :base(events)
        {

        }
        public void AddException(string tag, Exception e, bool handled = false)
        {
            this.Add(new OperationError(tag, e, handled));
        }

        public IEnumerable<OperationEvent> Errors => this.OfType<OperationError>();
    }
}
