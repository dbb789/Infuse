using System.Collections.Generic;
using Infuse.Collections;

namespace Infuse
{
    public class InfuseServiceCollection<TServiceType> : InfuseServiceContainer where TServiceType : class, new()
    {
        public IEnumerable<TServiceType> Services => _services;
        public int Count => _services.Count;
        
        private readonly HashSet<TServiceType> _services;

        public InfuseServiceCollection()
        {
            _services = new HashSet<TServiceType>();
        }
        
        public void Add(TServiceType instance)
        {
            _services.Add(instance);
        }

        public void Remove(TServiceType instance)
        {
            _services.Remove(instance);
        }
    }
}
