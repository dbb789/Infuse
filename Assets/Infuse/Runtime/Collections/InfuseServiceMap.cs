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
            if (type.IsAssignableFrom(instance.GetType()))
            {
                if (_serviceMap.ContainsKey(type))
                {
                    throw new InfuseException($"Service of type {type} is already registered.");
                }

                Debug.Log($"Infuse: Registering service of type {type}.");
                _serviceMap.Add(type, instance);
            }
            else if (typeof(InfuseServiceContainer).IsAssignableFrom(type))
            {
                InfuseServiceContainer container;
                bool toAdd = false;

                if (_serviceMap.TryGetValue(type, out var existing))
                {
                    container = (InfuseServiceContainer)existing;
                }
                else
                {
                    container = (InfuseServiceContainer)Activator.CreateInstance(type);
                    toAdd = true;
                }

                container.Register(instance);

                // Only add the new container if registration completed without
                // throwing and the container is confirmed to be populated.
                if (toAdd && container.Populated)
                {
                    _serviceMap.Add(type, container);
                }
            }
            else
            {
                throw new InfuseException($"Type {type} is not assignable from {instance.GetType()}.");
            }
        }
        
        public void Unregister(Type type, object instance)
        {
            if (type.IsAssignableFrom(instance.GetType()))
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
            else if (typeof(InfuseServiceContainer).IsAssignableFrom(type))
            {
                if (_serviceMap.TryGetValue(type, out var existing))
                {
                    var container = (InfuseServiceContainer)existing;
                    
                    container.Unregister(instance);

                    // Remove unpopulated container.
                    if (!container.Populated)
                    {
                        _serviceMap.Remove(type);
                    }
                }
                else
                {
                    throw new InfuseException($"Service of type {type} is not registered.");
                }
            }
            else
            {
                throw new InfuseException($"Type {type} is not assignable from {instance.GetType()}.");
            }
        }

        public bool ContainsAll(List<Type> requiredServices)
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
