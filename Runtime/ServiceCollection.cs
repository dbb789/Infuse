using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Collections;

namespace Infuse
{
    public class ServiceCollection<TServiceType> : ServiceContainer<TServiceType>
        where TServiceType : class
    {
        public override bool Populated => _services.Count > 0;
        
        public IEnumerable<TServiceType> Services => _services;
        public int Count => _services.Count;
        
        private readonly HashSet<TServiceType> _services;

        public event Action<TServiceType> OnServiceAdded;
        public event Action<TServiceType> OnServiceRemoved;

        public ServiceCollection()
        {
            _services = new HashSet<TServiceType>();
        }
        
        public override void Register(TServiceType instance)
        {
            _services.Add(instance);

            InvokeServiceAdded(instance);
        }

        public override void Unregister(TServiceType instance)
        {
            _services.Remove(instance);
            
            InvokeServiceRemoved(instance);
        }
        
        // Catch exceptions here so that a badly behaved subscriber doesn't
        // break everything else.
        private void InvokeServiceAdded(TServiceType instance)
        {
            try
            {
                OnServiceAdded?.Invoke(instance);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void InvokeServiceRemoved(TServiceType instance)
        {
            try
            {
                OnServiceRemoved?.Invoke(instance);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override string ToString()
        {
            return string.Join(", ", _services);
        }
    }
}
