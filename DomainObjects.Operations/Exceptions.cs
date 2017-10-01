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
}
