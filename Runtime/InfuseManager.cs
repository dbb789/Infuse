using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Common;

namespace Infuse
{
    public static class InfuseManager
    {
        private static readonly InfuseContext _context = GetGlobalContext();
        
        public static void Infuse(object instance, bool defuseOnDestroy = true)
        {
            _context.Infuse(instance, defuseOnDestroy);
        }
        
        public static void Defuse(object instance)
        {
            _context.Defuse(instance);
        }
        
        public static void RegisterService<TServiceType>(object instance) where TServiceType : class
        {
            _context.RegisterService<TServiceType>(instance);
        }
        
        public static void UnregisterService<TServiceType>(object instance) where TServiceType : class
        {
            _context.UnregisterService<TServiceType>(instance);
        }

        private static InfuseContext GetGlobalContext()
        {
            var context = Resources.Load<InfuseScriptableContext>("InfuseGlobalContext");

            if (context == null)
            {
                throw new InfuseException("InfuseGlobalContext asset not found.");
            }
            
            return context;
        }
    }
}
