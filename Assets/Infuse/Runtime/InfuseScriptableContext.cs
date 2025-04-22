using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Collections;
using Infuse.Util;

namespace Infuse
{
    public class InfuseScriptableContext : ScriptableObject, InfuseContext
    {
        // Wraps OnInfuseCompleted callback inside InfuseScriptableContext so that it's
        // not exposed as a public member.
        private class InfuseContextCompletionHandler : IInfuseCompletionHandler
        {
            private InfuseScriptableContext _context;
            
            public InfuseContextCompletionHandler(InfuseScriptableContext context)
            {
                _context = context;
            }
            
            public void OnInfuseCompleted(InfuseTypeInfo infuseType, object instance)
            {
                _context.OnInfuseCompleted(infuseType, instance);
            }
        }

        public InfuseTypeInfoMap TypeMap => _typeMap;
        public InfuseInstanceMap InstanceMap => _instanceMap;

        private InfuseTypeInfoMap _typeMap;
        private InfuseServiceMap _serviceMap;
        private InfuseInstanceMap _instanceMap;
        private InfuseContextCompletionHandler _completionHandler;
        private Action<object> _destroyCancellationCallback;
        
        public InfuseScriptableContext()
        {
            _typeMap = new();
            _serviceMap = new();
            _instanceMap = new();
            _completionHandler = new(this);
            _destroyCancellationCallback = (instance) => Defuse(instance);
        }
        
        public void Infuse(object instance, bool defuseOnDestroy = true)
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

            IDisposable disposable = null;
            
            if (defuseOnDestroy && instance is MonoBehaviour monoBehaviour)
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

            var infuseType = GetInfuseType(type);
            
            if (infuseType.Resolved)
            {
                OnUnresolved(infuseType, instance);
            }

            _instanceMap.Remove(type, instance);
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
            typeInfo.Infuse(instance, _serviceMap, _completionHandler);
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

        public void UnregisterService(Type serviceType, object instance)
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
