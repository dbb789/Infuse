using Infuse;
using NUnit.Framework;

namespace Infuse.Tests
{
    public class InfuseInstanceMap_Tests
    {
        private class TestClassA { }
        private class TestClassB { }

        [Test]
        public void Empty()
        {
            var instanceMap = new InfuseInstanceMap();
        }

        [Test]
        public void SimpleAdd()
        {
            var instanceMap = new InfuseInstanceMap();
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
            var instanceMap = new InfuseInstanceMap();
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
    }
}
