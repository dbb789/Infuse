using Infuse;
using NUnit.Framework;

namespace Infuse.Tests
{
    public class InfuseInstanceSet_Tests
    {
        [Test]
        public void Empty()
        {
            var instanceSet = new InfuseInstanceSet();
            
            Assert.AreEqual(instanceSet.Count, 0);

            var enumerator = instanceSet.GetEnumerator();
            
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void Add()
        {
            var instanceSet = new InfuseInstanceSet();
            var instance = new object();

            Assert.IsTrue(instanceSet.Add(instance));
            Assert.AreEqual(instanceSet.Count, 1);
            Assert.IsTrue(instanceSet.Contains(instance));
            
            var enumerator = instanceSet.GetEnumerator();
            
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(enumerator.Current, instance);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void Remove()
        {
            var instanceSet = new InfuseInstanceSet();
            var instance = new object();

            Assert.IsTrue(instanceSet.Add(instance));
            Assert.IsTrue(instanceSet.Remove(instance));

            Assert.IsFalse(instanceSet.Contains(instance));
            Assert.AreEqual(instanceSet.Count, 0);
            
            var enumerator = instanceSet.GetEnumerator();
            
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void Multiple()
        {
            var instanceSet = new InfuseInstanceSet();
            var instanceA = new object();
            var instanceB = new object();
            var instanceC = new object();

            Assert.IsTrue(instanceSet.Add(instanceA));
            Assert.IsTrue(instanceSet.Add(instanceB));

            Assert.AreEqual(instanceSet.Count, 2);
            Assert.IsTrue(instanceSet.Contains(instanceA));
            Assert.IsTrue(instanceSet.Contains(instanceB));
            Assert.IsFalse(instanceSet.Contains(instanceC));

            Assert.IsTrue(instanceSet.Add(instanceC));

            Assert.AreEqual(instanceSet.Count, 3);
            Assert.IsTrue(instanceSet.Contains(instanceA));
            Assert.IsTrue(instanceSet.Contains(instanceB));
            Assert.IsTrue(instanceSet.Contains(instanceC));

            Assert.IsTrue(instanceSet.Remove(instanceB));
            Assert.IsFalse(instanceSet.Remove(instanceB));

            Assert.AreEqual(instanceSet.Count, 2);
            Assert.IsTrue(instanceSet.Contains(instanceA));
            Assert.IsTrue(instanceSet.Contains(instanceC));
        }
    }
}
