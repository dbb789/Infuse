using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    // This class is infused as an InfuseServiceStack. This means that we can
    // have multiple instances of the same service, with one overriding the
    // other.
    public class BubblePoolWithInfuseContext : SimplePool, InfuseAs<ISimplePool>
    {
        [SerializeField]
        private InfuseScriptableContext _context;
        
        private void OnEnable()
        {
            // We're calling Defuse() ourselves here, so pass false as a second
            // argument to disable automatically calling Defuse() on
            // MonoBehaviour destroy.
            _context.Register(this, false);
        }

        private void OnDisable()
        {
            _context.Unregister(this);
        }

        protected override GameObject CreateInstance(GameObject prefab, Transform parent)
        {
            var go = Instantiate(prefab, parent);
            var bubble = go.GetComponent<BubbleWithInfuseContext>();

            bubble.Initialize(_context);
            
            return go;
        }
    }
}
