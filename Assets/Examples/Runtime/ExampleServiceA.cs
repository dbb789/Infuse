using System;
using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceA : MonoBehaviour, InfuseService<ExampleServiceA>
    {
        private void Awake()
        {
            InfuseManager.Infuse(this);
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
