using System.Collections.Generic;
using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class SimplePool : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        
        [SerializeField]
        private int _initialSize = 10;

        private Stack<GameObject> _pool;

        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        private async Awaitable OnInfuse()
        {
            Debug.Log("SimplePool.OnInfuse()", gameObject);
            
            _pool = new Stack<GameObject>();

            var goArray = await InstantiateAsync(_prefab, _initialSize, transform);

            foreach (var go in goArray)
            {
                go.SetActive(false);
                
                _pool.Push(go);
            }
        }

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
                return Instantiate(_prefab, transform);
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
