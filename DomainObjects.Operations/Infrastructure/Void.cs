using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public interface IVoid
    {

    }

    public sealed class Void : IVoid
    {
        public static Void Value { get; } = new Void();
        private Void()
        {

        }
    }
}
