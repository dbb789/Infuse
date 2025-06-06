using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class ExampleClient : MonoBehaviour, InfuseAs<ServiceCollection<ExampleClient>>
    {
        private ExampleServiceC _exampleServiceC;
        
        private void Awake()
        {
            InfuseGlobalContext.Register(this);
        }

        private void OnInfuse(ExampleServiceC exampleServiceC)
        {
            Debug.Log("ExampleClient.OnInfuse()", gameObject);
            _exampleServiceC = exampleServiceC;
        }

        private void OnDefuse()
        {
            Debug.Log("ExampleClient.OnDefuse()", gameObject);

            _exampleServiceC = null;
        }
    }
}
