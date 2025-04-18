using System;
using System.Collections.Generic;

namespace Infuse
{
    public class OnDefuseFunc
    {
        private Action<object> _func;

        public OnDefuseFunc(Action<object> func)
        {
            _func = func;
        }
        
        public void Invoke(object instance)
        {
            _func?.Invoke(instance);
        }
    }
}
