using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public static class InfuseManager
    {
        private static readonly InfuseContext _context = new();
        
        public static void Infuse(object instance, bool defuseOnDestroy = true)
        {
            _context.Infuse(instance, defuseOnDestroy);
        }
        
        public static void Defuse(object instance)
        {
            _context.Defuse(instance);
        }
    }
}
