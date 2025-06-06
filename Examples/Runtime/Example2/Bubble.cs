using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    // Each Bubble will be added to a common ServiceCollection<Bubble> as a
    // result of calling Infuse() and Defuse() below.
    public class Bubble : MonoBehaviour, InfuseAs<ServiceCollection<Bubble>>
    {
        private SimplePool _parentPool;
        private Vector3 _velocity;
        private float _lifeTime;

        public void Initialize(SimplePool bubblePool)
        {
            _parentPool = bubblePool;
        }
        
        private void OnEnable()
        {
            // We're calling Defuse() ourselves here, so pass false as a second
            // argument to disable automatically calling Defuse() on
            // MonoBehaviour destroy.
            InfuseGlobalContext.Register(this, false);

            transform.localPosition = Vector3.zero;
            _velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + Random.Range(2f, 3f);
        }

        private void OnDisable()
        {
            InfuseGlobalContext.Unregister(this);
        }
        
        private void Update()
        {
            transform.position += _velocity * Time.deltaTime;
            
            if (Time.time >= _lifeTime)
            {
                _parentPool.Recycle(gameObject);
            }
        }
    }
}
