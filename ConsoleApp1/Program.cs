using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.Operations;
using System.Threading;
using System.Globalization;

namespace ConsoleApp1
{


    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            var i = -1;

            //var s = new OperationStack(
            //    "Start",(op) =>
            //    {
            //        //i = 3;
            //        //flow.End("2");
            //        throw new Exception("test");
            //        //return op.Return();
            //    })
            //    .Then("Stub", op =>
            //    {
            //        i = 3;
            //    })
            //    .Then<string>("Get id2", (op) =>
            //    {
            //        if (i != 1)
            //        {
            //            i--;
            //            //return flow.Return(2);
            //            return op.Retry();
            //        }
            //        else
            //            return op.Return("2");
            //    })
            //    .Then<int>("Get id",(op) =>
            //    {
            //        return op.Return(2);
            //        //return op.End(2);
            //    })
            //    //.ThenAppend("Check message",(resp, ss) =>
            //    //{
            //    //    resp.EventLog.Add(new OperationError() { Message = "Test" });
            //    //    var newResp = new QueryResult<int>(resp);
            //    //    newResp.Result = 1;
            //    //    return newResp;
            //    //})
            //    .Then<int>((op) =>
            //    {
            //        if (i < 3)
            //        {
            //            return op.Retry(i++);
            //        }

            //        return op.Return(i++);
            //        //return op.Break();
            //    })
            //    .Finally<int>((op) =>
            //    {
            //        return op.Return(1);
            //    });


            var stack = new OperationStack(
                new OperationStackOptions()
                {
                    EndOnException = true,
                    TimeMeasurement = false,
                }
                )
                .Then(async op =>
                {
                    await Task.Delay(20);
                    throw new Exception("Test");
                })
                .Then(op =>
                {
                    throw new ArgumentException("Test");
                    return op.Return();
                })
                .Then("T1", op =>
                 {
                     if (i < 2)
                     {
                         i++;
                         return op.Goto("T1");
                     }
                     return op.Return();
                 })
                 .ThenReturn(op =>
                 {
                     return op.Return(4);
                 })
                 .ThenReturn(async op =>
                 {
                     await Task.Delay(20);
                     return op.Return(6);
                 })
                 .OnUnhandledExceptionsOf<ArgumentException>(handler =>
                 {
                     return handler.Return();
                 })
                 .OnUnhandledExceptions(handler =>
                 {
                     return handler.Return();
                 })
                 .FinallyReturn(op =>
                 {
                     return op.Return("Finally");
                 })
                 ;





            //var r = stack.ToResult();
            var rasync = stack.ToResultAsync().Result;

            var ss = new OperationStack()
                .ThenAppend(op =>
                {
                    return stack.ToResultAsync().Result;
                })
                .ThenAppend(stack.ToResultAsync().Result)
                .Then(op =>
                {
                    var r = stack.ToResultAsync().Result;
                    op.Append(r);
                })
                .ThenReturn<int>(op =>
                {
                    return op.Return(2);
                })
                ;
                
                var rs = ss.ToResult();

        }


    }
}
