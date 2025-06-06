using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class ExampleServiceB : MonoBehaviour, IExampleServiceB, InfuseAs<IExampleServiceB>
    {
        private void Awake()
        {
            InfuseGlobalContext.Register(this);
        }

        private async Awaitable OnInfuse()
        {
            Debug.Log("ExampleServiceB.OnInfuse()", gameObject);

            await Awaitable.NextFrameAsync();
        }
        
        private void OnDefuse()
        {
            Debug.Log("ExampleServiceB.OnDefuse()", gameObject);
        }
    }
}
