using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Collections;
using Infuse.Util;

namespace Infuse
{
    public class InfuseBaseContext : InfuseContext
    {
        public InfuseTypeInfoMap TypeMap => _typeMap;
        public InfuseInstanceMap InstanceMap => _instanceMap;
        public InfuseServiceMap ServiceMap => _serviceMap;
        
        private readonly InfuseTypeInfoMap _typeMap;
        private readonly InfuseServiceMap _serviceMap;
        private readonly InfuseInstanceMap _instanceMap;
        private readonly Action<object> _destroyCancellationCallback;
        private readonly Action<InfuseTypeInfo, object> _onInfuseCompleted;
        
        public InfuseBaseContext()
        {
            _typeMap = new();
            _serviceMap = new();
            _instanceMap = new();
            _destroyCancellationCallback = (instance) => Unregister(instance);
            _onInfuseCompleted = OnInfuseCompleted;
        }
        
        public void Register(object instance, bool unregisterOnDestroy = true)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            
            var type = instance.GetType();
            
            if (_instanceMap.Contains(type, instance))
            {
                Debug.LogWarning($"Instance of type {type} is already registered.");
                return;
            }

            IDisposable disposable = null;
            
            if (unregisterOnDestroy && instance is MonoBehaviour monoBehaviour)
            {
                disposable = monoBehaviour.destroyCancellationToken.Register(_destroyCancellationCallback, instance);
            }

            _instanceMap.Add(type, instance, disposable);
            
            var infuseType = GetInfuseType(type);
            
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
            
            if (!_instanceMap.Contains(type, instance))
            {
                Debug.LogWarning($"Instance of type {type} is not registered.");
                return;
            }

            var infuseType = GetInfuseType(type);
            
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
            var instanceSet = _instanceMap.GetInstanceSet(typeInfo.InstanceType);

            foreach (var instance in instanceSet)
            {
                OnResolved(typeInfo, instance);
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
            var instanceSet = _instanceMap.GetInstanceSet(typeInfo.InstanceType);

            foreach (var instance in instanceSet)
            {
                OnUnresolved(typeInfo, instance);
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
            
            foreach (var requiredType in _typeMap.GetTypesRequiringService(serviceType))
            {
                UpdateResolvedState(requiredType);
            }
        }

        private void UnregisterService(Type serviceType, object instance)
        {
            _serviceMap.Unregister(serviceType, instance);
            
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
