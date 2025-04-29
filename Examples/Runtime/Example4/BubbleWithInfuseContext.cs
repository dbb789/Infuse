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

        private void Awake()
        {
            enabled = false;
        }
        
        // Note here that in this case, the parent pool is registering the
        // component for us.
        private void OnInfuse(ISimplePool parentPool, IBubbleCounterService bubbleCounter)
        {
            _parentPool = parentPool;
            _bubbleCounter = bubbleCounter;
            enabled = true;
        }

        private void OnDefuse()
        {
            enabled = false;
            _parentPool = null;
            _bubbleCounter = null;
        }

        private void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            _velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + Random.Range(2f, 3f);
            _bubbleCounter.AddBubble();
        }

        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            
            if (Time.time >= _lifeTime)
            {
                _bubbleCounter.RemoveBubble();
                _parentPool.Recycle(gameObject);
            }
        }
    }
}
