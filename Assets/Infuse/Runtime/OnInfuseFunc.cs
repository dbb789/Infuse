using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class OnInfuseFunc
    {
        public HashSet<Type> Dependencies => _dependencies;

        private Func<object, InfuseInstanceMap, Awaitable> _func;
        private HashSet<Type> _dependencies;

        public OnInfuseFunc(Func<object, InfuseInstanceMap, Awaitable> func,
                            HashSet<Type> dependencies)
        {
            _func = func;
            _dependencies = dependencies ?? new HashSet<Type>();
        }
        
        public Awaitable Invoke(object instance,
                                InfuseInstanceMap instanceMap)
        {
            return _func.Invoke(instance, instanceMap);
        }
    }
}
