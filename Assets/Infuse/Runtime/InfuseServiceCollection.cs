using System;
using System.Collections.Generic;
using Infuse.Collections;

namespace Infuse
{
    public class InfuseServiceCollection<TServiceType> : InfuseTransient
    {
        public IEnumerable<TServiceType> Services => _services;
        public int Count => _services.Count;
        
        private readonly HashSet<TServiceType> _services;

        public event Action<TServiceType> OnServiceAdded;
        public event Action<TServiceType> OnServiceRemoved;

        public InfuseServiceCollection()
        {
            _services = new HashSet<TServiceType>();
        }
        
        public void Add(TServiceType instance)
        {
            _services.Add(instance);

            OnServiceAdded?.Invoke(instance);
        }

        public void Remove(TServiceType instance)
        {
            _services.Remove(instance);
            
            OnServiceRemoved?.Invoke(instance);
        }
    }
}
