using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class BubbleSpawner : MonoBehaviour
    {
        private ServiceStack<ISimplePool> _bubblePool;
        private IUpdateEvent _updateEvent;
        private float _nextSpawnTime;
        
        private void Awake()
        {
            InfuseGlobalContext.Register(this);
        }

        public void OnInfuse(ServiceStack<ISimplePool> bubblePool, IUpdateEvent updateEvent)
        {
            Debug.Log("BubbleSpawner.OnInfuse()", gameObject);
            
            _bubblePool = bubblePool;
            _updateEvent = updateEvent;
            _nextSpawnTime = Time.time;

            _updateEvent.Add(this, UpdateEvent);
        }

        private void OnDefuse()
        {
            Debug.Log("BubbleSpawner.OnDefuse()", gameObject);

            _updateEvent.Remove(this);
            
            _bubblePool = null;
            _updateEvent = null;
        }

        private void UpdateEvent()
        {
            if (Time.time < _nextSpawnTime)
            {
                return;
            }
            
            _nextSpawnTime = Time.time + Random.Range(0.25f, 0.5f);
            _bubblePool.Current.Get();
        }
    }
}
