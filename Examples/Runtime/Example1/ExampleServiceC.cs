using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class ExampleServiceC : ExampleServiceCBase, InfuseAs<ExampleServiceC>
    {
        protected void OnInfuse(IExampleServiceA exampleServiceA, IExampleServiceB exampleServiceB)
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
