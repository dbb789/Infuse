using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseContext
    {
        private InfuseTypeMap _typeMap;
        private InfuseInstanceMap _instanceMap;
        
        public InfuseContext()
        {
            _typeMap = new();
            _instanceMap = new();
        }

        public async Awaitable Infuse(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            var infuseType = _typeMap.GetInfuseType(type);

            if (infuseType.Resolved)
            {
                await infuseType.Infuse(instance, _instanceMap);
                
                if (_instanceMap.Add(type, instance))
                {
                    foreach (var requiredType in _typeMap.GetRequiresInfuseType(type))
                    {
                        await UpdateResolvedState(requiredType);
                    }
                }
            }
            else
            {
                _instanceMap.Add(type, instance);
            }
        }
        
        public void Defuse(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            var infuseType = _typeMap.GetInfuseType(type);
            
            if (infuseType.Resolved)
            {
                infuseType.Defuse(instance);
                
                if (_instanceMap.Remove(type, instance))
                {
                    foreach (var requiredType in _typeMap.GetRequiresInfuseType(type))
                    {
                        UpdateResolvedState(requiredType);
                    }
                }
            }
            else
            {
                _instanceMap.Remove(type, instance);
            }
        }
        
        private async Awaitable UpdateResolvedState(InfuseType infuseType)
        {
            bool nextResolved = IsResolved(infuseType);

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

            foreach (var instance in instanceSet)
            {
                await infuseType.Infuse(instance, _instanceMap);
            }
        }
        
        private void OnUnresolved(InfuseType infuseType)
        {
            var instanceSet = _instanceMap.GetInstanceSet(infuseType.Type);

            foreach (var instance in instanceSet)
            {
                infuseType.Defuse(instance);
            }
        }
        
        private bool IsResolved(InfuseType infuseType)
        {
            foreach (var requiredType in infuseType.Requires)
            {
                bool hasProvider = false;
                var providesList = _typeMap.GetProvidesInfuseType(requiredType);

                foreach (var providesType in providesList)
                {
                    if (providesType.Resolved && _instanceMap.GetInstanceSet(providesType.Type).Count > 0)
                    {
                        hasProvider = true;
                        break;
                    }
                }

                if (!hasProvider)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
