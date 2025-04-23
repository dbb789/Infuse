using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    [RequireComponent(typeof(Camera))]
    public class RegisterCamera : MonoBehaviour
    {
        private void Awake()
        {
            InfuseManager.RegisterService<InfuseServiceCollection<Camera>>(GetComponent<Camera>());
        }
    }
}
