using System;
using System.Collections.Generic;
using NUnit.Framework;
using Infuse.Collections;
using Infuse.TypeInfo;

namespace Infuse.TypeInfo.Tests
{
    public class OnInfuseFunc_Tests
    {
        private class TestClassA { }
        private class TestClassB { }
        private class TestClassC { }
        
        [Test]
        public void Null()
        {
            var onInfuseFunc = OnInfuseFunc.Null;
            
            Assert.IsTrue(onInfuseFunc.Empty);
            Assert.IsEmpty(onInfuseFunc.Dependencies);

            var instance = new TestClassA();
            var typeInfo = new InfuseTypeInfo(typeof(TestClassA),
                                              new Type[0],
                                              OnInfuseFunc.Null,
                                              OnDefuseFunc.Null);
            
            bool onInfuseCalled = false;
            
            onInfuseFunc.Invoke(instance, null, typeInfo, (typeInfoArg, instanceArg) =>
            {
                Assert.IsFalse(onInfuseCalled);
                Assert.AreEqual(typeInfo, typeInfoArg);
                Assert.AreEqual(instance, instanceArg);
                
                onInfuseCalled = true;
            });

            Assert.IsTrue(onInfuseCalled);
        }

        [Test]
        public void FuncAndDependencies()
        {
            var instance = new TestClassA();
            var dependencies = new Type [] { typeof(TestClassB), typeof(TestClassC) };
            var typeInfo = new InfuseTypeInfo(typeof(TestClassA),
                                              dependencies,
                                              OnInfuseFunc.Null,
                                              OnDefuseFunc.Null);
            
            var serviceMap = new ServiceMap();

            bool onInfuseCalled = false;

            Action<InfuseTypeInfo, object> onInfuseCompleted = (typeInfoArg, instanceArg) =>
            {
                Assert.IsFalse(onInfuseCalled);
                Assert.AreEqual(typeInfo, typeInfoArg);
                Assert.AreEqual(instance, instanceArg);

                onInfuseCalled = true;
            };
            
            bool funcCalled = false;
            
            OnInfuseFunc.InfuseFunc func = (instanceArg, serviceMapArg, typeInfoArg, onInfuseCompletedArg) =>
            {
                Assert.IsFalse(funcCalled);
                Assert.AreEqual(instance, instanceArg);
                Assert.AreEqual(serviceMap, serviceMapArg);
                Assert.AreEqual(typeInfo, typeInfoArg);
                Assert.AreEqual(onInfuseCompleted, onInfuseCompletedArg);

                onInfuseCompletedArg.Invoke(typeInfoArg, instanceArg);
                
                funcCalled = true;
            };

            var onInfuseFunc = new OnInfuseFunc(func, dependencies);
            
            Assert.IsFalse(onInfuseFunc.Empty);
            Assert.That(dependencies, Is.EquivalentTo(onInfuseFunc.Dependencies));
            
            onInfuseFunc.Invoke(instance, serviceMap, typeInfo, onInfuseCompleted);

            Assert.IsTrue(funcCalled);
            Assert.IsTrue(onInfuseCalled);
        }
    }
}
