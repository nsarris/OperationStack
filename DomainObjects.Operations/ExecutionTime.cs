using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    [System.Diagnostics.DebuggerDisplay("{ElapsedMilliseconds}ms")]
    public class ExecutionTime
    {
        public bool Timed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Elapsed { get; set; }
        public long ElapsedMilliseconds => Convert.ToInt64(Elapsed.TotalMilliseconds);

        public ExecutionTime()
        {

        }
        public ExecutionTime(DateTime start, DateTime end)
        {
            Set(start, end);
        }

        public ExecutionTime(DateTime start, DateTime end, TimeSpan elapsed)
        {
            Set(start, end, elapsed);
        }

        public ExecutionTime(DateTime end, TimeSpan elapsed)
        {
            Set(end, elapsed);
        }

        internal void Set(DateTime start, DateTime end)
        {
            StartTime = start;
            EndTime = end;
            Elapsed = start - end;
            Timed = true;
        }

        internal void Set(DateTime start, DateTime end, TimeSpan elapsed)
        {
            StartTime = start;
            EndTime = end;
            Elapsed = elapsed;
            Timed = true;

        }

        internal void Set(DateTime end, TimeSpan elapsed)
        {
            StartTime = end - elapsed;
            EndTime = end;
            Elapsed = elapsed;
            Timed = true;

        }
    }
}
