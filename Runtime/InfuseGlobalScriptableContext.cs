using UnityEngine;
using Infuse.Collections;

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
            _baseContext ??= new InfuseBaseContext(InfuseTypeInfoMap.GlobalInstance);

            return _baseContext;
        }
    }
}
