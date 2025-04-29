using System;
using System.Collections.Generic;
using System.Linq;

namespace Infuse.TypeInfo
{
    public class InfuseTypeEntryMap : IDisposable
    {
        public IEnumerable<InfuseTypeEntry> Entries => _map.Values;

        private readonly Dictionary<Type, InfuseTypeEntry> _map;
        
        public InfuseTypeEntryMap()
        {
            _map = new();
        }

        public void Dispose()
        {
            _map.Clear();
        }
        
        public void Add(InfuseTypeEntry entry)
        {
            _map.Add(entry.TypeInfo.InstanceType, entry);
        }
        
        public bool TryGetTypeEntry(Type type, out InfuseTypeEntry entry)
        {
            return _map.TryGetValue(type, out entry);
        } 
    }
}
