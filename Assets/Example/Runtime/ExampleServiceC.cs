using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceC : MonoBehaviour, InfuseProvides<ExampleServiceC>
    {
        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        private void OnInfuse(ExampleServiceA exampleServiceA, ExampleServiceB exampleServiceB)
        {
            Debug.Log("ExampleServiceC.OnInfuse()");
        }
        
        private void OnDefuse()
        {
            Debug.Log("ExampleServiceC.OnDefuse()");
        }
    }
}
