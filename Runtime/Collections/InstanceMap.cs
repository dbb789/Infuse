using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class InstanceMap : IDisposable
    {
        public IEnumerable<Type> Types => _instanceMap?.Keys ?? Enumerable.Empty<Type>();
        
        private Dictionary<Type, InstanceSet> _instanceMap;

        public InstanceMap()
        {
            _instanceMap = new Dictionary<Type, InstanceSet>();
        }

        public void Dispose()
        {
            _instanceMap.Clear();
        }

        public void Add(Type type, object instance, IDisposable disposable = null)
        {
            GetCreateInstanceSet(type).Add(instance, disposable);
        }
        
        public void Remove(Type type, object instance)
        {
            GetCreateInstanceSet(type).Remove(instance);
        }

        public bool Contains(Type type, object instance)
        {
            if (TryGetInstanceSet(type, out var instanceSet))
            {
                return instanceSet.Contains(instance);
            }

            return false;
        }
        
        public bool TryGetInstanceSet(Type type, out InstanceSet instanceSet)
        {
            return _instanceMap.TryGetValue(type, out instanceSet);
        }
        
        public bool Contains(Type type)
        {
            return (TryGetInstanceSet(type, out var instanceSet) &&
                    instanceSet.Count > 0);
        }

        private InstanceSet GetCreateInstanceSet(Type type)
        {
            InstanceSet instanceSet;
            
            if (!_instanceMap.TryGetValue(type, out instanceSet))
            {
                instanceSet = new();
                _instanceMap.Add(type, instanceSet);
            }

            return instanceSet;
        }
    }
}
