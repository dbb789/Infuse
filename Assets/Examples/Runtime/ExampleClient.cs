using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleClient : MonoBehaviour
    {
        private ExampleServiceC _exampleServiceC;

        private void Awake()
        {
            InfuseManager.InfuseMonoBehaviour(this);
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
