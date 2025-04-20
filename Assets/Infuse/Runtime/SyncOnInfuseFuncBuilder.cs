using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Infuse
{
    public class SyncOnInfuseFuncBuilder : IOnInfuseFuncBuilder
    {
        private Action<object, InfuseServiceMap> _func;
        private HashSet<Type> _dependencies;

        public SyncOnInfuseFuncBuilder()
        {
            _func = null;
            _dependencies = null;
        }

        public void AddMethod(MethodInfo method)
        {
            if (method.ReturnType != typeof(void))
            {
                Debug.LogWarning($"Infuse: Return type must be void: {method.Name}");
            }

            _dependencies ??= new HashSet<Type>();
            
            var parameterTypeArray = OnInfuseFuncBuilderUtil.CreateParameterTypeArray(method.GetParameters(), _dependencies);
            
            Action<object, InfuseServiceMap> invoker = (instance, serviceMap) =>
            {
                var parameters = new object[parameterTypeArray.Length];
                
                for (int i = 0; i < parameterTypeArray.Length; i++)
                {
                    var parameterType = parameterTypeArray[i];
                    
                    if (serviceMap.TryGetService(parameterType, out var value))
                    {
                        parameters[i] = value;
                    }
                    else
                    {
                        throw new InfuseException($"Missing dependency: {parameterType}");
                    }
                }
                
                method.Invoke(instance, parameters);
            };

            if (_func != null)
            {
                _func += invoker;
            }
            else
            {
                _func = invoker;
            }
        }
        
        public OnInfuseFunc Build()
        {
            var func = _func;
            var infuseFunc = new OnInfuseFunc((instance, serviceMap, infuseType, completionHandler) =>
            {
                try
                {
                    func?.Invoke(instance, serviceMap);

                    completionHandler.OnInfuseCompleted(infuseType, instance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Infuse: Exception in OnInfuseFunc: {e}");
                }
            }, _dependencies);

            // Reset the builder state.
            _func = null;
            _dependencies = null;

            return infuseFunc;
        }
    }
}
