using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public static class ExceptionErrorBuilder
    {
        private static Lazy<Func<Exception, OperationEvent>> builder;

        public static OperationEvent Build(Exception e)
        {
            return builder.Value(e);
        }

        static ExceptionErrorBuilder()
        {
            builder = new Lazy<Func<Exception, OperationEvent>>(() =>
            {
                var ctor = typeof(OperationEvent).GetConstructor(new Type[] { typeof(Exception) });

                if (ctor != null)
                {
                    var param1 = Expression.Parameter(typeof(Exception));
                    var l = Expression.Lambda<Func<Exception, OperationEvent>>(Expression.New(ctor, param1), param1);
                    return l.Compile();
                }
                else
                {
                    ctor = typeof(OperationEvent).GetConstructor(new Type[] { });
                    var l = Expression.Lambda<Func<OperationEvent>>(Expression.New(ctor));
                    var f = l.Compile();
                    OperationEvent r(Exception e)
                    {
                        var o = f();
                        o.FromException(e);
                        return o;
                    }
                    return r;
                }
            });
        }
    }
}
