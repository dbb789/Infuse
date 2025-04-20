using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseTypeMap
    {
        private Dictionary<Type, InfuseType> _typeMap;
        private Dictionary<Type, List<InfuseType>> _requiresServiceMap;

        public InfuseTypeMap()
        {
            _typeMap = new Dictionary<Type, InfuseType>();
            _requiresServiceMap = new Dictionary<Type, List<InfuseType>>();
        }
        
        public InfuseType GetInfuseType(Type type)
        {
            InfuseType infuseType;
            
            if (!_typeMap.TryGetValue(type, out infuseType))
            {
                infuseType = InfuseTypeUtil.CreateInfuseType(type);
                
                _typeMap.Add(type, infuseType);

                foreach (var requiredService in infuseType.RequiredServices)
                {
                    if (_requiresServiceMap.TryGetValue(requiredService, out var infuseTypes))
                    {
                        infuseTypes.Add(infuseType);
                    }
                    else
                    {
                        _requiresServiceMap.Add(requiredService, new List<InfuseType> { infuseType });
                    }
                }
            }

            return infuseType;
        }

        public List<InfuseType> GetTypesRequiringService(Type type)
        {
            if (_requiresServiceMap.TryGetValue(type, out var infuseTypes))
            {
                return infuseTypes;
            }

            var list = new List<InfuseType>();
            
            _requiresServiceMap.Add(type, list);
            
            return list;
        }
    }
}
