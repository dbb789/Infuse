using System;
using System.Collections.Generic;

namespace Infuse.Collections
{
    // Tracks the resolved state of InfuseTypeInfo types, which unlike
    // InfuseTypeInfo, is mutable and varies from context to context.
    public class InfuseTypeResolvedMap
    {
        private readonly Dictionary<Type, bool> _map;

        public InfuseTypeResolvedMap()
        {
            _map = new();
        }

        public bool TryGetResolved(InfuseTypeInfo typeInfo, out bool resolved)
        {
            return _map.TryGetValue(typeInfo.InstanceType, out resolved);
        }

        public void SetResolved(InfuseTypeInfo typeInfo, bool resolved)
        {
            _map[typeInfo.InstanceType] = resolved;
        }
    }
}
