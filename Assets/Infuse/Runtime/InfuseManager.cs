using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public static class InfuseManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            _context = new InfuseContext();
        }

        private static InfuseContext _context;

        public static Awaitable Infuse(object instance)
        {
            return _context.Infuse(instance);
        }
    }
}
