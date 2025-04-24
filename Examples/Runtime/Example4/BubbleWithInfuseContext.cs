using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class BubbleWithInfuseContext : MonoBehaviour
    {
        private ISimplePool _parentPool;
        private IBubbleCounterService _bubbleCounter;
        
        private Vector3 _velocity;
        private float _lifeTime;

        // Note here that in this case, the parent pool is registering the
        // component for us.
        private void OnInfuse(ISimplePool parentPool, IBubbleCounterService bubbleCounter)
        {
            _parentPool = parentPool;
            _bubbleCounter = bubbleCounter;
        }

        private void OnDefuse()
        {
            _parentPool = null;
            _bubbleCounter = null;
        }

        private void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            _velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + Random.Range(2f, 3f);

            if (_bubbleCounter != null)
            {
                _bubbleCounter.AddBubble();
            }
        }

        private void OnDisable()
        {
            if (_bubbleCounter != null)
            {
                _bubbleCounter.RemoveBubble();
            }
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            
            if (Time.time >= _lifeTime)
            {
                // If the parent pool has gone out of scope, just destroy the
                // object instead of recycling it. This isn't a particularly
                // realistic scenario - it's just for demonstration purposes.
                if (_parentPool != null)
                {
                    _parentPool.Recycle(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
