using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public static class InfuseManager
    {
        private static readonly InfuseContext _context = new();
        
        public static void Infuse(object instance)
        {
            _context.Infuse(instance);
        }
        
        public static void InfuseMonoBehaviour(MonoBehaviour instance)
        {
            _context.InfuseMonoBehaviour(instance);
        }

        public static void Defuse(object instance)
        {
            _context.Defuse(instance);
        }
    }
}
