using System;
using System.Collections.Generic;
using Infuse.Collections;

namespace Infuse
{
    public class InfuseServiceStack<TServiceType> : InfuseTransient
    {
        public TServiceType Current => _stack.Count > 0 ? _stack[_stack.Count - 1] : default;
        public bool HasCurrent => _stack.Count > 0;

        private readonly List<TServiceType> _stack;

        public InfuseServiceStack()
        {
            _stack = new(4);
        }
        
        public void Push(TServiceType instance)
        {
            _stack.Add(instance);
        }

        public void Pop(TServiceType instance)
        {
            _stack.Remove(instance);
        }
    }
}
