using UnityEngine;

namespace Infuse.Examples
{
    public interface ISimplePool
    {
        GameObject Get();
        void Recycle(GameObject go);
    }
}
