using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseServiceMap
    {
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
            
            Debug.Log($"Registering service of type {type}.");
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

            Debug.Log($"Unregistering service of type {type}.");
            _serviceMap.Remove(type);
        }

        public bool ContainsAll(HashSet<Type> requiredServices)
        {
            foreach (var type in requiredServices)
            {
                if (!_serviceMap.ContainsKey(type))
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

            throw new InfuseException($"Service of type {type} is not registered.");
        }
    }
}
