using UnityEngine;
using Infuse;

namespace InfuseExample
{
    public class Bubble : MonoBehaviour, InfuseService<InfuseServiceCollection<Bubble>>
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
            transform.position = Vector3.zero;
            _velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + Random.Range(0.5f, 1.5f);

            InfuseManager.Infuse(this, false);
        }

        private void OnDisable()
        {
            InfuseManager.Defuse(this);
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
