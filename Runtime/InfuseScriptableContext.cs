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
        public InfuseTypeInfoMap TypeMap => GetBaseContext().TypeMap;
        public InfuseInstanceMap InstanceMap => GetBaseContext().InstanceMap;
        public InfuseServiceMap ServiceMap => GetBaseContext().ServiceMap;

        [SerializeField]
        private InfuseScriptableContext _parentContext = null;
        
        private InfuseBaseContext _baseContext;

        // Unity's ScriptableObject lifetime isn't particularly clearly defined
        // and changes between editor, player and platform. So we'll lazily
        // initialise the context and discard it for replacement in OnDisable().
        private void OnDisable()
        {
            _baseContext = null;
        }

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

        /**
         * Invoked to lazily create the stored InfuseBaseContext. Override this
         * to create a custom context.
         * @return The InfuseBaseContext instance.
         */
        protected virtual InfuseBaseContext GetBaseContext()
        {
            _baseContext ??= new InfuseBaseContext(_parentContext?.ServiceMap);

            return _baseContext;
        }
    }
}
