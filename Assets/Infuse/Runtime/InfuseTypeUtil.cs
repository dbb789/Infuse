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

            var methods = new List<MethodInfo>();
            var baseType = type;

            while (baseType != null && baseType != typeof(object))
            {
                methods.AddRange(baseType.GetMethods(BindingFlags.Public |
                                                     BindingFlags.NonPublic |
                                                     BindingFlags.Instance));
                
                baseType = baseType.BaseType;
            }

            var infuseMethods = new List<MethodInfo>();
            var defuseMethods = new List<MethodInfo>();

            foreach (var method in Enumerable.Reverse(methods))
            {
                if (method.Name == "OnInfuse")
                {
                    infuseMethods.Add(method);
                }
            }

            foreach (var method in methods)
            {
                if (method.Name == "OnDefuse")
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
            bool hasAsync = false;

            foreach (var method in methods)
            {
                if (method.ReturnType == typeof(Awaitable))
                {
                    hasAsync = true;
                    break;
                }
            }
            
            IOnInfuseFuncBuilder builder = hasAsync ?
                new AsyncOnInfuseFuncBuilder() :
                new SyncOnInfuseFuncBuilder();
            
            foreach (var method in methods)
            {
                builder.AddMethod(method);
            }

            return builder.Build();
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
