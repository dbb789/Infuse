using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Infuse.Collections;
using Infuse.Common;

namespace Infuse.TypeInfo
{
    public static class OnInfuseFuncUtil
    {
        public static OnInfuseFunc Create(Type type, MethodInfo method)
        {
            if (method == null)
            {
                return OnInfuseFunc.Null;
            }

            if (method.ReturnType == typeof(Awaitable))
            {
                return CreateAsyncOnInfuseFunc(type, method);
            }

            return CreateSyncOnInfuseFunc(type, method);
        }
        
        public static bool ValidateMethod(MethodInfo method)
        {
            if (method.Name != "OnInfuse")
            {
                return false;
            }
            
            if (method.IsAbstract)
            {
                return false;
            }

            if (method.IsGenericMethod)
            {
                Debug.LogError($"Infuse: OnInfuse method cannot be generic: {method}");
                return false;
            }

            if (method.ReturnType != typeof(void) &&
                method.ReturnType != typeof(Awaitable))
            {
                Debug.LogError($"Infuse: OnInfuse method must return void or Awaitable: {method}");
                return false;
            }

            return true;
        }

        private static OnInfuseFunc CreateSyncOnInfuseFunc(Type type, MethodInfo method)
        {
            var dependencies = new HashSet<Type>();
            var parameterTypeArray = CreateParameterTypeArray(method, dependencies);

            // Builds a lambda expression that looks something like;
            // (instance, serviceMap) => ((Type)instance).OnInfuse((Type1)serviceMap.GetService(typeof(Type1)),
            //                                                     (Type2)serviceMap.GetService(typeof(Type2)))

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var serviceMapParameter = Expression.Parameter(typeof(ServiceMap), "serviceMap");
            var invokeExpression = Expression.Lambda<Action<object, ServiceMap>>(
                Expression.Call(Expression.Convert(instanceParameter, type),
                                method,
                                parameterTypeArray.Select(
                                    t => Expression.Convert(Expression.Call(serviceMapParameter,
                                                                            typeof(ServiceMap).GetMethod("GetService"),
                                                                            Expression.Constant(t)), t)).ToArray()),
                instanceParameter,
                serviceMapParameter);

            var invokeFunc = invokeExpression.Compile();

            return new OnInfuseFunc((instance, serviceMap, infuseType, onInfuseCompleted) =>
            {
                try
                {
                    invokeFunc(instance, serviceMap);
                    onInfuseCompleted?.Invoke(infuseType, instance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Infuse: Exception in OnInfuseFunc: {e.Message}");
                    Debug.LogException(e);
                }
            }, dependencies);
        }

        private static OnInfuseFunc CreateAsyncOnInfuseFunc(Type type, MethodInfo method)
        {
            var dependencies = new HashSet<Type>();
            var parameterTypeArray = CreateParameterTypeArray(method, dependencies);
            var invokedMethod = method;

            // As above except we're returning the Awaitable.
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var serviceMapParameter = Expression.Parameter(typeof(ServiceMap), "serviceMap");
            var invokeExpression = Expression.Lambda<Func<object, ServiceMap, Awaitable>>(
                Expression.Call(Expression.Convert(instanceParameter, type),
                                method,
                                parameterTypeArray.Select(
                                    t => Expression.Convert(Expression.Call(serviceMapParameter,
                                                                            typeof(ServiceMap).GetMethod("GetService"),
                                                                            Expression.Constant(t)), t)).ToArray()),
                instanceParameter,
                serviceMapParameter);

            var invokeFunc = invokeExpression.Compile();

            return new OnInfuseFunc(async (instance, serviceMap, infuseType, onInfuseCompleted) =>
            {
                try
                {
                    await invokeFunc.Invoke(instance, serviceMap);
                    onInfuseCompleted?.Invoke(infuseType, instance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Infuse: Exception in OnInfuseFunc: {e.Message}");
                    Debug.LogException(e);
                }
            }, dependencies);
        }

        private static Type [] CreateParameterTypeArray(MethodInfo method, HashSet<Type> dependencies)
        {
            var parameters = method.GetParameters();
            var parameterTypeArray = new Type[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                
                if (parameter.IsIn)
                {
                    throw new InfuseException($"In parameters are not supported: {parameter.Name}");
                }
                
                if (parameter.IsOut)
                {
                    throw new InfuseException($"Out parameters are not supported: {parameter.Name}");
                }

                var parameterType = parameter.ParameterType;

                dependencies.Add(parameterType);
                
                parameterTypeArray[i] = parameterType;
            }

            return parameterTypeArray;
        }
    }
}
