using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class Bubble : MonoBehaviour
    {
        private BubblePool _bubblePool;
        private Vector3 _velocity;
        private float _lifeTime;

        private void OnEnable()
        {
            transform.position = Vector3.zero;
            _velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + Random.Range(0.5f, 1.5f);

            // Technically we could do this in Awake() and it would work just
            // fine, but this is for demonstration purposes.
            InfuseManager.Infuse(this, false);
        }

        private void OnDisable()
        {
            InfuseManager.Defuse(this);
        }
        
        private void OnInfuse(BubblePool bubblePool)
        {
            _bubblePool = bubblePool;
        }

        private void OnDefuse()
        {
            _bubblePool = null;
        }

        private void Update()
        {
            // OnInfuse() may be called a few frames after OnEnable(), so check
            // that we've got a BubblePool first.
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
