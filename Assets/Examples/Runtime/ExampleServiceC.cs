using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceC : ExampleServiceCBase, InfuseService<ExampleServiceC>
    {
        private void OnInfuse(ExampleServiceB exampleServiceB)
        {
            Debug.Log("ExampleServiceC.OnInfuse()");
        }
        
        private void OnDefuse()
        {
            Debug.Log("ExampleServiceC.OnDefuse()");
        }
    }
}
