using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class OnInfuseFunc
    {
        public HashSet<Type> Dependencies => _dependencies;

        private Func<object, InfuseServiceMap, Awaitable> _func;
        private HashSet<Type> _dependencies;

        public OnInfuseFunc(Func<object, InfuseServiceMap, Awaitable> func,
                            HashSet<Type> dependencies)
        {
            _func = func;
            _dependencies = dependencies ?? new HashSet<Type>();
        }

        public void Invoke(object instance,
                           InfuseServiceMap serviceMap,
                           InfuseType infuseType,
                           IInfuseCompletionHandler completionHandler)
        {
            InvokeAsync(instance, serviceMap, infuseType, completionHandler);
        }

        private async void InvokeAsync(object instance,
                                       InfuseServiceMap serviceMap,
                                       InfuseType infuseType,
                                       IInfuseCompletionHandler completionHandler)
        {
            try
            {
                await _func?.Invoke(instance, serviceMap);
                
                completionHandler.OnInfuseCompleted(infuseType, instance);
            }
            catch (Exception e)
            {
                Debug.LogError($"Infusion failed for {instance.GetType()}: {e.Message}");
            }
        }
    }
}
