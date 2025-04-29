using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class BubbleWithInfuseContext : MonoBehaviour
    {
        private InfuseContext _context;
        
        private ISimplePool _parentPool;
        private IBubbleCounterService _bubbleCounter;
        
        private Vector3 _velocity;
        private float _lifeTime;

        public void Initialize(InfuseContext context)
        {
            _context = context;

            if (enabled)
            {
                _context.Register(this, false);
            }
        }

        private void OnEnable()
        {
            _context?.Register(this, false);
        }
        
        private void OnDisable()
        {
            _context?.Unregister(this);
        }

        // Note here that in this case, the parent pool is registering the
        // component for us.
        private void OnInfuse(ISimplePool parentPool, IBubbleCounterService bubbleCounter)
        {
            _parentPool = parentPool;
            _bubbleCounter = bubbleCounter;

            transform.localPosition = Vector3.zero;
            _velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + Random.Range(2f, 3f);
            _bubbleCounter.AddBubble();
        }

        private void OnDefuse()
        {
            _bubbleCounter.RemoveBubble();
            _parentPool = null;
            _bubbleCounter = null;
        }

        private void Update()
        {
            if (_parentPool == null)
            {
                return;
            }
            
            transform.position += _velocity * Time.deltaTime;
            
            if (Time.time >= _lifeTime)
            {
                _parentPool.Recycle(gameObject);
            }
        }
    }
}
