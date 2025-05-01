using System.Collections.Generic;
using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class CameraSelector : MonoBehaviour
    {
        [SerializeField]
        private CameraSelectorButton _cameraSelectorButtonPrefab;
        
        [SerializeField]
        private Transform _cameraSelectorContainer;

        private ServiceCollection<Camera> _cameraCollection;
        private Dictionary<Camera, CameraSelectorButton> _cameraSelectorMap = new();
        
        private void Awake()
        {
            InfuseGlobalContext.Register(this);
        }

        private void OnInfuse(ServiceCollection<Camera> cameraCollection)
        {
            _cameraCollection = cameraCollection;

            // There should only be the one camera that caused the
            // InfuseServiceCollection to become available, but it's cleaner to
            // just add the whole collection.
            foreach (var camera in cameraCollection.Services)
            {
                AddCamera(camera);
            }

            _cameraCollection.OnServiceAdded += AddCamera;
            _cameraCollection.OnServiceRemoved += RemoveCamera;
        }

        private void OnDefuse()
        {
            _cameraCollection.OnServiceAdded -= AddCamera;
            _cameraCollection.OnServiceRemoved -= RemoveCamera;
            _cameraCollection = null;
        }

        private void AddCamera(Camera camera)
        {
            var button = Instantiate(_cameraSelectorButtonPrefab, _cameraSelectorContainer);

            button.Initialize(camera.name, camera);
            button.OnClick.AddListener(SelectCamera);
            
            _cameraSelectorMap.Add(camera, button);
        }

        private void RemoveCamera(Camera camera)
        {
            if (_cameraSelectorMap.TryGetValue(camera, out var button))
            {
                button.OnClick.RemoveListener(SelectCamera);
                
                Destroy(button.gameObject);
                
                _cameraSelectorMap.Remove(camera);
            }
        }

        private void SelectCamera(Camera selectedCamera)
        {
            foreach (var camera in _cameraCollection.Services)
            {
                camera.enabled = (camera == selectedCamera);
            }
        }
    }
}
