using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class BubbleSpawner : MonoBehaviour
    {
        private InfuseServiceStack<BubblePool> _bubblePool;
        private float _nextSpawnTime;
        
        private void Awake()
        {
            InfuseManager.Infuse(this);
        }

        // Ensures that Update() will never be called when BubblePool isn't
        // available.
        private void OnEnable()
        {
            if (_bubblePool == null)
            {
                enabled = false;
            }
        }

        public void OnInfuse(InfuseServiceStack<BubblePool> bubblePool)
        {
            Debug.Log("BubbleSpawner.OnInfuse()", gameObject);
            
            _bubblePool = bubblePool;
            _nextSpawnTime = Time.time;
            enabled = true;
        }

        private void OnDefuse()
        {
            Debug.Log("BubbleSpawner.OnDefuse()", gameObject);
            
            _bubblePool = null;
            enabled = false;
        }

        private void Update()
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
