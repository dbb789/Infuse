using System;
using Infuse.Collections;
using UnityEngine;

namespace Infuse.Util
{
    public static class InfuseServiceUtil
    {
        public static bool TryGetServiceType(Type declaration, out Type serviceType)
        {
            if (declaration.IsGenericType &&
                declaration.GetGenericTypeDefinition() == typeof(InfuseService<>) &&
                declaration.GenericTypeArguments.Length == 1)
            {
                serviceType = declaration.GenericTypeArguments[0];
                return true;
            }

            serviceType = default;
            
            return false;
        }

        public static bool IsTransientType(Type type)
        {
            return typeof(InfuseTransient).IsAssignableFrom(type);
        }
    }
}
