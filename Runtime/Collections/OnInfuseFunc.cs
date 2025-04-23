using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class OnInfuseFunc
    {
        public List<Type> Dependencies => _dependencies;

        public delegate void InfuseFunc(object instance,
                                        InfuseServiceMap serviceMap,
                                        InfuseTypeInfo typeInfo,
                                        Action<InfuseTypeInfo, object> onInfuseCompleted);

        private readonly InfuseFunc _func;
        private readonly List<Type> _dependencies;

        public OnInfuseFunc() : this(null, null)
        {
            // ..
        }

        public OnInfuseFunc(InfuseFunc func,
                            IEnumerable<Type> dependencies)
        {
            _func = func;
            _dependencies = new List<Type>(dependencies ?? Enumerable.Empty<Type>());
        }

        public void Invoke(object instance,
                           InfuseServiceMap serviceMap,
                           InfuseTypeInfo typeInfo,
                           Action<InfuseTypeInfo, object> onInfuseCompleted)
        {
            if (_func != null)
            {
                _func.Invoke(instance, serviceMap, typeInfo, onInfuseCompleted);
            }
            else
            {
                onInfuseCompleted?.Invoke(typeInfo, instance);
            }
        }
    }
}
