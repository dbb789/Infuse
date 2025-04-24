using System.Collections.Generic;
using UnityEngine;

namespace Infuse.Examples
{
    // Very simple GameObject pool implementation for demonstration purposes.
    public abstract class SimplePool : MonoBehaviour, ISimplePool
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
