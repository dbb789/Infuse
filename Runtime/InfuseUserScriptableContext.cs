using UnityEngine;
using Infuse.Collections;

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
            _baseContext ??= new InfuseBaseContext(InfuseTypeInfoMap.GlobalInstance,
                                                   _parentContext?.ServiceMap);

            return _baseContext;
        }
    }
}
