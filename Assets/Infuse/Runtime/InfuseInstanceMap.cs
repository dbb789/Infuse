using System;
using System.Collections.Generic;
using System.Linq;

namespace Infuse
{
    public class InfuseInstanceMap
    {
        private Dictionary<Type, InfuseInstanceSet> _instanceMap;

        public InfuseInstanceMap()
        {
            _instanceMap = new Dictionary<Type, InfuseInstanceSet>();
        }

        public bool Add(Type type, object instance)
        {
            var instanceSet = GetInstanceSet(type);
            
            if (instanceSet.Add(instance))
            {
                return (instanceSet.Count == 1);
            }

            return false;
        }
        
        public bool Remove(Type type, object instance)
        {
            var instanceSet = GetInstanceSet(type);
            
            if (instanceSet.Remove(instance))
            {
                return (instanceSet.Count == 0);
            }

            return false;
        }

        public bool TryGetInstance(Type type, out object instance)
        {
            if (_instanceMap.TryGetValue(type, out var instances) && instances.Count > 0)
            {
                instance = instances.First;
                return true;
            }

            instance = default;
            
            return false;
        }
        
        public InfuseInstanceSet GetInstanceSet(Type type)
        {
            InfuseInstanceSet instanceSet;
            
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
