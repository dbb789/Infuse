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

        protected void OnInfuse(ExampleServiceA exampleServiceA)
        {
            Debug.Log("ExampleServiceCBase.OnInfuse()");
        }
        
        protected virtual void OnDefuse()
        {
            Debug.Log("ExampleServiceCBase.OnDefuse()");
        }
    }
}
