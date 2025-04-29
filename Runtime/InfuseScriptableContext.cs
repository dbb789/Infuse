using UnityEngine;
using Infuse.Collections;
using Infuse.TypeInfo;

namespace Infuse
{
    /**
     * InfuseScriptableContext is a ScriptableObject that wraps an
     * InfuseBaseContext to be tied to a Unity asset and viewed in the editor.
     */
    public abstract class InfuseScriptableContext : ScriptableObject, InfuseContext
    {
        public InfuseTypeEntryMap TypeEntryMap => GetBaseContext().TypeEntryMap;
        public InstanceMap InstanceMap => GetBaseContext().InstanceMap;
        public ServiceMap ServiceMap => GetBaseContext().ServiceMap;

        /**
         * Register an object instance with this Infuse Context.
         * @param instance The object instance to register.
         * @param unregisterOnDestroy If true, the object will be unregistered when it is destroyed.
         */
        public void Register(object instance, bool unregisterOnDestroy = true)
        {
            GetBaseContext().Register(instance, unregisterOnDestroy);
        }

        /**
         * Unregister an object instance from this Infuse Context.
         * @param instance The object instance to unregister.
         */
        public void Unregister(object instance)
        {
            GetBaseContext().Unregister(instance);
        }

        /**
         * Register a service instance with this Infuse Context.
         * @param instance The service instance to register.
         * @typeparam TServiceType The type of the service.
         */
        public void RegisterService<TServiceType>(object instance) where TServiceType : class
        {
            GetBaseContext().RegisterService<TServiceType>(instance);
        }

        /**
         * Unregister a service instance from this Infuse Context.
         * @param instance The service instance to unregister.
         * @typeparam TServiceType The type of the service.
         */
        public void UnregisterService<TServiceType>(object instance) where TServiceType : class
        {
            GetBaseContext().UnregisterService<TServiceType>(instance);
        }

        protected abstract InfuseBaseContext GetBaseContext();
    }
}
