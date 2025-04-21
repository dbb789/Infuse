using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceC : ExampleServiceCBase, InfuseService<ExampleServiceC>
    {
        protected void OnInfuse(ExampleServiceA exampleServiceA, ExampleServiceB exampleServiceB)
        {
            base.OnInfuse(exampleServiceA);
            
            Debug.Log("ExampleServiceC.OnInfuse()");
        }
        
        protected override void OnDefuse()
        {
            Debug.Log("ExampleServiceC.OnDefuse()");
            
            base.OnDefuse();
        }
    }
}
