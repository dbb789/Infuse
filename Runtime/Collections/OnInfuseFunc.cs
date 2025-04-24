using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class OnInfuseFunc
    {
        public static readonly OnInfuseFunc Null = new OnInfuseFunc();
        
        public bool Empty => _func == null;
        public IEnumerable<Type> Dependencies => _dependencies;

        public delegate void InfuseFunc(object instance,
                                        InfuseServiceMap serviceMap,
                                        InfuseTypeInfo typeInfo,
                                        Action<InfuseTypeInfo, object> onInfuseCompleted);

        private readonly InfuseFunc _func;
        private readonly Type [] _dependencies;

        private OnInfuseFunc()
        {
            _func = null;
            _dependencies = Array.Empty<Type>();
        }

        public OnInfuseFunc(InfuseFunc func,
                            IEnumerable<Type> dependencies)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
            _dependencies = dependencies?.ToArray() ?? Array.Empty<Type>();
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
