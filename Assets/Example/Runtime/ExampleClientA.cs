using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleClientA : MonoBehaviour
    {
        private ExampleServiceC _exampleServiceC;

        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        private void OnInfuse(ExampleServiceC exampleServiceC)
        {
            Debug.Log("ExampleClientA.OnInfuse()");
            _exampleServiceC = exampleServiceC;
        }

        private void OnDefuse()
        {
            _exampleServiceC = null;
        }
    }
}
