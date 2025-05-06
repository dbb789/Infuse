using System;
using System.Collections.Generic;
using NUnit.Framework;
using Infuse.Collections;
using Infuse.Common;

namespace Infuse.Collections.Tests
{
    public class ServiceMap_Tests
    {
        private class TestClassA { }
        private class TestClassB { }
        private class TestServiceContainer<T> : ServiceContainer<T> where T : class
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
        
        private class ThrowServiceContainer<T> : ServiceContainer<T> where T : class
        {
            public override bool Populated => false;

            private bool _populated;
            
            public override void Register(T instance)
            {
                throw new Exception($"Test Exception");
            }
            
            public override void Unregister(T instance)
            {
                throw new Exception($"Test Exception");
            }
            
        }

        private class EternalServiceContainer<T> : ServiceContainer<T> where T : class
        {
            public override bool Populated => true;

            private bool _populated;
            
            public override void Register(T instance)
            {
            }
            
            public override void Unregister(T instance)
            {
            }
        }

        [Test]
        public void Empty()
        {
            var serviceMap = new ServiceMap();

            Assert.IsFalse(serviceMap.Contains(typeof(TestClassA)));
            Assert.Throws<InfuseException>(() => serviceMap.GetService(typeof(TestClassA)));
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet()));
            Assert.IsFalse(serviceMap.ContainsAll(new TypeSet(new Type [] { typeof(TestClassA) })));
        }

        [Test]
        public void RegisterUnregister()
        {
            var serviceMap = new ServiceMap();
            var instanceA = new TestClassA();
            var instanceB = new TestClassB();

            serviceMap.Register(typeof(TestClassA), instanceA);

            Assert.IsTrue(serviceMap.Contains(typeof(TestClassA)));
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet(new Type [] { typeof(TestClassA) })));
            Assert.AreEqual(serviceMap.GetService(typeof(TestClassA)), instanceA);
            
            serviceMap.Register(typeof(TestClassB), instanceB);
            
            Assert.IsTrue(serviceMap.Contains(typeof(TestClassA)));
            Assert.AreEqual(serviceMap.GetService(typeof(TestClassA)), instanceA);
            Assert.IsTrue(serviceMap.Contains(typeof(TestClassB)));
            Assert.AreEqual(serviceMap.GetService(typeof(TestClassB)), instanceB);
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet(new Type [] { typeof(TestClassA), typeof(TestClassB) })));

            serviceMap.Unregister(typeof(TestClassA), instanceA);

            Assert.IsFalse(serviceMap.Contains(typeof(TestClassA)));
            Assert.Throws<InfuseException>(() => serviceMap.GetService(typeof(TestClassA)));
            Assert.IsTrue(serviceMap.Contains(typeof(TestClassB)));
            Assert.AreEqual(serviceMap.GetService(typeof(TestClassB)), instanceB);
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet(new Type [] { typeof(TestClassB) })));
        }

        [Test]
        public void InvalidAssignment()
        {
            var serviceMap = new ServiceMap();
            var instanceA = new TestClassA();
            var instanceB = new TestClassB();

            Assert.Throws<InfuseException>(() => serviceMap.Register(typeof(TestClassA), instanceB));
            Assert.Throws<InfuseException>(() => serviceMap.Register(typeof(TestClassB), instanceA));

            Assert.Throws<InfuseException>(() => serviceMap.Unregister(typeof(TestClassA), instanceB));
            Assert.Throws<InfuseException>(() => serviceMap.Unregister(typeof(TestClassB), instanceA));
        }

        [Test]
        public void UnregisterNonExistent()
        {
            var serviceMap = new ServiceMap();
            var instanceA = new TestClassA();
            var instanceB = new TestClassB();

            serviceMap.Register(typeof(TestClassA), instanceA);

            Assert.Throws<InfuseException>(() => serviceMap.Unregister(typeof(TestClassB), instanceA));
            Assert.Throws<InfuseException>(() => serviceMap.Unregister(typeof(TestClassB), instanceB));

            serviceMap.Unregister(typeof(TestClassA), instanceA);
        }

        [Test]
        public void RegisterUnregisterServiceContainer()
        {
            var serviceMap = new ServiceMap();
            var instanceA = new TestClassA();
            var instanceB = new TestClassB();

            serviceMap.Register(typeof(TestServiceContainer<TestClassA>), instanceA);

            Assert.IsTrue(serviceMap.Contains(typeof(TestServiceContainer<TestClassA>)));
            Assert.IsFalse(serviceMap.Contains(typeof(TestServiceContainer<TestClassB>)));
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet(new Type [] {
                            typeof(TestServiceContainer<TestClassA>) })));

            Assert.AreEqual(((TestServiceContainer<TestClassA>)
                             serviceMap.GetService(typeof(TestServiceContainer<TestClassA>))).Instance, instanceA);
            Assert.Throws<InfuseException>(() => serviceMap.GetService(typeof(TestServiceContainer<TestClassB>)));
            
            serviceMap.Register(typeof(TestServiceContainer<TestClassB>), instanceB);

            Assert.IsTrue(serviceMap.Contains(typeof(TestServiceContainer<TestClassA>)));
            Assert.IsTrue(serviceMap.Contains(typeof(TestServiceContainer<TestClassB>)));
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet(new Type [] {
                        typeof(TestServiceContainer<TestClassA>),
                        typeof(TestServiceContainer<TestClassB>) })));
            
            Assert.AreEqual(((TestServiceContainer<TestClassA>)
                             serviceMap.GetService(typeof(TestServiceContainer<TestClassA>))).Instance, instanceA);
            Assert.AreEqual(((TestServiceContainer<TestClassB>)
                             serviceMap.GetService(typeof(TestServiceContainer<TestClassB>))).Instance, instanceB);

            serviceMap.Unregister(typeof(TestServiceContainer<TestClassA>), instanceA);

            Assert.IsFalse(serviceMap.Contains(typeof(TestServiceContainer<TestClassA>)));
            Assert.IsTrue(serviceMap.Contains(typeof(TestServiceContainer<TestClassB>)));
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet(new Type [] {
                            typeof(TestServiceContainer<TestClassB>) })));
            
            Assert.Throws<InfuseException>(() => serviceMap.GetService(typeof(TestServiceContainer<TestClassA>)));
            Assert.AreEqual(((TestServiceContainer<TestClassB>)
                             serviceMap.GetService(typeof(TestServiceContainer<TestClassB>))).Instance, instanceB);
            
            serviceMap.Unregister(typeof(TestServiceContainer<TestClassB>), instanceB);
            
            Assert.IsFalse(serviceMap.Contains(typeof(TestServiceContainer<TestClassA>)));
            Assert.IsFalse(serviceMap.Contains(typeof(TestServiceContainer<TestClassB>)));
            Assert.IsTrue(serviceMap.ContainsAll(new TypeSet()));

            Assert.Throws<InfuseException>(() => serviceMap.GetService(typeof(TestServiceContainer<TestClassA>)));
            Assert.Throws<InfuseException>(() => serviceMap.GetService(typeof(TestServiceContainer<TestClassB>)));
        }

        [Test]
        public void ServiceContainerMultiRegister()
        {
            var serviceMap = new ServiceMap();
            var instance0 = new TestClassA();
            var instance1 = new TestClassA();

            serviceMap.Register(typeof(TestServiceContainer<TestClassA>), instance0);
            
            Assert.AreEqual(((TestServiceContainer<TestClassA>)
                             serviceMap.GetService(typeof(TestServiceContainer<TestClassA>))).Instance, instance0);

            serviceMap.Register(typeof(TestServiceContainer<TestClassA>), instance1);

            Assert.AreEqual(((TestServiceContainer<TestClassA>)
                             serviceMap.GetService(typeof(TestServiceContainer<TestClassA>))).Instance, instance1);

            serviceMap.Unregister(typeof(TestServiceContainer<TestClassA>), instance1);

            Assert.Throws<InfuseException>(() => serviceMap.GetService(typeof(TestServiceContainer<TestClassA>)));
        }

        [Test]
        public void ServiceContainerRegisterThrow()
        {
            var serviceMap = new ServiceMap();
            var instance = new TestClassA();
            
            Assert.Throws<Exception>(() => serviceMap.Register(typeof(ThrowServiceContainer<TestClassA>), instance));
            Assert.IsFalse(serviceMap.Contains(typeof(ThrowServiceContainer<TestClassA>)));
        }

        [Test]
        public void ServiceContainerKeepActiveWhilePopulated()
        {
            var serviceMap = new ServiceMap();
            var instance = new TestClassA();

            serviceMap.Register(typeof(EternalServiceContainer<TestClassA>), instance);
            Assert.IsTrue(serviceMap.Contains(typeof(EternalServiceContainer<TestClassA>)));

            serviceMap.Unregister(typeof(EternalServiceContainer<TestClassA>), instance);
            Assert.IsTrue(serviceMap.Contains(typeof(EternalServiceContainer<TestClassA>)));
        }
    }
}
