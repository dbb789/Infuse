using System;
using System.Collections.Generic;

namespace Infuse
{
    public class OnInfuseFunc
    {
        public HashSet<Type> Dependencies => _dependencies;

        private Action<object, InfuseInstanceMap> _func;
        private HashSet<Type> _dependencies;

        public OnInfuseFunc(Action<object, InfuseInstanceMap> func,
                            HashSet<Type> dependencies)
        {
            _func = func;
            _dependencies = dependencies ?? new HashSet<Type>();
        }
        
        public void Invoke(object instance,
                           InfuseInstanceMap instanceMap)
        {
            _func?.Invoke(instance, instanceMap);
        }
    }
}
