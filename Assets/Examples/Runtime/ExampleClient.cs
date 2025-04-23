using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleClient : MonoBehaviour, InfuseService<InfuseServiceCollection<ExampleClient>>
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
