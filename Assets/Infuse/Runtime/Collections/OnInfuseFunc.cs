using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse.Collections
{
    public class OnInfuseFunc
    {
        public HashSet<Type> Dependencies => _dependencies;

        private Action<object, InfuseServiceMap, InfuseTypeInfo, IInfuseCompletionHandler> _func;
        private HashSet<Type> _dependencies;
        
        public OnInfuseFunc() : this(null, null)
        {
            // ..
        }

        public OnInfuseFunc(Action<object, InfuseServiceMap, InfuseTypeInfo, IInfuseCompletionHandler> func,
                            HashSet<Type> dependencies)
        {
            _func = func;
            _dependencies = dependencies ?? new HashSet<Type>();
        }

        public void Invoke(object instance,
                           InfuseServiceMap serviceMap,
                           InfuseTypeInfo typeInfo,
                           IInfuseCompletionHandler completionHandler)
        {
            if (_func != null)
            {
                _func.Invoke(instance, serviceMap, typeInfo, completionHandler);
            }
            else
            {
                completionHandler.OnInfuseCompleted(typeInfo, instance);
            }
        }
    }
}
