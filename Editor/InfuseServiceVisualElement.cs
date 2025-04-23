using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Infuse.Editor
{
    public class InfuseServiceVisualElement : VisualElement
    {
        private VisualElement _root;

        public InfuseServiceVisualElement()
        {
            _root = new VisualElement();

            Add(_root);
        }

        public void SetContent(Type serviceType, object service)
        {
            _root.Clear();

            if (service is UnityEngine.Object unityObject)
            {
                var objectField = new ObjectField
                {
                    label = InfuseEditorUtil.GetReadableTypeName(serviceType),
                    objectType = service.GetType(),
                    value = unityObject
                };

                _root.Add(objectField);
            }
            else
            {
                var serviceLabel = new Label($"{InfuseEditorUtil.GetReadableTypeName(serviceType)} : {service}");
                
                _root.Add(serviceLabel);
            }
        }
    }
}
