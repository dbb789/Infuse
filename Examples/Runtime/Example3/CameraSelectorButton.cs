using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Infuse.Examples
{
    public class CameraSelectorButton : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _label;

        private Camera _camera;
        
        public UnityEvent<Camera> OnClick;
        
        public void Initialize(string cameraName, Camera camera)
        {
            _label.text = cameraName;
            _camera = camera;
        }

        public void OnButtonClick()
        {
            OnClick.Invoke(_camera);
        }
    }
}
