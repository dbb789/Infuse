using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceB : MonoBehaviour, InfuseProvides<ExampleServiceB>
    {
        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        private async Awaitable OnInfuse()
        {
            Debug.Log(">> ExampleServiceB.OnInfuse()");

            await Awaitable.NextFrameAsync();

            Debug.Log("<< ExampleServiceB.OnInfuse()");
        }
        
        private void OnDefuse()
        {
            Debug.Log("ExampleServiceB.OnDefuse()");
        }
    }
}
