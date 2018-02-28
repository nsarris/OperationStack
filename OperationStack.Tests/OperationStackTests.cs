using DomainObjects.Operations;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace OperationStack.Tests
{
    [TestFixture]
    public class OperationStackTests
    {
        [Test]
        public void RightResultTest()
        {
            var os = new OperationStack<object, OperationEvent>()
                .Then(op =>
                {
                    var x = 42;
                    return op.Return();
                })
                .ThenReturn(op =>
                {
                    Assert.AreEqual(0, op.Events.Count());
                    return op.Return(42);
                })
                .ToResult();

            Assert.AreEqual(2, os.StackTrace.Count);
            Assert.AreEqual(42, os.Result.Value);
            Assert.AreEqual(os.Events.ToList().Count, 0);
            Assert.IsTrue(os.Success);
        }

        [Test]
        public void ThrowsExceptionTest()
        {
            var exceptionOs = new OperationStack<object, OperationEvent>()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .OnErrors(h =>
                {
                    Assert.AreEqual(1, h.Errors.Count());
                })
                .ToResult();

            Assert.Greater(exceptionOs.Events.ToList().Count, 0);
            Assert.False(exceptionOs.Success);
        }

        [Test]
        public void HandleExceptions()
        {
            var handledExceptionOs = new OperationStack<object, OperationEvent>(new OperationStackOptions()
            {
                FailOnException = false
            })
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .OnUnhandledExceptions(h =>
                {
                    Assert.AreEqual(1, h.ExceptionErrors.Count());
                    foreach (var e in h.ExceptionErrors)
                    {
                        e.Error.Handle();
                    }
                    return h.Return();
                })
                .ToResult();

            Assert.True(handledExceptionOs.Success);
            Assert.True(handledExceptionOs.Events.All(e => e.IsHandled));
        }

        [Test]
        public void UnhandledExceptions()
        {
            var unhandledExceptionOs = new OperationStack<object, OperationEvent>()
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
                .ToResult();

            Assert.False(unhandledExceptionOs.Success);
            Assert.False(unhandledExceptionOs.Events.All(e => e.IsHandled));
        }

        [Test]
        public void EventsErrorsExceptionsTest()
        {
            var os = new OperationStack<object, OperationEvent>()
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
                .OnUnhandledExceptions(h =>
                {
                    Assert.True(h.ExceptionErrors.All(e => e.Error.IsHandled));
                    return h.Return();
                })
                .Then(op =>
                {
                    var oe = new OperationEvent("Test event");
                })
                .OnEvents(h =>
                {
                    Assert.True(h.Events.All(e => e.IsHandled));
                    return h.Return();
                })
                .Then(op =>
                {
                    var oe = new OperationEvent(new Exception());
                })
                .OnEvents(h =>
                {
                    Assert.True(h.Events.All(e => e.IsHandled));
                    return h.Return();
                })
                .Then(op =>
                {
                    var oe = new OperationEvent("Test event");
                    oe.Throw();
                    return op.Return();
                })
                .OnErrors(h =>
                {
                    Assert.False(h.Errors.All(e => e.IsHandled));
                    return h.Return();
                })
                .OnUnhandledExceptions(h =>
                {
                    foreach (var e in h.ExceptionErrors)
                    {
                        e.Error.Handle();
                    }
                })
                .ToResult();

            Assert.True(os.Events.All(e => e.IsHandled));
        }
    }
}
