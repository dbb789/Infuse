using System;
using Infuse.Collections;

namespace Infuse
{
    public class UpdateEvent : InstanceTraversalEvent, IUpdateEvent, InfuseAs<IUpdateEvent>
    {
        private void Update()
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
