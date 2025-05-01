using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    [RequireComponent(typeof(Camera))]
    public class RegisterCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            InfuseGlobalContext.RegisterService<ServiceCollection<Camera>>(GetComponent<Camera>());
        }

        private void OnDisable()
        {
            InfuseGlobalContext.UnregisterService<ServiceCollection<Camera>>(GetComponent<Camera>());
        }
    }
}
