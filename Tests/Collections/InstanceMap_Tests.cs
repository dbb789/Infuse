using System;
using NUnit.Framework;
using Infuse.Collections;

namespace Infuse.Collections.Tests
{
    public class InstanceMap_Tests
    {
        private class TestClassA { }
        private class TestClassB { }
        private class TestDisposable : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
        
        [Test]
        public void Empty()
        {
            var instanceMap = new InstanceMap();
        }

        [Test]
        public void SimpleAdd()
        {
            var instanceMap = new InstanceMap();
            var instanceA = new TestClassA();
            var instanceB = new TestClassB();

            instanceMap.Add(typeof(TestClassA), instanceA);
            instanceMap.Add(typeof(TestClassB), instanceB);

            Assert.IsTrue(instanceMap.Contains(typeof(TestClassA), instanceA));
            Assert.IsTrue(instanceMap.Contains(typeof(TestClassB), instanceB));
        }

        [Test]
        public void SimpleRemove()
        {
            var instanceMap = new InstanceMap();
            var instanceA = new TestClassA();
            var instanceB = new TestClassB();

            instanceMap.Add(typeof(TestClassA), instanceA);
            instanceMap.Add(typeof(TestClassB), instanceB);

            Assert.IsTrue(instanceMap.Contains(typeof(TestClassA), instanceA));
            Assert.IsTrue(instanceMap.Contains(typeof(TestClassB), instanceB));

            instanceMap.Remove(typeof(TestClassA), instanceA);
            instanceMap.Remove(typeof(TestClassB), instanceB);

            Assert.IsFalse(instanceMap.Contains(typeof(TestClassA), instanceA));
            Assert.IsFalse(instanceMap.Contains(typeof(TestClassB), instanceB));
        }

        [Test]
        public void InvokeDisposable()
        {
            var instanceMap = new InstanceMap();
            var instanceA = new TestClassA();
            var disposable = new TestDisposable();

            instanceMap.Add(typeof(TestClassA), instanceA, disposable);

            Assert.IsTrue(instanceMap.Contains(typeof(TestClassA), instanceA));
            Assert.IsFalse(disposable.Disposed);

            instanceMap.Remove(typeof(TestClassA), instanceA);

            Assert.IsFalse(instanceMap.Contains(typeof(TestClassA), instanceA));
            Assert.IsTrue(disposable.Disposed);
        }
    }
}
