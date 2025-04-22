using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleClient : MonoBehaviour
    {
        private ExampleServiceC _exampleServiceC;
        private InfuseServiceCollection<ExampleClient> _selfMap;
        
        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        private void OnInfuse(ExampleServiceC exampleServiceC,
                              InfuseServiceCollection<ExampleClient> selfMap)
        {
            Debug.Log("ExampleClient.OnInfuse()", gameObject);
            _exampleServiceC = exampleServiceC;
            _selfMap = selfMap;
            _selfMap.Add(this);

            Debug.Log($"ExampleClient instances : {_selfMap.Count}");
        }

        private void OnDefuse()
        {
            Debug.Log("ExampleClient.OnDefuse()", gameObject);

            _selfMap.Remove(this);
            _exampleServiceC = null;
            _selfMap = null;
        }
    }
}
