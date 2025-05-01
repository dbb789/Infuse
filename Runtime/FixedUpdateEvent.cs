using System;
using Infuse.Collections;

namespace Infuse
{
    public class FixedUpdateEvent : InstanceTraversalEvent, IFixedUpdateEvent, InfuseAs<IFixedUpdateEvent>
    {
        private void FixedUpdate()
        {
            if (InstanceTraversalList == null)
            {
                return;
            }
            
            InstanceTraversalList.ApplyUpdates();
            
            foreach (var action in InstanceTraversalList)
            {
                action?.Invoke();
            }
        }
    }
}
