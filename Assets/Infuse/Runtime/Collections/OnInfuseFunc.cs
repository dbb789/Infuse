using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class OnInfuseFunc
    {
        public List<Type> Dependencies => _dependencies;

        private Action<object, InfuseServiceMap, InfuseTypeInfo, IInfuseCompletionHandler> _func;
        private List<Type> _dependencies;
        
        public OnInfuseFunc() : this(null, null)
        {
            // ..
        }

        public OnInfuseFunc(Action<object, InfuseServiceMap, InfuseTypeInfo, IInfuseCompletionHandler> func,
                            IEnumerable<Type> dependencies)
        {
            _func = func;
            _dependencies = new List<Type>(dependencies ?? Enumerable.Empty<Type>());
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
