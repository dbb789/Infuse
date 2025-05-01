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
    public static class OnDefuseFuncUtil
    {
        public static OnDefuseFunc Create(Type type, MethodInfo method)
        {
            if (method == null)
            {
                return OnDefuseFunc.Null;
            }

            // Building a lambda expression here should be a little bit faster
            // and also seems to save us an unnecessary heap allocation.
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var invokeExpression = Expression.Lambda<Action<object>>(
                Expression.Call(Expression.Convert(instanceParameter, type), method),
                instanceParameter);

            var invokeFunc = invokeExpression.Compile();
            
            return new OnDefuseFunc((instance) =>
            {
                try
                {
                    invokeFunc(instance);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Infuse: Exception in OnDefuseFunc: {e.Message}");
                    Debug.LogException(e);
                }
            });                
        }

        public static bool ValidateMethod(MethodInfo method)
        {
            if (method.Name != "OnDefuse")
            {
                return false;
            }

            if (method.IsAbstract)
            {
                return false;
            }

            if (method.IsGenericMethod)
            {
                Debug.LogError($"Infuse: OnDefuse method cannot be generic: {method}");
                return false;
            }
            
            if (method.ReturnType != typeof(void))
            {
                Debug.LogError($"Infuse: OnDefuse method must return void: {method}");
                return false;
            }

            if (method.GetParameters().Length != 0)
            {
                Debug.LogError($"Infuse: OnDefuse method cannot have parameters: {method}");
                return false;
            }

            return true;
        }
    }
}
