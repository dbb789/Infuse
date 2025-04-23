using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    // This class is infused as an InfuseServiceStack. This means that we can
    // have multiple instances of the same service, with one overriding the
    // other.
    public class BubblePool : SimplePool, InfuseAs<InfuseServiceStack<BubblePool>>
    {
        protected override GameObject CreateInstance(GameObject prefab, Transform parent)
        {
            var go = Instantiate(prefab, parent);

            // Pass the Bubble this pool instance so that it can recycle itself.
            go.GetComponent<Bubble>().Initialize(this);
            
            return go;
        }
    }
}
