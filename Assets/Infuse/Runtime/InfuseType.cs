using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseType
    {
        public Type Type => _type;
        public bool Resolved { get; set; }
        
        public HashSet<Type> ProvidedServices => _providedServices;
        public HashSet<Type> RequiredServices => _requiredServices;

        private Type _type;
        
        private HashSet<Type> _providedServices;
        private HashSet<Type> _requiredServices;
        
        private OnInfuseFunc _infuseFunc;
        private OnDefuseFunc _defuseFunc;
        
        public InfuseType(Type type,
                          IEnumerable<Type> providedServices,
                          OnInfuseFunc infuseFunc,
                          OnDefuseFunc defuseFunc)
        {
            _type = type;
            
            _infuseFunc = infuseFunc ?? throw new ArgumentNullException(nameof(infuseFunc));
            _defuseFunc = defuseFunc ?? throw new ArgumentNullException(nameof(defuseFunc));
            
            _providedServices = new HashSet<Type>(providedServices ?? Array.Empty<Type>());
            _requiredServices = new HashSet<Type>(_infuseFunc.Dependencies);

            // Any type with no dependencies is always resolved by definition.
            Resolved = (_requiredServices.Count == 0);
        }
        
        public void Infuse(object instance,
                           InfuseServiceMap serviceMap,
                           IInfuseCompletionHandler completionHandler)
        {
            _infuseFunc.Invoke(instance, serviceMap, this, completionHandler);
        }

        public void Defuse(object instance)
        {
            _defuseFunc.Invoke(instance);
        }

        public override string ToString()
        {
            return $"ProvidedServices: {string.Join(", ", _providedServices)} / RequiredServices: {string.Join(", ", _requiredServices)}";
        }
    }
}
