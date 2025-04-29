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
        public static InfuseContext Context { get; private set; } = GetContext();
        
        /**
         * Registers an object instance with the Global Infuse Context.
         * @param instance The object instance to register.
         * @param unregisterOnDestroy If true, the instance will be unregistered when it is destroyed.
         */
        public static void Register(object instance, bool unregisterOnDestroy = true)
        {
            Context.Register(instance, unregisterOnDestroy);
        }

        /**
         * Unregisters an object instance from the Global Infuse Context.
         * @param instance The object instance to unregister.
         */
        public static void Unregister(object instance)
        {
            Context.Unregister(instance);
        }

        /**
         * Registers a service instance with the Global Infuse Context.
         * @param instance The service instance to register.
         * @typeparam TServiceType The type of the service.
         */
        public static void RegisterService<TServiceType>(object instance) where TServiceType : class
        {
            Context.RegisterService<TServiceType>(instance);
        }

        /**
         * Unregisters a service instance from the Global Infuse Context.
         * @param instance The service instance to unregister.
         * @typeparam TServiceType The type of the service.
         */
        public static void UnregisterService<TServiceType>(object instance) where TServiceType : class
        {
            Context.UnregisterService<TServiceType>(instance);
        }

        private static InfuseContext GetContext()
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
