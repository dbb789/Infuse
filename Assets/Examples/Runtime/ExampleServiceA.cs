using System;
using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class ExampleServiceA : MonoBehaviour, InfuseService<ExampleServiceA>
    {
        private void OnEnable()
        {
            InfuseManager.Infuse(this);
        }

        private void OnDisable()
        {
            InfuseManager.Defuse(this);
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
