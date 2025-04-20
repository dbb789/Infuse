using System;
using System.Collections.Generic;

namespace Infuse
{
    public class InfuseInstanceSet
    {
        public struct Enumerator
        {
            private HashSet<object>.Enumerator _enumerator;
            
            public Enumerator(HashSet<object> instanceSet)
            {
                _enumerator = instanceSet.GetEnumerator();
            }

            public object Current => _enumerator.Current;
            public bool MoveNext() => _enumerator.MoveNext();
            public void Dispose() => _enumerator.Dispose();
        }

        public object First
        {
            get
            {
                using var enumerator = _instanceSet.GetEnumerator();
                
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }

                throw new InvalidOperationException("No instances in set.");
            }
        }
        
        public int Count => _instanceSet.Count;
        
        private HashSet<object> _instanceSet;

        public InfuseInstanceSet()
        {
            _instanceSet = new();
        }
        
        public bool Add(object instance)
        {
            return _instanceSet.Add(instance);
        }

        public bool Remove(object instance)
        {
            return _instanceSet.Remove(instance);
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_instanceSet);
        }
    }
}
