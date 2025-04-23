using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Infuse.Editor
{
    public class InfuseInstanceVisualElement : VisualElement
    {
        private VisualElement _root;

        public InfuseInstanceVisualElement()
        {
            _root = new VisualElement();

            Add(_root);
        }

        public void SetContent(object instance)
        {
            _root.Clear();

            if (instance is UnityEngine.Object unityObject)
            {
                var objectField = new ObjectField
                {
                    label = "",
                    objectType = instance.GetType(),
                    value = unityObject
                };

                _root.Add(objectField);
            }
            else
            {
                var textField = new TextField
                {
                    label = "",
                    value = instance.ToString()
                };

                _root.Add(textField);
            }
        }
    }
}
