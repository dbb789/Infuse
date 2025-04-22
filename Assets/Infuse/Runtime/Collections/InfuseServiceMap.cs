using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Common;

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
                if (!_serviceMap.ContainsKey(type) && !CreateContainerType(type))
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

            if (CreateContainerType(type))
            {
                return _serviceMap[type];
            }

            throw new InfuseException($"Service of type {type} is not registered.");
        }

        private bool CreateContainerType(Type type)
        {
            if (!typeof(InfuseServiceContainer).IsAssignableFrom(type))
            {
                return false;
            }
            
            var container = Activator.CreateInstance(type);
            
            _serviceMap.Add(type, container);

            return true;
        }
    }
}
