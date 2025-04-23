using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class InfuseTypeInfo
    {
        public Type InstanceType => _instanceType;
        public bool Resolved { get; set; }
        
        public List<Type> ProvidedServices => _providedServices;
        public List<Type> RequiredServices => _requiredServices;

        private readonly Type _instanceType;
        
        private readonly List<Type> _providedServices;
        private readonly List<Type> _requiredServices;

        private readonly OnInfuseFunc _infuseFunc;
        private readonly OnDefuseFunc _defuseFunc;
        
        public InfuseTypeInfo(Type instanceType,
                              IEnumerable<Type> providedServices,
                              IEnumerable<Type> requiredServices,
                              OnInfuseFunc infuseFunc,
                              OnDefuseFunc defuseFunc)
        {
            _instanceType = instanceType;
            
            _infuseFunc = infuseFunc ?? throw new ArgumentNullException(nameof(infuseFunc));
            _defuseFunc = defuseFunc ?? throw new ArgumentNullException(nameof(defuseFunc));
            
            _providedServices = new List<Type>(providedServices ?? Enumerable.Empty<Type>());
            _requiredServices = new List<Type>(requiredServices ?? Enumerable.Empty<Type>());

            // The context is always responsible for resolving the type, even if it's got no dependencie.
            Resolved = false;
        }
        
        public void Infuse(object instance,
                           InfuseServiceMap serviceMap,
                           Action<InfuseTypeInfo, object> onInfuseCompleted)
        {
            _infuseFunc.Invoke(instance, serviceMap, this, onInfuseCompleted);
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
