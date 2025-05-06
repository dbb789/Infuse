using System;
using System.Collections.Generic;
using NUnit.Framework;
using Infuse.Collections;
using Infuse.TypeInfo;

namespace Infuse.TypeInfo.Tests
{
    public class OnDefuseFunc_Tests
    {
        private class TestClassA { }

        [Test]
        public void Null()
        {
            var onDefuseFunc = OnDefuseFunc.Null;
            
            Assert.IsTrue(onDefuseFunc.Empty);

            var instance = new TestClassA();

            onDefuseFunc.Invoke(instance);
        }

        [Test]
        public void Func()
        {
            var instance = new TestClassA();

            bool funcCalled = false;

            Action<object> func = (instanceArg) =>
            {
                Assert.IsFalse(funcCalled);
                Assert.AreEqual(instance, instanceArg);
                
                funcCalled = true;
            };

            var onDefuseFunc = new OnDefuseFunc(func);

            Assert.IsFalse(onDefuseFunc.Empty);

            onDefuseFunc.Invoke(instance);

            Assert.IsTrue(funcCalled);
        }
    }
}
