using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace Infuse
{
    public class AsyncOnInfuseFuncBuilder : IOnInfuseFuncBuilder
    {
        private List<Func<object, InfuseServiceMap, Awaitable>> _funcList;
        private HashSet<Type> _dependencies;

        public AsyncOnInfuseFuncBuilder()
        {
            _funcList = new();
            _dependencies = null;
        }

        public void AddMethod(MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                Debug.LogWarning($"Infuse: Generic methods are not supported: {method.Name}");
                return;
            }
            
            if (method.ReturnType == typeof(void))
            {
                AddSyncMethod(method);
            }
            else if (method.ReturnType == typeof(Awaitable))
            {
                AddAsyncMethod(method);
            }
            else
            {
                Debug.LogWarning($"Infuse: Return type must be void or Awaitable: {method.Name}");
            }
        }
        
        private void AddSyncMethod(MethodInfo method)
        {
            _dependencies ??= new HashSet<Type>();
            
            var parameterTypeArray = OnInfuseFuncBuilderUtil.CreateParameterTypeArray(method.GetParameters(), _dependencies);
            
            Func<object, InfuseServiceMap, Awaitable> invoker = (instance, serviceMap) =>
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

                return CompletedAwaitable();
            };

            _funcList.Add(invoker);
        }
        
        private void AddAsyncMethod(MethodInfo method)
        {
            _dependencies ??= new HashSet<Type>();
            
            var parameterTypeArray = OnInfuseFuncBuilderUtil.CreateParameterTypeArray(method.GetParameters(), _dependencies);
            
            Func<object, InfuseServiceMap, Awaitable> invoker = (instance, serviceMap) =>
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
                
                return (Awaitable)method.Invoke(instance, parameters);
            };
            
            _funcList.Add(invoker);
        }

        public OnInfuseFunc Build()
        {
            var funcArray = _funcList.ToArray();
            
            var infuseFunc = new OnInfuseFunc(async (instance, serviceMap, infuseType, completionHandler) =>
            {
                try
                {
                    foreach (var func in funcArray)
                    {
                        await func.Invoke(instance, serviceMap);
                    }

                    completionHandler.OnInfuseCompleted(infuseType, instance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Infuse: Exception in OnInfuseFunc: {e}");
                }
            }, _dependencies);

            // Reset the builder state.
            _funcList.Clear();
            _dependencies = null;

            return infuseFunc;
        }

        private static readonly AwaitableCompletionSource completionSource = new();
        
        private static Awaitable CompletedAwaitable()
        {
            completionSource.SetResult();
            
            var awaitable = completionSource.Awaitable;
            
            completionSource.Reset();
            
            return awaitable;
        }
    }
}
