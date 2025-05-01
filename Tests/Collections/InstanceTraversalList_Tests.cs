using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Infuse;
using Infuse.Collections;
using Infuse.TypeInfo;

namespace Infuse.Collections.Tests
{
    public class InstanceTraversalList_Tests
    {
        private class ServiceA : InfuseAs<ServiceA> { }

        private class ServiceB : InfuseAs<ServiceB>
        {
            private void OnInfuse(ServiceA serviceA) { }
        }

        private class ServiceC : InfuseAs<ServiceC>
        {
            private void OnInfuse(ServiceA serviceA, ServiceB serviceB) { }
        }

        private class ServiceD : InfuseAs<ServiceD>
        {
            private void OnInfuse(ServiceC serviceC) { }
        }

        [Test]
        public void Empty()
        {
            var instanceTraversalList = new InstanceTraversalList<object>(new InfuseTypeInfoCache());

            var enumerator = instanceTraversalList.GetEnumerator();
            
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void SingleInstance()
        {
            var instanceTraversalList = new InstanceTraversalList<object>(new InfuseTypeInfoCache());
            var serviceA = new ServiceA();
            
            instanceTraversalList.Add(serviceA, serviceA);
            instanceTraversalList.ApplyUpdates();
            
            var enumerator = instanceTraversalList.GetEnumerator();
            
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(serviceA, enumerator.Current);
        }

        [Test]
        public void MultipleInstances()
        {
            var instanceTraversalList = new InstanceTraversalList<object>(new InfuseTypeInfoCache());
            var serviceA = new ServiceA();
            var serviceB = new ServiceB();
            
            instanceTraversalList.Add(serviceB, serviceB);
            instanceTraversalList.Add(serviceA, serviceA);
            instanceTraversalList.ApplyUpdates();

            var enumerator = instanceTraversalList.GetEnumerator();
            
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(serviceA, enumerator.Current);
            
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(serviceB, enumerator.Current);
        }

        [Test]
        public void MultipleInstancesWithRemove()
        {
            var instanceTraversalList = new InstanceTraversalList<object>(new InfuseTypeInfoCache());
            var serviceA = new ServiceA();
            var serviceB = new ServiceB();
            
            instanceTraversalList.Add(serviceB, serviceB);
            instanceTraversalList.Add(serviceA, serviceA);
            instanceTraversalList.Remove(serviceB);
            instanceTraversalList.ApplyUpdates();

            var enumerator = instanceTraversalList.GetEnumerator();
            
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(serviceA, enumerator.Current);
        }

        [Test]
        public void MultipleInstancesBruteForce()
        {
            var orderedList = new List<object>
            {
                new ServiceA(), new ServiceA(), new ServiceA(),
                new ServiceB(), new ServiceB(), new ServiceB(),
                new ServiceC(), new ServiceC(), new ServiceC(),
                new ServiceD(), new ServiceD(), new ServiceD()
            };

            for (int testRun = 0; testRun < 1000; ++testRun)
            {
                var instanceTraversalList = new InstanceTraversalList<object>(new InfuseTypeInfoCache());

                var shuffled = orderedList.OrderBy(x => Guid.NewGuid().ToString()).ToList();
                
                foreach (var service in shuffled)
                {
                    instanceTraversalList.Add(service, service);
                }
                
                instanceTraversalList.ApplyUpdates();

                var outList = new List<object>();

                foreach (var service in instanceTraversalList)
                {
                    outList.Add(service);
                }

                Assert.AreEqual(orderedList.Count, outList.Count);
                
                for (int i = 0; i < orderedList.Count; i++)
                {
                    // Order is unspecified within the same type.
                    Assert.AreEqual(orderedList[i].GetType(), outList[i].GetType());
                }
            }
        }

        [Test]
        public void MultipleInstancesTransitiveBruteForce()
        {
            var orderedList = new List<object>
            {
                new ServiceA(), new ServiceA(), new ServiceA(),
                new ServiceD(), new ServiceD(), new ServiceD()
            };

            for (int testRun = 0; testRun < 1000; ++testRun)
            {
                var instanceTraversalList = new InstanceTraversalList<object>(new InfuseTypeInfoCache());

                var shuffled = orderedList.OrderBy(x => Guid.NewGuid().ToString()).ToList();
                
                foreach (var service in shuffled)
                {
                    instanceTraversalList.Add(service, service);
                }
                
                instanceTraversalList.ApplyUpdates();

                var outList = new List<object>();

                foreach (var service in instanceTraversalList)
                {
                    outList.Add(service);
                }

                Assert.AreEqual(orderedList.Count, outList.Count);
                
                for (int i = 0; i < orderedList.Count; i++)
                {
                    // Order is unspecified within the same type.
                    Assert.AreEqual(orderedList[i].GetType(), outList[i].GetType());
                }
            }
        }
    }
}
