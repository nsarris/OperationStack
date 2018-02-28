using DomainObjects.Operations;
using NUnit.Framework;
using System;
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
            var handleExceptionOs = new OperationStack<object, OperationEvent>()
                .Then(op =>
                {
                    throw new Exception();
                    return op.Return();
                })
                .OnErrors(h => 
                {
                    Assert.AreEqual(1, h.Errors.Count());
                    return h.Return();
                });
            Assert.Pass();
        }
    }
}
