using System;
using System.Collections.Generic;

namespace Infuse.Collections
{
    public class OnDefuseFunc
    {
        public static readonly OnDefuseFunc Null = new OnDefuseFunc();

        public bool Empty => _func == null;
        private Action<object> _func;

        private OnDefuseFunc()
        {
            _func = null;
        }
        
        public OnDefuseFunc(Action<object> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }
        
        public void Invoke(object instance)
        {
            _func?.Invoke(instance);
        }
    }
}
