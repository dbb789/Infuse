using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class OnInfuseFunc
    {
        public HashSet<Type> Dependencies => _dependencies;

        private Func<object, InfuseServiceMap, Awaitable> _func;
        private HashSet<Type> _dependencies;

        public OnInfuseFunc(Func<object, InfuseServiceMap, Awaitable> func,
                            HashSet<Type> dependencies)
        {
            _func = func;
            _dependencies = dependencies ?? new HashSet<Type>();
        }
        
        public Awaitable Invoke(object instance,
                                InfuseServiceMap serviceMap)
        {
            return _func.Invoke(instance, serviceMap);
        }
    }
}
