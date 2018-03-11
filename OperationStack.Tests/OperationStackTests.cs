using DomainObjects.Operations;
using NUnit.Framework;
using System;
using System.Linq;

namespace OperationStackTests
{
    [TestFixture]
    public class OperationStackTests
    {
        [Test]
        public void RightResultTest()
        {
            var os =
                new OperationStackBuilder().Build()
                //new OperationStack<object, object, OperationEvent>()
                .Then(op =>
                {
                    //var x = 42;
                    return op.Return();
                })
                .ThenReturn(op =>
                {
                    Assert.AreEqual(0, op.Events.Count());
                    return op.Return(42);
                })
                .Execute();

            Assert.AreEqual(2, os.StackTrace.Count);
            Assert.AreEqual(42, os.Result.Value);
            Assert.AreEqual(os.Events.ToList().Count, 0);
            Assert.IsTrue(os.Success);
        }

        [Test]
        public void ThrowsExceptionTest()
        {
            var exceptionOs = new OperationStackBuilder().Build()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .OnErrors(h =>
                {
                    Assert.AreEqual(1, h.Errors.Count());
                })
                .Execute();

            Assert.Greater(exceptionOs.Events.ToList().Count, 0);
            Assert.False(exceptionOs.Success);
        }

        [Test]
        public void HandledExceptions()
        {
            var handledExceptionOs = new OperationStackBuilder()
                .WithOptions(new OperationStackOptions
                {
                    FailOnException = false
                })
                .WithState(() => (1,1))
                .WithInput<(int,string,string)>()
                .Build()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .OnExceptions(h =>
                {
                    Assert.AreEqual(1, h.ExceptionErrors.Count());
                    foreach (var e in h.ExceptionErrors)
                    {
                        e.Error.Handle();
                    }
                    return h.Return();
                })
                .Execute((2,"nikos","sarris"));

            Assert.True(handledExceptionOs.Success);
            Assert.True(handledExceptionOs.Events.All(e => e.IsHandled));
        }

        [Test]
        public void UnhandledExceptions()
        {
            var unhandledExceptionOs = new OperationStackBuilder().Build()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .ThenReturn(op =>
                {
                    return op.Return(42);
                })
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .Execute();

            Assert.False(unhandledExceptionOs.Success);
            Assert.False(unhandledExceptionOs.Events.All(e => e.IsHandled));
        }

        [Test]
        public void EventsErrorsExceptionsTest()
        {
            var os = new OperationStackBuilder().Build()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .OnErrors(h =>
                {
                    Assert.False(h.Errors.All(e => e.IsHandled));
                    foreach (var e in h.Errors)
                    {
                        e.Handle();
                    }
                })
                .OnExceptions(h =>
                {
                    Assert.True(h.ExceptionErrors.All(e => e.Error.IsHandled));
                    return h.Return();
                })
                .Then(op =>
                {
                    var oe = new OperationEvent("Test event");
                    //op.Throw(oe);
                })
                .OnEvents(h =>
                {
                    Assert.True(h.Events.All(e => e.IsHandled));
                    return h.Return();
                })
                .Then(op =>
                {
                    var oe = new OperationEvent(new Exception());
                    op.Throw(oe);
                })
                .OnEvents(h =>
                {
                    Assert.True(h.Events.All(e => e.IsHandled));
                    return h.Return();
                })
                .Then(op =>
                {
                    var oe = new OperationEvent("Test event");
                    oe.Throw(); // Should throw an exception because the OperationEvent is not an error
                    return op.Return();
                })
                .OnErrors(h =>
                {
                    Assert.False(h.Errors.All(e => e.IsHandled));
                    return h.Return();
                })
                .OnExceptions(h =>
                {
                    foreach (var e in h.ExceptionErrors)
                    {
                        e.Error.Handle();
                    }
                });

            var r=os.Execute();

            Assert.True(r.Events.All(e => e.IsHandled));
        }

        [Test]
        public void ResultFlowingThroughEventHandlers()
        {
            var os = new OperationStackBuilder().Build()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .ThenReturn(op =>
                {
                    return op.Return(42);
                })
                .Catch(h =>
                {
                    
                })
                .Execute();

            Assert.True(os.Success);
            Assert.True(os.Result.Value == 42);
            Assert.True(os.Events.All(e => e.IsHandled));
        }

        [Test]
        public void ResultChangedInEventHandler()
        {
            var os = new OperationStackBuilder().Build()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .ThenReturn(op =>
                {
                    return op.Return(42);
                })
                .CatchExceptionsOf<Exception>(h =>
                {
                    return h.Return(43);
                })
                .Execute();

            Assert.True(os.Success);
            Assert.True(os.Result.Value == 43);
            Assert.True(os.Events.All(e => e.IsHandled));
        }

        [Test]
        public void NestedOperatioStacks()
        {
            var os1 = new OperationStackBuilder().Build()
                .ThenReturn(op =>
                {
                    op.Events.Add(new OperationEvent("OS1"));
                    return op.Return(1);
                });

            var os2 = new OperationStackBuilder().Build()
                .ThenReturn(op =>
                {
                    op.Events.Add(new OperationEvent("OS2"));
                    return op.Return(2);
                });

            //var os3 = new OperationStack()
            //    .ThenAppend(os1)
            //    //.ThenAppend(op => os2.Execute(op.Input))
            //    .ThenAppend(os2)
            //    .Execute();

            //Assert.AreEqual(os3.Events.Count(), 2);
        }
    }
}
