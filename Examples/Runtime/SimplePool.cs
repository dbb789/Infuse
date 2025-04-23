using System.Collections.Generic;
using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    // Very simple GameObject pool implementation for demonstration purposes.
    public abstract class SimplePool : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        
        [SerializeField]
        private int _initialSize = 10;

        private Stack<GameObject> _pool = new();

        private void Awake()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                var go = CreateInstance(_prefab, transform);
                
                go.SetActive(false);
                
                _pool.Push(go);
            }
        }
        
        private void OnEnable()
        {
            // We're calling Defuse() ourselves here, so pass false as a second
            // argument to disable automatically calling Defuse() on
            // MonoBehaviour destroy.
            InfuseManager.Infuse(this, false);
        }

        private void OnDisable()
        {
            InfuseManager.Defuse(this);
        }

        protected abstract GameObject CreateInstance(GameObject prefab, Transform parent);

        public GameObject Get()
        {
            if (_pool.Count > 0)
            {
                var go =  _pool.Pop();

                go.SetActive(true);

                return go;
            }
            else
            {
                return CreateInstance(_prefab, transform);
            }
        }

        public void Recycle(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            go.SetActive(false);
            
            _pool.Push(go);
        }
    }
}
