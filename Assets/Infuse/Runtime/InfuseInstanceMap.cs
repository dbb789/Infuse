using System;
using System.Collections.Generic;
using System.Linq;

namespace Infuse
{
    public class InfuseInstanceMap
    {
        private Dictionary<Type, List<object>> _instanceMap;

        public InfuseInstanceMap()
        {
            _instanceMap = new Dictionary<Type, List<object>>();
        }

        public int Add(Type type, object instance)
        {
            var list = GetInstanceList(type);

            list.Add(instance);

            return list.Count;
        }

        public bool TryGetInstance(Type type, out object instance)
        {
            if (_instanceMap.TryGetValue(type, out var instances) && instances.Count > 0)
            {
                instance = instances[0];
                return true;
            }

            instance = default;
            return false;
        }
        
        public List<object> GetInstanceList(Type type)
        {
            List<object> instanceList;
            
            if (!_instanceMap.TryGetValue(type, out instanceList))
            {
                instanceList = new List<object>();
                _instanceMap.Add(type, instanceList);
            }

            return instanceList;
        }

        public bool Contains(Type type)
        {
            return (_instanceMap.TryGetValue(type, out var instanceList) &&
                    instanceList.Count > 0);
        }

        public static void Move(Type type, InfuseInstanceMap source, InfuseInstanceMap dest)
        {
            var sourceList= source.GetInstanceList(type);
            var destList = dest.GetInstanceList(type);

            if (destList.Count > 0)
            {
                throw new InfuseException($"Cannot transfer instances of type {type} because the destination already has instances.");
            }

            source._instanceMap[type] = destList;
            dest._instanceMap[type] = sourceList;
        }
    }
}
