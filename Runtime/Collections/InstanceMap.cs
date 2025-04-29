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
            GetInstanceSet(type).Add(instance, disposable);
        }
        
        public void Remove(Type type, object instance)
        {
            GetInstanceSet(type).Remove(instance);
        }

        public bool Contains(Type type, object instance)
        {
            if (_instanceMap.TryGetValue(type, out var instances))
            {
                return instances.Contains(instance);
            }

            return false;
        }
        
        public InstanceSet GetInstanceSet(Type type)
        {
            InstanceSet instanceSet;
            
            if (!_instanceMap.TryGetValue(type, out instanceSet))
            {
                instanceSet = new();
                _instanceMap.Add(type, instanceSet);
            }

            return instanceSet;
        }

        public bool Contains(Type type)
        {
            return (_instanceMap.TryGetValue(type, out var instanceList) &&
                    instanceList.Count > 0);
        }
    }
}
