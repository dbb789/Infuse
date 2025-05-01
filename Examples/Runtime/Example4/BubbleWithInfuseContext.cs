using System;
using UnityEngine;
using Infuse;

namespace Infuse.Examples
{
    public class BubbleWithInfuseContext : MonoBehaviour
    {
        private InfuseContext _context;
        
        private ISimplePool _parentPool;
        private IBubbleCounterService _bubbleCounter;
        private IUpdateEvent _updateEvent;
        
        private Vector3 _velocity;
        private float _lifeTime;

        private Action _updateAction;
        
        public void Initialize(InfuseContext context)
        {
            _context = context;

            // Storing a reference to a member function as an Action saves
            // several unnecessary allocations later.
            _updateAction = UpdateEvent;
        }
        
        private void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            _velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
            _lifeTime = Time.time + UnityEngine.Random.Range(2f, 3f);

            _context.Register(this, false);
        }

        private void OnDisable()
        {
            _context.Unregister(this);
        }
        
        private void OnInfuse(ISimplePool parentPool,
                              IBubbleCounterService bubbleCounter,
                              IUpdateEvent updateEvent)
        {
            _parentPool = parentPool;
            _bubbleCounter = bubbleCounter;
            _updateEvent = updateEvent;

            _bubbleCounter.AddBubble();
            _updateEvent.Add(this, _updateAction);
        }
        
        private void OnDefuse()
        {
            _bubbleCounter.RemoveBubble();
            _updateEvent.Remove(this);
            
            _parentPool = null;
            _bubbleCounter = null;
        }

        private void UpdateEvent()
        {
            transform.position += _velocity * Time.deltaTime;
            
            if (Time.time >= _lifeTime)
            {
                _parentPool.Recycle(gameObject);
            }
        }
    }
}
