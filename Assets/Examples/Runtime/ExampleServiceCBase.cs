using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceCBase : MonoBehaviour
    {
        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        private void OnInfuse(ExampleServiceA exampleServiceA)
        {
            Debug.Log("ExampleServiceCBase.OnInfuse()");
        }
        
        private void OnDefuse()
        {
            Debug.Log("ExampleServiceCBase.OnDefuse()");
        }
    }
}
