using System;
using System.Collections.Generic;
using System.Linq;

namespace Infuse.Collections
{
    public class TypeSet
    {
        public struct Enumerator
        {
            private readonly Type [] _types;
            private int _index;

            public Enumerator(Type [] types)
            {
                _types = types;
                _index = -1;
            }

            public Type Current => _types[_index];
            
            public bool MoveNext()
            {
                return ++_index < _types.Length;
            }
        }
        
        public int Count => _types.Length;
        
        private readonly Type [] _types;
        
        public TypeSet()
        {
            _types = Array.Empty<Type>();
        }
        
        public TypeSet(IEnumerable<Type> types)
        {
            // Crude, but shouldn't be a performance issue.
            _types = new HashSet<Type>(types ?? Enumerable.Empty<Type>()).ToArray();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_types);
        }
    }
}
