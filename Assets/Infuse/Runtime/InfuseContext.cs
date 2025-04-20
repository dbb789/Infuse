using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseContext
    {
        private InfuseTypeMap _typeMap;
        private InfuseServiceMap _serviceMap;
        private InfuseInstanceMap _instanceMap;

        public InfuseContext()
        {
            _typeMap = new();
            _serviceMap = new();
            _instanceMap = new();
        }
        
        public Awaitable Infuse(MonoBehaviour instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            
            instance.destroyCancellationToken.Register(() => Defuse(instance));

            return Infuse((object)instance);
        }
        
        public async Awaitable Infuse(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            
            try
            {
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
                    await infuseType.Infuse(instance, _serviceMap);

                    foreach (var serviceType in infuseType.Provides)
                    {
                        _serviceMap.Register(serviceType, instance);

                        foreach (var requiredType in _typeMap.GetRequiresInfuseType(serviceType))
                        {
                            await UpdateResolvedState(requiredType);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Infuse failed for {instance.GetType()}: {e}");
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
                foreach (var serviceType in infuseType.Provides)
                {
                    _serviceMap.Unregister(serviceType, instance);

                    foreach (var requiredType in _typeMap.GetRequiresInfuseType(serviceType))
                    {
                        UpdateResolvedState(requiredType);
                    }
                }
                
                infuseType.Defuse(instance);
            }

            _instanceMap.Remove(type, instance);
        }
        
        private async Awaitable UpdateResolvedState(InfuseType infuseType)
        {
            bool nextResolved = IsResolved(infuseType);

            Debug.Log($"UpdateResolvedState: {infuseType.Type} -> {nextResolved}");
            
            if (infuseType.Resolved != nextResolved)
            {
                infuseType.Resolved = nextResolved;
                
                if (nextResolved)
                {
                    await OnResolved(infuseType);
                }
                else
                {
                    OnUnresolved(infuseType);
                }
                
                foreach (var requiredType in _typeMap.GetRequiresInfuseType(infuseType.Type))
                {
                    await UpdateResolvedState(requiredType);
                }
            }
        }

        private async Awaitable OnResolved(InfuseType infuseType)
        {
            var instanceSet = _instanceMap.GetInstanceSet(infuseType.Type);

            Debug.Log($"OnResolved: {infuseType.Type} -> {instanceSet.Count} instances");
            
            foreach (var instance in instanceSet)
            {
                await infuseType.Infuse(instance, _serviceMap);

                foreach (var serviceType in infuseType.Provides)
                {
                    _serviceMap.Register(serviceType, instance);
                }
            }
        }
        
        private void OnUnresolved(InfuseType infuseType)
        {
            var instanceSet = _instanceMap.GetInstanceSet(infuseType.Type);

            foreach (var instance in instanceSet)
            {
                foreach (var serviceType in infuseType.Provides)
                {
                    _serviceMap.Unregister(serviceType, instance);
                }

                infuseType.Defuse(instance);
            }
        }
        
        private bool IsResolved(InfuseType infuseType)
        {
            foreach (var requiredType in infuseType.Requires)
            {
                if (!_serviceMap.Contains(requiredType))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
