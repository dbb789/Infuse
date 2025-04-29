using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Collections;
using Infuse.Util;

namespace Infuse
{
    public class InfuseBaseContext : InfuseContext, IDisposable
    {
        public InfuseTypeInfoMap TypeMap => _typeMap;
        public ServiceMap ServiceMap => _serviceMap;
        public InstanceMap InstanceMap => _instanceMap;
        
        private readonly InfuseTypeInfoMap _typeMap;
        private readonly ServiceMap _serviceMap;
        private readonly InstanceMap _instanceMap;
        private readonly Action<object> _destroyCancellationCallback;
        private readonly Action<InfuseTypeInfo, object> _onInfuseCompleted;
        
        public InfuseBaseContext(ServiceMap parentServiceMap = null)
        {
            _typeMap = new();
            _serviceMap = new(parentServiceMap);
            _instanceMap = new();
            _destroyCancellationCallback = (instance) => Unregister(instance);
            _onInfuseCompleted = OnInfuseCompleted;
            
            _serviceMap.OnServiceTypeRegistered += ServiceTypeStateUpdated;
            _serviceMap.OnServiceTypeUnregistered += ServiceTypeStateUpdated;
            _serviceMap.Register(typeof(InfuseContext), this);
        }

        public void Dispose()
        {
            foreach (var type in _instanceMap.Types)
            {
                if (_instanceMap.TryGetInstanceSet(type, out var instanceSet))
                {
                    if (instanceSet.Count > 0)
                    {
                        Debug.LogError($"Infuse: Instances of type {type} are still registered while disposing context. These will be unregistered.");
                    }

                    foreach (var instance in instanceSet)
                    {
                        // Will call OnDefuse() on the instance, hopefully forcing a cleanup and avoiding later errors.
                        Unregister(instance);
                    }
                }
            }

            _instanceMap.Dispose();

            _serviceMap.Unregister(typeof(InfuseContext), this);
            _serviceMap.OnServiceTypeRegistered -= ServiceTypeStateUpdated;
            _serviceMap.OnServiceTypeUnregistered -= ServiceTypeStateUpdated;
            _serviceMap.Dispose();

            _typeMap.Dispose();
        }
        
        public void Register(object instance, bool unregisterOnDestroy = true)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            var infuseType = GetInfuseType(type);

            // This type has no interaction with Infuse and can be ignored. See
            // also InfuseTypeInfo.cs.
            if (infuseType.Empty)
            {
                return;
            }

            if (_instanceMap.Contains(type, instance))
            {
                Debug.LogWarning($"Infuse: Instance of type {type} is already registered.");
                return;
            }

            IDisposable disposable = null;
            
            if (unregisterOnDestroy && instance is MonoBehaviour monoBehaviour)
            {
                disposable = monoBehaviour.destroyCancellationToken.Register(_destroyCancellationCallback, instance);
            }

            _instanceMap.Add(type, instance, disposable);
            
            if (infuseType.Resolved)
            {
                OnResolved(infuseType, instance);
            }
        }

        public void Unregister(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            var infuseType = GetInfuseType(type);

            // As above.
            if (infuseType.Empty)
            {
                return;
            }

            if (!_instanceMap.Contains(type, instance))
            {
                Debug.LogWarning($"Infuse: Instance of type {type} is not registered.");
                return;
            }
            
            if (infuseType.Resolved)
            {
                OnUnresolved(infuseType, instance);
            }

            _instanceMap.Remove(type, instance);
        }
        
        public void RegisterService<TServiceType>(object instance) where TServiceType : class
        {
            RegisterService(typeof(TServiceType), instance);
        }
        
        public void UnregisterService<TServiceType>(object instance) where TServiceType : class
        {
            UnregisterService(typeof(TServiceType), instance);
        }

        private void UpdateResolvedState(InfuseTypeInfo infuseType)
        {
            bool nextResolved = _serviceMap.ContainsAll(infuseType.RequiredServices);

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

        private void OnResolved(InfuseTypeInfo typeInfo)
        {
            if (_instanceMap.TryGetInstanceSet(typeInfo.InstanceType, out var instanceSet))
            {
                foreach (var instance in instanceSet)
                {
                    OnResolved(typeInfo, instance);
                }
            }
        }

        private void OnResolved(InfuseTypeInfo typeInfo, object instance)
        {
            typeInfo.Infuse(instance, _serviceMap, _onInfuseCompleted);
        }

        private void OnInfuseCompleted(InfuseTypeInfo typeInfo, object instance)
        {
            foreach (var serviceType in typeInfo.ProvidedServices)
            {
                RegisterService(serviceType, instance);
            }
        }
        
        private void OnUnresolved(InfuseTypeInfo typeInfo)
        {
            if (_instanceMap.TryGetInstanceSet(typeInfo.InstanceType, out var instanceSet))
            {
                foreach (var instance in instanceSet)
                {
                    OnUnresolved(typeInfo, instance);
                }
            }
        }

        private void OnUnresolved(InfuseTypeInfo typeInfo, object instance)
        {
            foreach (var serviceType in typeInfo.ProvidedServices)
            {
                UnregisterService(serviceType, instance);
            }

            typeInfo.Defuse(instance);
        }

        private void RegisterService(Type serviceType, object instance)
        {
            _serviceMap.Register(serviceType, instance);
        }

        private void UnregisterService(Type serviceType, object instance)
        {
            _serviceMap.Unregister(serviceType, instance);
        }

        private void ServiceTypeStateUpdated(Type serviceType)
        {               
            foreach (var requiredType in _typeMap.GetTypesRequiringService(serviceType))
            {
                UpdateResolvedState(requiredType);
            }
        }

        private InfuseTypeInfo GetInfuseType(Type type)
        {
            InfuseTypeInfo typeInfo;
            
            if (!_typeMap.TryGetType(type, out typeInfo))
            {
                typeInfo = InfuseTypeInfoUtil.CreateInfuseTypeInfo(type);
                typeInfo.Resolved = _serviceMap.ContainsAll(typeInfo.RequiredServices);

                _typeMap.Add(typeInfo);
            }

            return typeInfo;
        }
    }
}
