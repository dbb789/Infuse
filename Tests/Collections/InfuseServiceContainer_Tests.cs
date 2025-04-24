using System;
using NUnit.Framework;
using Infuse.Collections;
using Infuse.Common;

namespace Infuse.Collections.Tests
{
    public class InfuseServiceContainer_Tests
    {
        private interface ITestInterfaceA { }
        private class TestClassA : ITestInterfaceA { }
        private class TestClassB { }
        
        private class TestServiceContainer<T> : InfuseServiceContainer<T> where T : class
        {
            public T Instance;
            public override bool Populated => _populated;
            
            private bool _populated;
            
            public override void Register(T instance)
            {
                Instance = instance;
                _populated = true;
            }
            
            public override void Unregister(T instance)
            {
                if (Instance != instance)
                {
                    throw new InfuseException($"Instance {instance} is not registered in the container.");
                }

                _populated = false;
                Instance = null;
            }
        }
    
        [Test]
        public void TestValidType()
        {
            var container = new TestServiceContainer<ITestInterfaceA>();
            
            Assert.IsFalse(container.Populated);
            
            var instance = new TestClassA();
            
            container.Register(instance); 
            Assert.IsTrue(container.Populated);
            Assert.AreEqual(instance, container.Instance);
            container.Unregister(instance);

            Assert.IsFalse(container.Populated);
        }

        [Test]
        public void TestRegisterInvalidType()
        {
            var container = new TestServiceContainer<ITestInterfaceA>();
            
            Assert.IsFalse(container.Populated);
            
            var instance = new TestClassB();
            
            Assert.Throws<InfuseException>(() => container.Register(instance));
            Assert.IsNull(container.Instance);
            Assert.IsFalse(container.Populated);
        }

        [Test]
        public void TestUnregisterInvalidType()
        {
            var container = new TestServiceContainer<ITestInterfaceA>();

            var instance = new TestClassB();
            
            Assert.Throws<InfuseException>(() => container.Unregister(instance));
            Assert.IsFalse(container.Populated);
        }
    }
}
