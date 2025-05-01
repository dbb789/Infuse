using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    // This class is infused as a ServiceStack. This means that we can have
    // multiple instances of the same service, with one overriding the other.
    public class BubblePool : SimplePool, InfuseAs<ServiceStack<ISimplePool>>
    {
        private void OnEnable()
        {
            // We're calling Defuse() ourselves here, so pass false as a second
            // argument to disable automatically calling Defuse() on
            // MonoBehaviour destroy.
            InfuseGlobalContext.Register(this, false);
        }

        private void OnDisable()
        {
            InfuseGlobalContext.Unregister(this);
        }

        protected override GameObject CreateInstance(GameObject prefab, Transform parent)
        {
            var go = Instantiate(prefab, parent);

            // Pass the Bubble this pool instance so that it can recycle itself.
            go.GetComponent<Bubble>().Initialize(this);
            
            return go;
        }
    }
}
