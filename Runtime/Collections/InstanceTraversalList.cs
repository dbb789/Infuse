using System;
using System.Collections.Generic;
using Infuse.TypeInfo;

namespace Infuse.Collections
{
    public class InstanceTraversalList<TData>
    {
        public struct Enumerator
        {
            private readonly InstanceTraversalList<TData> _list;
            private List<Type>.Enumerator _typeEnumerator;
            private Dictionary<object, TData>.Enumerator _instanceEnumerator;
            private bool _initial;

            public Enumerator(InstanceTraversalList<TData> list)
            {
                _list = list;
                _typeEnumerator = _list._typeList.GetEnumerator();
                _instanceEnumerator = default;
                _initial = true;
            }

            public TData Current => _instanceEnumerator.Current.Value;

            public bool MoveNext()
            {
                if (!_initial && _instanceEnumerator.MoveNext())
                {
                    return true;
                }

                _initial = false;

                if (_typeEnumerator.MoveNext())
                {
                    var type = _typeEnumerator.Current;
                    var entry = _list._instanceMap[type];

                    _instanceEnumerator = entry.GetEnumerator();

                    return _instanceEnumerator.MoveNext();
                }

                return false;
            }
        }

        private struct UpdateOperation
        {
            public enum OperationType
            {
                Add,
                Remove
            }

            public static UpdateOperation CreateAdd(object instance, TData data)
            {
                return new UpdateOperation(OperationType.Add, instance, data);
            }

            public static UpdateOperation CreateRemove(object instance)
            {
                return new UpdateOperation(OperationType.Remove, instance, default);
            }
            
            public OperationType Type { get; }
            public object Instance { get; }
            public TData Data { get; }

            private UpdateOperation(OperationType type, object instance, TData data)
            {
                Type = type;
                Instance = instance;
                Data = data;
            }
        }
        
        private readonly InfuseTypeInfoCache _typeInfoCache;
        private readonly Dictionary<Type, Dictionary<object, TData>> _instanceMap;
        private readonly List<Type> _typeList;
        private readonly List<UpdateOperation> _updateOperationList;
        
        public InstanceTraversalList(InfuseTypeInfoCache typeInfoCache)
        {
            _typeInfoCache = typeInfoCache;
            _instanceMap = new(4);
            _typeList = new(4);
            _updateOperationList = new(4);
        }

        public void Add(object instance, TData data)
        {
            _updateOperationList.Add(UpdateOperation.CreateAdd(instance, data));
        }

        public void Remove(object instance)
        {
            _updateOperationList.Add(UpdateOperation.CreateRemove(instance));
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void ApplyUpdates()
        {
            bool typeAdded = false;
            
            foreach (var operation in _updateOperationList)
            {
                switch (operation.Type)
                {
                case UpdateOperation.OperationType.Add:
                    typeAdded |= PerformAdd(operation.Instance, operation.Data);
                    break;
                    
                case UpdateOperation.OperationType.Remove:
                    PerformRemove(operation.Instance);
                    break;
                }
            }
            
            _updateOperationList.Clear();
            
            if (typeAdded)
            {
                UpdateTypeList();
            }
        }
        
        private void UpdateTypeList()
        {
            var visitSet = new HashSet<Type>();

            _typeList.Clear();
            
            foreach (var entry in _instanceMap)
            {
                AddDependenciesOf(entry.Key, visitSet);
            }
        }
        
        private bool PerformAdd(object instance, TData data)
        {
            bool typeAdded = false;
            var instanceType = instance.GetType();

            if (_instanceMap.TryGetValue(instanceType, out var entry))
            {
                entry.Add(instance, data);
            }
            else
            {
                _instanceMap.Add(instanceType,
                                 new Dictionary<object, TData> { { instance, data } });
                typeAdded = true;
            }

            return typeAdded;
        }

        private void PerformRemove(object instance)
        {
            var instanceType = instance.GetType();

            if (_instanceMap.TryGetValue(instanceType, out var entry))
            {
                entry.Remove(instance);
            }
        }

        private void AddDependenciesOf(Type type,
                                       HashSet<Type> visitSet)
        {
            if (visitSet.Contains(type))
            {
                return;
            }

            visitSet.Add(type);
            
            var typeInfo = _typeInfoCache.GetTypeInfo(type);

            foreach (var requiredType in typeInfo.RequiredServices)
            {
                AddDependenciesOf(requiredType, visitSet);
            }

            if (_instanceMap.ContainsKey(type))
            {
                _typeList.Add(type);
            }
        }
    }
}
