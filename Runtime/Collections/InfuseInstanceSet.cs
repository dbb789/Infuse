using System;
using System.Collections.Generic;

namespace Infuse.Collections
{
    public class InfuseInstanceSet
    {
        public struct Enumerator
        {
            private Dictionary<object, IDisposable>.Enumerator _enumerator;
            
            public Enumerator(Dictionary<object, IDisposable> instanceSet)
            {
                _enumerator = instanceSet.GetEnumerator();
            }

            public object Current => _enumerator.Current.Key;
            public bool MoveNext() => _enumerator.MoveNext();
            public void Dispose() => _enumerator.Dispose();
        }
        
        public int Count => _instanceSet.Count;
        
        private Dictionary<object, IDisposable> _instanceSet;

        public InfuseInstanceSet()
        {
            _instanceSet = new();
        }

        public bool Add(object instance, IDisposable disposable = null)
        {
            if (!_instanceSet.ContainsKey(instance))
            {
                _instanceSet.Add(instance, disposable);

                return true;
            }

            return false;
        }

        public bool Remove(object instance)
        {
            if (_instanceSet.TryGetValue(instance, out var disposable))
            {
                _instanceSet.Remove(instance);
                disposable?.Dispose();

                return true;
            }

            return false;
        }

        public bool Contains(object instance)
        {
            return _instanceSet.ContainsKey(instance);
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_instanceSet);
        }
    }
}
