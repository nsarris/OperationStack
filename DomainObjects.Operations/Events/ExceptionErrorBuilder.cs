using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Operations
{
    public static class ExceptionErrorBuilder<TOperationEvent>
        where TOperationEvent : OperationEvent
    {
        private static Lazy<Func<Exception, TOperationEvent>> builder;

        public static TOperationEvent Build(Exception e)
        {
            return builder.Value(e);
        }

        static ExceptionErrorBuilder()
        {
            builder = new Lazy<Func<Exception, TOperationEvent>>(() =>
            {
                var ctor = typeof(TOperationEvent).GetConstructor(new Type[] { typeof(Exception) });

                if (ctor != null)
                {
                    var param1 = Expression.Parameter(typeof(Exception));
                    var l = Expression.Lambda<Func<Exception, TOperationEvent>>(Expression.New(ctor, param1), param1);
                    return l.Compile();
                }
                else
                {
                    ctor = typeof(TOperationEvent).GetConstructor(new Type[] { });
                    var l = Expression.Lambda<Func<TOperationEvent>>(Expression.New(ctor));
                    var f = l.Compile();
                    TOperationEvent r(Exception e)
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
