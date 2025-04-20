using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public static class InfuseManager
    {
        private static readonly InfuseContext _context = new();
        
        public static Awaitable Infuse(MonoBehaviour instance)
        {
            return _context.Infuse(instance);
        }

        public static Awaitable Infuse(object instance)
        {
            return _context.Infuse(instance);
        }

        public static void Defuse(object instance)
        {
            _context.Defuse(instance);
        }
    }
}
