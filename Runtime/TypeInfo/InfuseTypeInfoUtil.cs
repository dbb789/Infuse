using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Infuse.Collections;
using Infuse.Common;
using Infuse.TypeInfo;

namespace Infuse.TypeInfo
{
    public static class InfuseTypeInfoUtil
    {
        // This is pretty heavyweight, but it's only called once per type as needed.
        public static InfuseTypeInfo CreateInfuseTypeInfo(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var provides = new HashSet<Type>();
            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (TryGetServiceType(interfaceType, out var serviceType))
                {
                    if (serviceType.IsAssignableFrom(type) ||
                        typeof(ServiceContainer).IsAssignableFrom(serviceType))
                    {
                        provides.Add(serviceType);
                    }
                    else
                    {
                        // Log this here just the once and ignore the
                        // declaration so that we don't end up spewing the same
                        // thing into the log over and over again.
                        Debug.LogError($"Infuse: {serviceType} is not assignable from {type}");
                    }
                }
            }

            // Recursively look down the inheritance hierarchy to find the first
            // valid instances of OnInfuse() and OnDefuse(). This should result
            // in a behaviour very similar to how Unity finds Awake(), Start()
            // etc. If we find invalid OnInfuse()/OnDefuse() methods, we'll
            // ignore them and spew into the error log (see below).
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

            var onInfuseFunc = OnInfuseFuncUtil.Create(type, infuseMethod);
            var onDefuseFunc = OnDefuseFuncUtil.Create(type, defuseMethod);

            return new InfuseTypeInfo(type,
                                      provides.AsEnumerable<Type>(),
                                      onInfuseFunc,
                                      onDefuseFunc);
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
                if (OnInfuseFuncUtil.ValidateMethod(method))
                {
                    if (infuseMethod == null)
                    {
                        infuseMethod = method;
                    }
                    else
                    {
                        Debug.LogError($"Infuse: Multiple OnInfuse methods found in {type}: {infuseMethod} and {method}");
                    }
                }
                
                if (OnDefuseFuncUtil.ValidateMethod(method))
                {
                    if (defuseMethod == null)
                    {
                        defuseMethod = method;
                    }
                    else
                    {
                        // I can't see how this could actually happen but
                        // just in case.
                        Debug.LogError($"Infuse: Multiple OnDefuse methods found in {type}: {defuseMethod} and {method}");
                    }
                }
            }
        }
        
        private static bool TryGetServiceType(Type type, out Type serviceType)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(InfuseAs<>) &&
                type.GenericTypeArguments.Length == 1)
            {
                serviceType = type.GenericTypeArguments[0];
                return true;
            }

            serviceType = default;
            
            return false;
        }
    }
}
