using System;
using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class ExampleServiceA : MonoBehaviour, InfuseAs<IExampleServiceA>, IExampleServiceA
    {
        private void OnEnable()
        {
            InfuseManager.Register(this);
        }

        private void OnDisable()
        {
            InfuseManager.Unregister(this);
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
