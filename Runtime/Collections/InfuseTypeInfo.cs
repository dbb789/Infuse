using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class InfuseTypeInfo
    {
        public Type InstanceType => _instanceType;
        public bool Empty => _empty;
        
        public List<Type> ProvidedServices => _providedServices;
        public List<Type> RequiredServices => _requiredServices;

        private readonly Type _instanceType;
        
        private readonly List<Type> _providedServices;
        private readonly List<Type> _requiredServices;

        private readonly OnInfuseFunc _infuseFunc;
        private readonly OnDefuseFunc _defuseFunc;
        
        private readonly bool _empty;
        
        public InfuseTypeInfo(Type instanceType,
                              IEnumerable<Type> providedServices,
                              OnInfuseFunc infuseFunc,
                              OnDefuseFunc defuseFunc)
        {
            _instanceType = instanceType;
            
            _infuseFunc = infuseFunc ?? throw new ArgumentNullException(nameof(infuseFunc));
            _defuseFunc = defuseFunc ?? throw new ArgumentNullException(nameof(defuseFunc));
            
            _providedServices = new List<Type>(providedServices ?? Enumerable.Empty<Type>());
            _requiredServices = new List<Type>(infuseFunc.Dependencies);

            // If we don't provide any services and we don't have an
            // OnInfuseFunc or OnDefuseFunc, then this type does not directly
            // interact with Infuse, and we can make Register()/Unregister() into a
            // no-op.
            _empty = (_providedServices.Count == 0) && _infuseFunc.Empty && _defuseFunc.Empty;

            if (_empty && _requiredServices.Count > 0)
            {
                // This of course should never, ever happen and indicates an internal bug.
                Debug.LogError($"Infuse: Type {instanceType} has required services but no provided services or infuse/defuse functions.");
            }
        }
        
        public void Infuse(object instance,
                           ServiceMap serviceMap,
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
