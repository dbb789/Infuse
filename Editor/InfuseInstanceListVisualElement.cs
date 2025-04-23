using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Infuse.Editor
{
    public class InfuseInstanceListVisualElement : VisualElement
    {
        private VisualElement _root;

        public InfuseInstanceListVisualElement()
        {
            _root = new VisualElement();

            Add(_root);
        }

        public void SetContent(Type type, IEnumerable<object> instanceList)
        {
            _root.Clear();
            
            var label = new Label
            {
                text = InfuseEditorUtil.GetReadableTypeName(type)
            };

            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            
            _root.Add(label);

            foreach (var instance in instanceList)
            {
                var instanceElement = new InfuseInstanceVisualElement();
                instanceElement.SetContent(instance);
                
                _root.Add(instanceElement);
            }

            var spacer = new VisualElement();
            
            spacer.style.height = 10;
            
            _root.Add(spacer);
        }
    }
}
