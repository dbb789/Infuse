using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceA : MonoBehaviour, InfuseProvides<ExampleServiceA>
    {
        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        private void OnInfuse()
        {
            Debug.Log("ExampleServiceA.OnInfuse()");
        }
        
        private void OnDefuse()
        {
            Debug.Log("ExampleServiceA.OnDefuse()");
        }
    }
}
