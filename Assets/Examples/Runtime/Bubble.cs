using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class Bubble : MonoBehaviour
    {
        private BubblePool _bubblePool;
        private Vector3 _velocity;
        private float _lifeTime;

        private void Awake()
        {
            InfuseManager.Infuse(this);
        }
        
        private void OnInfuse(BubblePool bubblePool)
        {
            _bubblePool = bubblePool;
        }

        private void OnDefuse()
        {
            _bubblePool = null;
        }

        private void OnEnable()
        {
            transform.position = Vector3.zero;
            
            _velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + Random.Range(0.5f, 1.5f);
        }

        private void Update()
        {
            if (_bubblePool == null)
            {
                return;
            }

            transform.position += _velocity * Time.deltaTime;
            
            if (Time.time >= _lifeTime)
            {
                _bubblePool.Recycle(gameObject);
            }
        }
    }
}
