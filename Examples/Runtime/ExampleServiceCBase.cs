using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class ExampleServiceCBase : MonoBehaviour
    {
        private void Awake()
        {
            InfuseManager.Register(this);
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
