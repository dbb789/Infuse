using UnityEngine;
using Infuse.Collections;

namespace Infuse
{
    // Wraps an InfuseBaseContext in a ScriptableObject so that we can tie it to
    // a Unity asset and play around with it in the editor.
    [CreateAssetMenu(menuName = "Infuse/InfuseScriptableContext", fileName = "InfuseScriptableContext")]
    public class InfuseScriptableContext : ScriptableObject, InfuseContext
    {
        public InfuseTypeInfoMap TypeMap => _baseContext.TypeMap;
        public InfuseInstanceMap InstanceMap => _baseContext.InstanceMap;
        public InfuseServiceMap ServiceMap => _baseContext.ServiceMap;
        
        private InfuseBaseContext _baseContext = new();

        public void Register(object instance, bool unregisterOnDestroy = true)
        {
            _baseContext.Register(instance, unregisterOnDestroy);
        }
        
        public void Unregister(object instance)
        {
            _baseContext.Unregister(instance);
        }

        public void RegisterService<TServiceType>(object instance) where TServiceType : class
        {
            _baseContext.RegisterService<TServiceType>(instance);
        }

        public void UnregisterService<TServiceType>(object instance) where TServiceType : class
        {
            _baseContext.UnregisterService<TServiceType>(instance);
        }
    }
}
