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
                        if (!serviceType.IsAssignableFrom(type))
                        {
                            throw new InfuseException($"Type {type} does not derive from {serviceType}");
                        }
                        
                        provides.Add(serviceType);
                    }
                }
            }
            
            var methods = type.GetMethods(BindingFlags.Public |
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance);

            var infuseMethods = new List<MethodInfo>();
            var defuseMethods = new List<MethodInfo>();

            foreach (var method in methods)
            {
                if (method.Name == "OnInfuse")
                {
                    infuseMethods.Add(method);
                }
                else if (method.Name == "OnDefuse")
                {
                    defuseMethods.Add(method);
                }
            }

            return new InfuseType(type,
                                  provides.AsEnumerable<Type>(),
                                  CreateOnInfuseFunc(infuseMethods),
                                  CreateOnDefuseFunc(defuseMethods));
        }


        private static OnInfuseFunc CreateOnInfuseFunc(List<MethodInfo> methods)
        {
            Func<object, InfuseServiceMap, Awaitable> func = null;
            var dependencies = new HashSet<Type>();
            
            foreach (var method in methods)
            {
                if (method.IsGenericMethod)
                {
                    throw new InfuseException($"Generic methods are not supported: {method.Name}");
                }

                if (method.ReturnType == typeof(void))
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

                        dependencies.Add(parameter.ParameterType);
                        parameterTypeArray[parameter.Position] = parameter.ParameterType;
                    }

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

                        return Awaitable.NextFrameAsync();
                    };

                    if (func != null)
                    {
                        func = async (instance, serviceMap) =>
                        {
                            await func(instance, serviceMap);
                            await invoker(instance, serviceMap);
                        };
                    }
                    else
                    {
                        func = invoker;
                    }
                }
                else if (method.ReturnType == typeof(Awaitable))
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

                        dependencies.Add(parameter.ParameterType);
                        parameterTypeArray[parameter.Position] = parameter.ParameterType;
                    }

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

                    if (func != null)
                    {
                        func = async (instance, serviceMap) =>
                        {
                            await func(instance, serviceMap);
                            await invoker(instance, serviceMap);
                        };
                    }
                    else
                    {
                        func = invoker;
                    }
                }
                else
                {
                    throw new InfuseException($"Return type must be void: {method.Name}");
                }
            }

            return new OnInfuseFunc(func, dependencies);
        }

        private static OnDefuseFunc CreateOnDefuseFunc(List<MethodInfo> methods)
        {
            Action<object> func = null;
            var dependencies = new HashSet<Type>();
            
            foreach (var method in methods)
            {
                if (method.IsGenericMethod)
                {
                    throw new InfuseException($"Generic methods are not supported: {method.Name}");
                }

                if (method.ReturnType != typeof(void))
                {
                    throw new InfuseException($"Return type must be void: {method.Name}");
                }

                if (method.GetParameters().Length != 0)
                {
                    throw new InfuseException($"OnDefuse method must have no parameters: {method.Name}");
                }
                
                Action<object> invoker = (instance) =>
                {
                    method.Invoke(instance, null);
                };

                if (func != null)
                {
                    func += invoker;
                }
                else
                {
                    func = invoker;
                }
            }

            return new OnDefuseFunc(func);
        }
    }
}
