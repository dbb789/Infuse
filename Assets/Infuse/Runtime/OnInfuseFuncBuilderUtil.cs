using System;
using System.Collections.Generic;
using System.Reflection;

namespace Infuse
{
    public static class OnInfuseFuncBuilderUtil
    {
        public static Type [] CreateParameterTypeArray(ParameterInfo [] parameters, HashSet<Type> dependencies)
        {
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
