using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Collections;
using Infuse.Common;

namespace Infuse
{
    public class InfuseServiceStack<TServiceType> : InfuseServiceContainer<TServiceType>
        where TServiceType : class
    {
        public override bool Populated => _stack.Count > 0;
        public TServiceType Current => _stack.Count > 0 ? _stack[_stack.Count - 1] : null;
        
        private readonly List<TServiceType> _stack;

        public event Action<TServiceType> OnRegistered;
        public event Action<TServiceType> OnUnregistered;

        public InfuseServiceStack()
        {
            _stack = new List<TServiceType>(4);
        }
        
        public override void Register(TServiceType instance)
        {
            // Registering twice is going to be a mistake in almost all cases.
            // If you need to do this you should easily be able to make a
            // variant of this class that allows for it.
            if (_stack.Contains(instance))
            {
                throw new InfuseException($"Instance of type {typeof(TServiceType)} is already registered.");
            }
            
            InvokeUnregistered();
            
            _stack.Add(instance);
            
            InvokeRegistered();
        }

        public override void Unregister(TServiceType instance)
        {
            if (!_stack.Contains(instance))
            {
                throw new InfuseException($"Instance of type {typeof(TServiceType)} is not registered.");
            }
            
            bool isCurrent = (Current == instance);

            InvokeUnregistered();

            _stack.Remove(instance);

            if (isCurrent)
            {
                // Only notify if we've got a current instance. If not then
                // we're shortly going to unregister this class and
                // unresolve/OnDefuse() it's dependencies.
                if (_stack.Count > 0)
                {
                    InvokeRegistered();
                }
            }
        }

        // Catch exceptions here so that a badly behaved subscriber doesn't
        // break everything else.
        private void InvokeRegistered()
        {
            try
            {
                OnRegistered?.Invoke(Current);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void InvokeUnregistered()
        {
            try
            {
                OnUnregistered?.Invoke(Current);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override string ToString()
        {
            return $"{Current}";
        }
    }
}
