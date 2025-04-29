using UnityEngine;
using Infuse.Collections;
using Infuse.TypeInfo;

namespace Infuse
{
    [CreateAssetMenu(menuName = "Infuse/InfuseUserScriptableContext", fileName = "InfuseUserScriptableContext")]
    public class InfuseUserScriptableContext : InfuseScriptableContext
    {
        [SerializeField]
        private InfuseScriptableContext _parentContext = null;
        
        private InfuseBaseContext _baseContext;

        private void OnDisable()
        {
            _baseContext = null;
        }
        
        protected override InfuseBaseContext GetBaseContext()
        {
            _baseContext ??= new InfuseBaseContext(InfuseTypeInfoCache.GlobalInstance,
                                                   _parentContext?.ServiceMap);

            return _baseContext;
        }
    }
}
