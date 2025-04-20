using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infuse
{
    public class InfuseTypeMap
    {
        private Dictionary<Type, InfuseType> _typeMap;
        private Dictionary<Type, List<InfuseType>> _byRequires;
        private Dictionary<Type, List<InfuseType>> _byProvides;

        public InfuseTypeMap()
        {
            _typeMap = new Dictionary<Type, InfuseType>();
            _byRequires = new Dictionary<Type, List<InfuseType>>();
            _byProvides = new Dictionary<Type, List<InfuseType>>();
        }
        
        public InfuseType GetInfuseType(Type type)
        {
            InfuseType infuseType;
            
            if (!_typeMap.TryGetValue(type, out infuseType))
            {
                infuseType = InfuseUtil.CreateInfuseType(type);

                Debug.Log($"InfuseType created for {type}: {infuseType}");
                
                _typeMap.Add(type, infuseType);

                foreach (var requiredType in infuseType.Requires)
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

                foreach (var providedType in infuseType.Provides)
                {
                    if (_byProvides.TryGetValue(providedType, out var infuseTypes))
                    {
                        infuseTypes.Add(infuseType);
                    }
                    else
                    {
                        _byProvides.Add(providedType, new List<InfuseType> { infuseType });
                    }
                }
            }

            return infuseType;
        }

        public List<InfuseType> GetRequiresInfuseType(Type type)
        {
            if (_byRequires.TryGetValue(type, out var infuseTypes))
            {
                return infuseTypes;
            }
            else
            {
                return new List<InfuseType>();
            }
        }

        public List<InfuseType> GetProvidesInfuseType(Type type)
        {
            if (_byProvides.TryGetValue(type, out var infuseTypes))
            {
                return infuseTypes;
            }
            else
            {
                return new List<InfuseType>();
            }
        }
    }
}
