using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Common;

namespace Infuse.Collections
{
    public class ServiceMap : IDisposable
    {
        public IEnumerable<KeyValuePair<Type, object>> Services => _serviceMap;
        
        private Dictionary<Type, object> _serviceMap;
        private ServiceMap _parent;

        public event Action<Type> OnServiceTypeRegistered;
        public event Action<Type> OnServiceTypeUnregistered;

        public ServiceMap(ServiceMap parent = null)
        {
            _serviceMap = new Dictionary<Type, object>();
            _parent = parent;

            if (_parent != null)
            {
                _parent.OnServiceTypeRegistered += InvokeServiceTypeRegistered;
                _parent.OnServiceTypeUnregistered += InvokeServiceTypeUnregistered;
            }
        }

        public void Dispose()
        {
            if (_parent != null)
            {
                _parent.OnServiceTypeRegistered -= InvokeServiceTypeRegistered;
                _parent.OnServiceTypeUnregistered -= InvokeServiceTypeUnregistered;
                _parent = null;
            }

            _serviceMap.Clear();
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
                
                RegisterServiceType(type, instance);
            }
            else if (typeof(ServiceContainer).IsAssignableFrom(type))
            {
                ServiceContainer container;
                bool toAdd = false;

                if (_serviceMap.TryGetValue(type, out var existing))
                {
                    container = (ServiceContainer)existing;
                }
                else
                {
                    container = (ServiceContainer)Activator.CreateInstance(type);
                    toAdd = true;
                }

                container.Register(instance);

                // Only add the new container if registration completed without
                // throwing and the container is confirmed to be populated.
                if (toAdd && container.Populated)
                {
                    RegisterServiceType(type, container);
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
                
                UnregisterServiceType(type);
            }
            else if (typeof(ServiceContainer).IsAssignableFrom(type))
            {
                if (_serviceMap.TryGetValue(type, out var existing))
                {
                    var container = (ServiceContainer)existing;
                    
                    container.Unregister(instance);
                    
                    // Remove unpopulated container.
                    if (!container.Populated)
                    {
                        UnregisterServiceType(type);
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
        
        public bool Contains(Type service)
        {
            if (_serviceMap.ContainsKey(service))
            {
                return true;
            }

            if (_parent != null)
            {
                return _parent.Contains(service);
            }

            return false;
        }
        
        public bool ContainsAll(TypeSet serviceList)
        {
            foreach (var service in serviceList)
            {
                if (!Contains(service))
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

            if (_parent != null)
            {
                return _parent.GetService(type);
            }
            
            throw new InfuseException($"Service of type {type} is not registered.");
        }

        private void RegisterServiceType(Type type, object instance)
        {
            _serviceMap.Add(type, instance);
            InvokeServiceTypeRegistered(type);
        }

        private void UnregisterServiceType(Type type)
        {
            _serviceMap.Remove(type);
            InvokeServiceTypeUnregistered(type);
        }

        private void InvokeServiceTypeRegistered(Type type)
        {
            OnServiceTypeRegistered?.Invoke(type);
        }

        private void InvokeServiceTypeUnregistered(Type type)
        {
            OnServiceTypeUnregistered?.Invoke(type);
        }
    }
}
