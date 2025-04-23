using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class BubblePool : SimplePool, InfuseService<InfuseServiceStack<BubblePool>>
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
