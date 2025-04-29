using System;
using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class ExampleServiceA : MonoBehaviour, IExampleServiceA, InfuseAs<IExampleServiceA>
    {
        private void OnEnable()
        {
            InfuseGlobalContext.Register(this, false);
        }

        private void OnDisable()
        {
            InfuseGlobalContext.Unregister(this);
        }

        private void OnInfuse()
        {
            Debug.Log("ExampleServiceA.OnInfuse()", gameObject);
        }
        
        private void OnDefuse()
        {
            Debug.Log("ExampleServiceA.OnDefuse()", gameObject);
        }
    }
}
