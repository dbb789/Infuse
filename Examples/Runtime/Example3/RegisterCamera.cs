using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    [RequireComponent(typeof(Camera))]
    public class RegisterCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            InfuseGlobalContext.RegisterService<InfuseServiceCollection<Camera>>(GetComponent<Camera>());
        }

        private void OnDisable()
        {
            InfuseGlobalContext.UnregisterService<InfuseServiceCollection<Camera>>(GetComponent<Camera>());
        }
    }
}
