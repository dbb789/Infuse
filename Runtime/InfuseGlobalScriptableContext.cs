using UnityEngine;
using Infuse.Collections;
using Infuse.TypeInfo;

namespace Infuse
{
    public class InfuseGlobalScriptableContext : InfuseScriptableContext
    {
        private InfuseBaseContext _baseContext;

        private void OnDisable()
        {
            if (_baseContext != null)
            {
                _baseContext.Dispose();
                _baseContext = null;
            }
        }
        
        protected override InfuseBaseContext GetBaseContext()
        {
            _baseContext ??= new InfuseBaseContext(InfuseTypeInfoCache.GlobalInstance);

            return _baseContext;
        }
    }
}
