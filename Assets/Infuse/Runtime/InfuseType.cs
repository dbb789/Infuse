using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseType
    {
        public Type Type => _type;
        public bool Resolved { get; set; }
        
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

            // Any type with no dependencies is always resolved by definition.
            Resolved = (_requires.Count == 0);
        }
        
        public Awaitable Infuse(object instance,
                                InfuseServiceMap serviceMap)
        {
            return _infuseFunc.Invoke(instance, serviceMap);
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
