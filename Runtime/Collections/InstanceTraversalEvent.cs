using System;
using UnityEngine;
using Infuse.TypeInfo;
using Infuse.Common;

namespace Infuse.Collections
{
    public abstract class InstanceTraversalEvent : MonoBehaviour
    {
        [SerializeField]
        private InfuseScriptableContext _context;

        protected InstanceTraversalList<Action> InstanceTraversalList;
        
        private void Awake()
        {
            _context.Register(this);
        }
        
        private void OnInfuse(InfuseTypeInfoCache typeInfoCache)
        {
            InstanceTraversalList = new InstanceTraversalList<Action>(typeInfoCache);
        }

        private void OnDefuse()
        {
            InstanceTraversalList = null;
        }
        
        public void Add(object instance, Action updateFunc)
        {
            if (InstanceTraversalList == null)
            {
                throw new InfuseException("UpdateEvent is not infused.");
            }

            InstanceTraversalList.Add(instance, updateFunc);
        }

        public void Remove(object instance)
        {
            if (InstanceTraversalList == null)
            {
                throw new InfuseException("UpdateEvent is not infused.");
            }

            InstanceTraversalList.Remove(instance);
        }
    }
}
