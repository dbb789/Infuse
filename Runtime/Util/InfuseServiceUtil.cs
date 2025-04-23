using System;
using Infuse.Collections;
using UnityEngine;

namespace Infuse.Util
{
    public static class InfuseServiceUtil
    {
        public static bool TryGetServiceType(Type type, out Type serviceType)
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
