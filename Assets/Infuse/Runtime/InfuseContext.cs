using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseContext
    {
        // Wraps OnInfuseCompleted callback inside InfuseContext so that it's
        // not exposed as a public member.
        private class InfuseContextCompletionHandler : IInfuseCompletionHandler
        {
            private InfuseContext _context;
            
            public InfuseContextCompletionHandler(InfuseContext context)
            {
                _context = context;
            }
            
            public void OnInfuseCompleted(InfuseType infuseType, object instance)
            {
                _context.OnInfuseCompleted(infuseType, instance);
            }
        }

        private InfuseTypeMap _typeMap;
        private InfuseServiceMap _serviceMap;
        private InfuseInstanceMap _instanceMap;
        private InfuseContextCompletionHandler _completionHandler;
        
        public InfuseContext()
        {
            _typeMap = new();
            _serviceMap = new();
            _instanceMap = new();
            _completionHandler = new(this);
        }
        
        public void InfuseMonoBehaviour(MonoBehaviour instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            
            instance.destroyCancellationToken.Register(() => Defuse(instance));

            Infuse(instance);
        }
        
        public void Infuse(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            
            var type = instance.GetType();
            
            if (_instanceMap.Contains(type, instance))
            {
                Debug.LogWarning($"Instance of type {type} is already infused.");
                return;
            }
            
            _instanceMap.Add(type, instance);
            
            var infuseType = _typeMap.GetInfuseType(type);
            
            if (infuseType.Resolved)
            {
                OnResolved(infuseType, instance);
            }
        }
        
        public void Defuse(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            
            if (!_instanceMap.Contains(type, instance))
            {
                Debug.LogWarning($"Instance of type {type} is not infused.");
                return;
            }

            var infuseType = _typeMap.GetInfuseType(type);
            
            if (infuseType.Resolved)
            {
                OnUnresolved(infuseType, instance);
            }

            _instanceMap.Remove(type, instance);
        }
        
        private void UpdateResolvedState(InfuseType infuseType)
        {
            bool nextResolved = IsResolved(infuseType);

            if (infuseType.Resolved != nextResolved)
            {
                infuseType.Resolved = nextResolved;
                
                if (nextResolved)
                {
                    OnResolved(infuseType);
                }
                else
                {
                    OnUnresolved(infuseType);
                }
            }
        }

        private void OnResolved(InfuseType infuseType)
        {
            var instanceSet = _instanceMap.GetInstanceSet(infuseType.Type);

            foreach (var instance in instanceSet)
            {
                OnResolved(infuseType, instance);
            }
        }

        private void OnResolved(InfuseType infuseType, object instance)
        {
            infuseType.Infuse(instance, _serviceMap, _completionHandler);
        }

        private void OnInfuseCompleted(InfuseType infuseType, object instance)
        {
            foreach (var serviceType in infuseType.ProvidedServices)
            {
                RegisterService(serviceType, instance);
            }
        }
        
        private void OnUnresolved(InfuseType infuseType)
        {
            var instanceSet = _instanceMap.GetInstanceSet(infuseType.Type);

            foreach (var instance in instanceSet)
            {
                OnUnresolved(infuseType, instance);
            }
        }

        private void OnUnresolved(InfuseType infuseType, object instance)
        {
            foreach (var serviceType in infuseType.ProvidedServices)
            {
                UnregisterService(serviceType, instance);
            }

            infuseType.Defuse(instance);
        }

        private void RegisterService(Type serviceType, object instance)
        {
            _serviceMap.Register(serviceType, instance);
            
            foreach (var requiredType in _typeMap.GetTypesRequiringService(serviceType))
            {
                UpdateResolvedState(requiredType);
            }
        }

        public void UnregisterService(Type serviceType, object instance)
        {
            _serviceMap.Unregister(serviceType, instance);
            
            foreach (var requiredType in _typeMap.GetTypesRequiringService(serviceType))
            {
                UpdateResolvedState(requiredType);
            }
        }
        
        private bool IsResolved(InfuseType infuseType)
        {
            foreach (var serviceType in infuseType.RequiredServices)
            {
                if (!_serviceMap.Contains(serviceType))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
