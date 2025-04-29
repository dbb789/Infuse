using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.TypeInfo
{
    public class InfuseTypeInfoCache
    {
        // InfuseTypeInfoMap is populated with InfuseTypeInfo instances that are
        // immutable and are generated purely using static type data via
        // reflection. This means that any two instances of the same type are
        // guaranteed to be identical, and as such we can safely default to a
        // global instance of this class to avoid the costly overhead of
        // regenerating the same InfuseTypeInfo instances over and over again.
        public static InfuseTypeInfoCache GlobalInstance { get; private set; } = new();
        
        public IEnumerable<InfuseTypeInfo> Types => _typeInfoMap?.Values ?? Enumerable.Empty<InfuseTypeInfo>();
        
        private Dictionary<Type, InfuseTypeInfo> _typeInfoMap;
        private Dictionary<Type, List<Type>> _requiresServiceMap;

        public InfuseTypeInfoCache()
        {
            _typeInfoMap = new Dictionary<Type, InfuseTypeInfo>();
            _requiresServiceMap = new Dictionary<Type, List<Type>>();
        }

        public InfuseTypeInfo GetTypeInfo(Type instanceType)
        {
            InfuseTypeInfo typeInfo;
                
            if (!_typeInfoMap.TryGetValue(instanceType, out typeInfo))
            {
                typeInfo = InfuseTypeInfoUtil.CreateInfuseTypeInfo(instanceType);
                Add(typeInfo);
            }

            return typeInfo;
        }
        
        private void Add(InfuseTypeInfo typeInfo)
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
