using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseTypeMap
    {
        private Dictionary<Type, InfuseType> _typeMap;
        private Dictionary<Type, List<InfuseType>> _byRequires;

        public InfuseTypeMap()
        {
            _typeMap = new Dictionary<Type, InfuseType>();
            _byRequires = new Dictionary<Type, List<InfuseType>>();
        }
        
        public InfuseType GetInfuseType(Type type)
        {
            InfuseType infuseType;
            
            if (!_typeMap.TryGetValue(type, out infuseType))
            {
                infuseType = InfuseTypeUtil.CreateInfuseType(type);

                Debug.Log($"InfuseType created for {type}: {infuseType}");
                
                _typeMap.Add(type, infuseType);

                foreach (var requiredType in infuseType.RequiredServices)
                {
                    if (_byRequires.TryGetValue(requiredType, out var infuseTypes))
                    {
                        infuseTypes.Add(infuseType);
                    }
                    else
                    {
                        _byRequires.Add(requiredType, new List<InfuseType> { infuseType });
                    }
                }
            }

            return infuseType;
        }

        public List<InfuseType> GetTypesRequiringService(Type type)
        {
            if (_byRequires.TryGetValue(type, out var infuseTypes))
            {
                return infuseTypes;
            }

            var list = new List<InfuseType>();
            
            _byRequires.Add(type, list);
            
            return list;
        }
    }
}
