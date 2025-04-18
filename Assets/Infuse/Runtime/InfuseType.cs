using System;
using System.Collections.Generic;

namespace Infuse
{
    public class InfuseType
    {
        public Type Type => _type;
        public HashSet<Type> Provides => _provides;
        public HashSet<Type> Requires => _requires;

        private Type _type;
        
        private HashSet<Type> _provides;
        private HashSet<Type> _requires;
        
        private OnInfuseFunc _infuseFunc;
        private OnDefuseFunc _defuseFunc;
        
        public InfuseType(Type type,
                          IEnumerable<Type> provides,
                          OnInfuseFunc infuseFunc,
                          OnDefuseFunc defuseFunc)
        {
            _type = type;
            
            _infuseFunc = infuseFunc ?? throw new ArgumentNullException(nameof(infuseFunc));
            _defuseFunc = defuseFunc ?? throw new ArgumentNullException(nameof(defuseFunc));
            
            _provides = new HashSet<Type>(provides ?? Array.Empty<Type>());
            _requires = new HashSet<Type>(_infuseFunc.Dependencies);
        }
        
        public void Infuse(object instance, InfuseInstanceMap instanceMap)
        {
            _infuseFunc.Invoke(instance, instanceMap);
        }

        public void Defuse(object instance)
        {
            _defuseFunc.Invoke(instance);
        }

        public override string ToString()
        {
            return $"Provides: {string.Join(", ", _provides)} / Requires: {string.Join(", ", _requires)}";
        }
    }
}
