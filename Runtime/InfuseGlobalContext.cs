using System;
using System.Collections.Generic;
using UnityEngine;
using Infuse.Common;

namespace Infuse
{
    /**
     * InfuseGlobalContext is a static convenience class that provides methods to
     * register and unregister object instances and services with the Global
     * Infuse Context.
     */
    public static class InfuseGlobalContext
    {
        private static readonly InfuseContext _context = GetContext();

        /**
         * Registers an object instance with the Global Infuse Context.
         * @param instance The object instance to register.
         * @param unregisterOnDestroy If true, the instance will be unregistered when it is destroyed.
         */
        public static void Register(object instance, bool unregisterOnDestroy = true)
        {
            _context.Register(instance, unregisterOnDestroy);
        }

        /**
         * Unregisters an object instance from the Global Infuse Context.
         * @param instance The object instance to unregister.
         */
        public static void Unregister(object instance)
        {
            _context.Unregister(instance);
        }

        /**
         * Registers a service instance with the Global Infuse Context.
         * @param instance The service instance to register.
         * @typeparam TServiceType The type of the service.
         */
        public static void RegisterService<TServiceType>(object instance) where TServiceType : class
        {
            _context.RegisterService<TServiceType>(instance);
        }

        /**
         * Unregisters a service instance from the Global Infuse Context.
         * @param instance The service instance to unregister.
         * @typeparam TServiceType The type of the service.
         */
        public static void UnregisterService<TServiceType>(object instance) where TServiceType : class
        {
            _context.UnregisterService<TServiceType>(instance);
        }

        public static InfuseContext GetContext()
        {
            Debug.Log("Infuse: Getting Global Infuse Context.");
            var context = Resources.Load<InfuseScriptableContext>("InfuseGlobalContext");

            if (context == null)
            {
                throw new InfuseException("InfuseGlobalContext asset not found.");
            }
            
            return context;
        }
    }
}
