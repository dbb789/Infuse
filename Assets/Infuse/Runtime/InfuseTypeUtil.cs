using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Infuse
{
    public static class InfuseTypeUtil
    {
        public static InfuseType CreateInfuseType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var provides = new HashSet<Type>();
            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(InfuseService<>))
                {
                    foreach (var serviceType in interfaceType.GenericTypeArguments)
                    {
                        if (serviceType.IsAssignableFrom(type))
                        {
                            provides.Add(serviceType);
                        }
                        else
                        {
                            Debug.LogError($"Infuse: Type {type} does not derive from {serviceType} - ignoring declaration.");
                        }
                    }
                }
            }

            MethodInfo infuseMethod = null;
            MethodInfo defuseMethod = null;
            var baseType = type;
            
            while (baseType != null && baseType != typeof(object))
            {
                TryGetInfuseMethodsFromType(baseType,
                                            out var foundInfuseMethod,
                                            out var foundDefuseMethod);

                infuseMethod ??= foundInfuseMethod;
                defuseMethod ??= foundDefuseMethod;

                if (infuseMethod != null && defuseMethod != null)
                {
                    break;
                }
                
                baseType = baseType.BaseType;
            }
            
            return new InfuseType(type,
                                  provides.AsEnumerable<Type>(),
                                  CreateOnInfuseFunc(infuseMethod),
                                  CreateOnDefuseFunc(defuseMethod));
        }

        private static void TryGetInfuseMethodsFromType(Type type,
                                                        out MethodInfo infuseMethod,
                                                        out MethodInfo defuseMethod)
        {
            infuseMethod = null;
            defuseMethod = null;
            
            var methods = type.GetMethods(BindingFlags.Public |
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance |
                                          BindingFlags.DeclaredOnly);

            
            foreach (var method in methods)
            {
                if (method.Name == "OnInfuse")
                {
                    if (ValidateOnInfuseMethod(method))
                    {
                        if (infuseMethod == null)
                        {
                            infuseMethod = method;
                        }
                        else
                        {
                            Debug.LogError($"Infuse: Multiple OnInfuse methods found in {type}: {infuseMethod.Name} and {method.Name}");
                        }
                    }
                }
                
                if (method.Name == "OnDefuse")
                {
                    if (ValidateOnDefuseMethod(method))
                    {
                        if (defuseMethod == null)
                        {
                            defuseMethod = method;
                        }
                        else
                        {
                            // I can't see how this could actually happen but
                            // just in case.
                            Debug.LogError($"Infuse: Multiple OnDefuse methods found in {type}: {defuseMethod.Name} and {method.Name}");
                        }
                    }
                }
            }
        }

        private static OnInfuseFunc CreateOnInfuseFunc(MethodInfo method)
        {
            if (method == null)
            {
                return new OnInfuseFunc();
            }
            
            if (method.ReturnType == typeof(Awaitable))
            {
                return CreateAsyncOnInfuseFunc(method);
            }

            return CreateSyncOnInfuseFunc(method);
        }

        public static OnInfuseFunc CreateSyncOnInfuseFunc(MethodInfo method)
        {
            var dependencies = new HashSet<Type>();
            var parameterTypeArray = CreateParameterTypeArray(method, dependencies);
            var invokedMethod = method;
            
            return new OnInfuseFunc((instance, serviceMap, infuseType, completionHandler) =>
            {
                try
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
                    
                    invokedMethod.Invoke(instance, parameters);
                    completionHandler.OnInfuseCompleted(infuseType, instance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Infuse: Exception in OnInfuseFunc: {e}");
                }
            }, dependencies);
        }
        
        public static OnInfuseFunc CreateAsyncOnInfuseFunc(MethodInfo method)
        {
            var dependencies = new HashSet<Type>();
            var parameterTypeArray = CreateParameterTypeArray(method, dependencies);
            var invokedMethod = method;
            
            return new OnInfuseFunc(async (instance, serviceMap, infuseType, completionHandler) =>
            {
                try
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

                    await (Awaitable)invokedMethod.Invoke(instance, parameters);
                    completionHandler.OnInfuseCompleted(infuseType, instance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Infuse: Exception in OnInfuseFunc: {e}");
                }
            }, dependencies);
        }

        private static OnDefuseFunc CreateOnDefuseFunc(MethodInfo method)
        {
            if (method == null)
            {
                return new OnDefuseFunc();
            }
            
            return new OnDefuseFunc((instance) =>
            {
                method.Invoke(instance, null);
            });                
        }
        
        private static bool ValidateOnInfuseMethod(MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                Debug.LogError($"Infuse: OnInfuse method cannot be generic: {method.Name}");
                return false;
            }

            if (method.ReturnType != typeof(void) &&
                method.ReturnType != typeof(Awaitable))
            {
                Debug.LogError($"Infuse: OnInfuse method must return void or Awaitable: {method.Name}");
                return false;
            }

            return true;
        }

        private static bool ValidateOnDefuseMethod(MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                Debug.LogError($"Infuse: OnDefuse method cannot be generic: {method.Name}");
                return false;
            }

            if (method.ReturnType != typeof(void))
            {
                Debug.LogError($"Infuse: OnDefuse method must return void: {method.Name}");
                return false;
            }

            if (method.GetParameters().Length != 0)
            {
                Debug.LogError($"Infuse: OnDefuse method cannot have parameters: {method.Name}");
                return false;
            }

            return true;
        }

        public static Type [] CreateParameterTypeArray(MethodInfo method, HashSet<Type> dependencies)
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
