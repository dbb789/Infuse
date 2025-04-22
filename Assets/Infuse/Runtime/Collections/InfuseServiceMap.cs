using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Common;
using Infuse.Util;

namespace Infuse.Collections
{
    public class InfuseServiceMap
    {
        public IEnumerable<Type> Types => _serviceMap.Keys;
        
        private Dictionary<Type, object> _serviceMap;

        public InfuseServiceMap()
        {
            _serviceMap = new Dictionary<Type, object>();
        }

        public void Register(Type type, object instance)
        {
            if (_serviceMap.ContainsKey(type))
            {
                throw new InfuseException($"Service of type {type} is already registered.");
            }
            
            Debug.Log($"Infuse: Registering service of type {type}.");
            _serviceMap.Add(type, instance);
        }
        
        public void Unregister(Type type, object instance)
        {
            if (!_serviceMap.TryGetValue(type, out var service))
            {
                throw new InfuseException($"Service of type {type} is not registered.");
            }

            if (service != instance)
            {
                throw new InfuseException($"Service of type {type} is not the same instance.");
            }

            Debug.Log($"Infuse: Unregistering service of type {type}.");
            _serviceMap.Remove(type);
        }

        public bool ContainsAll(HashSet<Type> requiredServices)
        {
            foreach (var type in requiredServices)
            {
                if (!_serviceMap.ContainsKey(type) && !TryCreateTransientType(type, out var _))
                {
                    return false;
                }
            }

            return true;
        }
        
        public object GetService(Type type)
        {
            if (_serviceMap.TryGetValue(type, out var instance))
            {
                return instance;
            }

            if (TryCreateTransientType(type, out var transientType))
            {
                return transientType;
            }

            throw new InfuseException($"Service of type {type} is not registered.");
        }

        private bool TryCreateTransientType(Type type, out object transientType)
        {
            if (!InfuseServiceUtil.IsTransientType(type))
            {
                transientType = default;
                
                return false;
            }

            if (!_serviceMap.TryGetValue(type, out transientType))
            {
                transientType = Activator.CreateInstance(type);
                _serviceMap.Add(type, transientType);
            }
            
            return true;
        }
    }
}
