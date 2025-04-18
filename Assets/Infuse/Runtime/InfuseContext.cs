using System;
using System.Collections.Generic;

namespace Infuse
{
    public class InfuseContext
    {
        private InfuseTypeMap _typeMap;
        private InfuseInstanceMap _resolvedMap;
        private InfuseInstanceMap _unresolvedMap;
        
        public InfuseContext()
        {
            _typeMap = new();
            _resolvedMap = new();
            _unresolvedMap = new();
        }

        public void Infuse(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var type = instance.GetType();
            var infuseType = _typeMap.GetInfuseType(type);

            if (IsResolved(infuseType))
            {
                infuseType.Infuse(instance, _resolvedMap);
                
                if (_resolvedMap.Add(type, instance) == 1)
                {
                    OnResolved(type);
                }
            }
            else
            {
                _unresolvedMap.Add(type, instance);
            }
        }

        private void OnResolved(Type type)
        {
            foreach (var dependentType in _typeMap.GetRequiresInfuseType(type))
            {
                if (IsResolved(dependentType))
                {
                    var unresolvedList = _unresolvedMap.GetInstanceList(dependentType.Type);

                    if (unresolvedList.Count > 0)
                    {
                        foreach (var unresolved in unresolvedList)
                        {
                            dependentType.Infuse(unresolved, _resolvedMap);
                        }
                        
                        InfuseInstanceMap.Move(dependentType.Type, _unresolvedMap, _resolvedMap);

                        OnResolved(dependentType.Type);
                    }
                }
            }
        }
        
        private bool IsResolved(InfuseType infuseType)
        {
            foreach (var requiredType in infuseType.Requires)
            {
                if (!_resolvedMap.Contains(requiredType))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
