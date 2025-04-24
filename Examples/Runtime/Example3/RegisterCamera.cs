using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    [RequireComponent(typeof(Camera))]
    public class RegisterCamera : MonoBehaviour
    {
        private void OnEnable()
        {
            InfuseManager.RegisterService<InfuseServiceCollection<Camera>>(GetComponent<Camera>());
        }

        private void OnDisable()
        {
            InfuseManager.UnregisterService<InfuseServiceCollection<Camera>>(GetComponent<Camera>());
        }
    }
}
