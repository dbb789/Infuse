using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class ExampleClient : MonoBehaviour, InfuseAs<InfuseServiceCollection<ExampleClient>>
    {
        private ExampleServiceC _exampleServiceC;
        
        private void Awake()
        {
            InfuseManager.Infuse(this);
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
