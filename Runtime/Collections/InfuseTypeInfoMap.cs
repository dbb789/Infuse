using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infuse.Collections
{
    public class InfuseTypeInfoMap : IDisposable
    {
        public IEnumerable<InfuseTypeInfo> Types => _typeInfoMap?.Values ?? Enumerable.Empty<InfuseTypeInfo>();
        
        private Dictionary<Type, InfuseTypeInfo> _typeInfoMap;
        private Dictionary<Type, List<InfuseTypeInfo>> _requiresServiceMap;

        public InfuseTypeInfoMap()
        {
            _typeInfoMap = new Dictionary<Type, InfuseTypeInfo>();
            _requiresServiceMap = new Dictionary<Type, List<InfuseTypeInfo>>();
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

            if (_typeInfoMap.ContainsKey(typeInfo.InstanceType))
            {
                Debug.LogError($"InfuseTypeInfo for {typeInfo.InstanceType} already exists.");
                return;
            }

            _typeInfoMap.Add(typeInfo.InstanceType, typeInfo);

            foreach (var requiredService in typeInfo.RequiredServices)
            {
                if (_requiresServiceMap.TryGetValue(requiredService, out var typeInfoList))
                {
                    typeInfoList.Add(typeInfo);
                }
                else
                {
                    _requiresServiceMap.Add(requiredService, new List<InfuseTypeInfo> { typeInfo });
                }
            }
        }
        
        public bool TryGetType(Type instanceType, out InfuseTypeInfo typeInfo)
        {
            return _typeInfoMap.TryGetValue(instanceType, out typeInfo);
        }

        public List<InfuseTypeInfo> GetTypesRequiringService(Type instanceType)
        {
            if (_requiresServiceMap.TryGetValue(instanceType, out var infuseTypes))
            {
                return infuseTypes;
            }

            var list = new List<InfuseTypeInfo>();
            
            _requiresServiceMap.Add(instanceType, list);
            
            return list;
        }
    }
}
