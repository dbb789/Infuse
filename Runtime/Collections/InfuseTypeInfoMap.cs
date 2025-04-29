using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class InfuseTypeInfoMap : IDisposable
    {
        // InfuseTypeInfoMap is populated with InfuseTypeInfo instances that are
        // immutable and are generated purely using static type data via
        // reflection. This means that any two instances of the same type are
        // guaranteed to be identical, and as such we can safely default to a
        // global instance of this class to avoid the costly overhead of
        // regenerating the same InfuseTypeInfo instances over and over again.
        public static InfuseTypeInfoMap GlobalInstance { get; private set; } = new InfuseTypeInfoMap();
        
        public IEnumerable<InfuseTypeInfo> Types => _typeInfoMap?.Values ?? Enumerable.Empty<InfuseTypeInfo>();
        
        private Dictionary<Type, InfuseTypeInfo> _typeInfoMap;
        private Dictionary<Type, List<Type>> _requiresServiceMap;

        public InfuseTypeInfoMap()
        {
            _typeInfoMap = new Dictionary<Type, InfuseTypeInfo>();
            _requiresServiceMap = new Dictionary<Type, List<Type>>();
        }

        public void Dispose()
        {
            _typeInfoMap.Clear();
            _requiresServiceMap.Clear();
        }

        public void Add(InfuseTypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            var instanceType = typeInfo.InstanceType;

            if (_typeInfoMap.ContainsKey(instanceType))
            {
                Debug.LogError($"InfuseTypeInfo for {typeInfo.InstanceType} already exists.");
                return;
            }

            _typeInfoMap.Add(instanceType, typeInfo);

            foreach (var requiredService in typeInfo.RequiredServices)
            {
                if (_requiresServiceMap.TryGetValue(requiredService, out var typeInfoList))
                {
                    typeInfoList.Add(instanceType);
                }
                else
                {
                    _requiresServiceMap.Add(requiredService, new List<Type> { instanceType });
                }
            }
        }
        
        public bool TryGetType(Type instanceType, out InfuseTypeInfo typeInfo)
        {
            return _typeInfoMap.TryGetValue(instanceType, out typeInfo);
        }

        public List<Type> GetTypesRequiringService(Type instanceType)
        {
            if (_requiresServiceMap.TryGetValue(instanceType, out var infuseTypes))
            {
                return infuseTypes;
            }

            var list = new List<Type>();
            
            _requiresServiceMap.Add(instanceType, list);
            
            return list;
        }
    }
}
