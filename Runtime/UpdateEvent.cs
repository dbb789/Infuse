using System;
using UnityEngine;
using Infuse.Collections;
using Infuse.TypeInfo;
using Infuse.Common;

namespace Infuse
{
    public class UpdateEvent : MonoBehaviour, IUpdateEvent, InfuseAs<IUpdateEvent>
    {
        [SerializeField]
        private InfuseScriptableContext _context;

        private InstanceTraversalList<Action> _instanceTraversalList;
        
        private void Awake()
        {
            _context.Register(this);
        }
        
        private void OnInfuse(InfuseTypeInfoCache typeInfoCache)
        {
            _instanceTraversalList = new InstanceTraversalList<Action>(typeInfoCache);
        }

        private void OnDefuse()
        {
            _instanceTraversalList = null;
        }
        
        public void Add(object instance, Action updateFunc)
        {
            if (_instanceTraversalList == null)
            {
                throw new InfuseException("UpdateEvent is not infused.");
            }

            _instanceTraversalList.Add(instance, updateFunc);
        }

        public void Remove(object instance)
        {
            if (_instanceTraversalList == null)
            {
                throw new InfuseException("UpdateEvent is not infused.");
            }

            _instanceTraversalList.Remove(instance);
        }

        private void Update()
        {
            if (_instanceTraversalList == null)
            {
                return;
            }
            
            _instanceTraversalList.ApplyUpdates();
            
            foreach (var action in _instanceTraversalList)
            {
                action?.Invoke();
            }
        }
    }
}
