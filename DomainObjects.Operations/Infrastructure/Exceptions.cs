using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public class OperationStackDeclarationException : Exception
    {
        public OperationStackDeclarationException(string message)
            :base(message)
        {

        }
    }

    public class OperationStackExecutionException : Exception
    {
        public OperationStackExecutionException(string message)
            : base(message)
        {

        }
    }
}
