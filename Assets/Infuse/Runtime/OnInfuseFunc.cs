using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class OnInfuseFunc
    {
        public HashSet<Type> Dependencies => _dependencies;

        private Action<object, InfuseServiceMap, InfuseType, IInfuseCompletionHandler> _func;
        private HashSet<Type> _dependencies;
        
        public OnInfuseFunc() : this(null, null)
        {
            // ..
        }

        public OnInfuseFunc(Action<object, InfuseServiceMap, InfuseType, IInfuseCompletionHandler> func,
                            HashSet<Type> dependencies)
        {
            _func = func;
            _dependencies = dependencies ?? new HashSet<Type>();
        }

        public void Invoke(object instance,
                           InfuseServiceMap serviceMap,
                           InfuseType infuseType,
                           IInfuseCompletionHandler completionHandler)
        {
            if (_func != null)
            {
                _func.Invoke(instance, serviceMap, infuseType, completionHandler);
            }
            else
            {
                completionHandler.OnInfuseCompleted(infuseType, instance);
            }
        }
    }
}
