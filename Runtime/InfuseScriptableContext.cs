using UnityEngine;
using Infuse.Collections;

namespace Infuse
{
    /**
     * InfuseScriptableContext is a ScriptableObject that wraps an
     * InfuseBaseContext to be tied to a Unity asset and viewed in the editor.
     */
    [CreateAssetMenu(menuName = "Infuse/InfuseScriptableContext", fileName = "InfuseScriptableContext")]
    public class InfuseScriptableContext : ScriptableObject, InfuseContext
    {
        public InfuseTypeInfoMap TypeMap => _baseContext.TypeMap;
        public InfuseInstanceMap InstanceMap => _baseContext.InstanceMap;
        public InfuseServiceMap ServiceMap => _baseContext.ServiceMap;
        
        private InfuseBaseContext _baseContext = new();
        
        /**
         * Register an object instance with this Infuse Context.
         * @param instance The object instance to register.
         * @param unregisterOnDestroy If true, the object will be unregistered when it is destroyed.
         */
        public void Register(object instance, bool unregisterOnDestroy = true)
        {
            _baseContext.Register(instance, unregisterOnDestroy);
        }

        /**
         * Unregister an object instance from this Infuse Context.
         * @param instance The object instance to unregister.
         */
        public void Unregister(object instance)
        {
            _baseContext.Unregister(instance);
        }

        /**
         * Register a service instance with this Infuse Context.
         * @param instance The service instance to register.
         * @typeparam TServiceType The type of the service.
         */
        public void RegisterService<TServiceType>(object instance) where TServiceType : class
        {
            _baseContext.RegisterService<TServiceType>(instance);
        }

        /**
         * Unregister a service instance from this Infuse Context.
         * @param instance The service instance to unregister.
         * @typeparam TServiceType The type of the service.
         */
        public void UnregisterService<TServiceType>(object instance) where TServiceType : class
        {
            _baseContext.UnregisterService<TServiceType>(instance);
        }
    }
}
