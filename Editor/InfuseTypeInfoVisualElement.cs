using UnityEngine;
using UnityEngine.UIElements;
using Infuse.Collections;

namespace Infuse.Editor
{
    public class InfuseTypeInfoVisualElement : VisualElement
    {
        private Label _label;
        private VisualElement _requiredServicesList;
        private VisualElement _providedServicesList;
        
        public InfuseTypeInfoVisualElement()
        {
            var root = new VisualElement();
            
            _label = new Label();

            _label.style.unityFontStyleAndWeight = FontStyle.Bold;
            
            root.Add(_label);

            _requiredServicesList = new Box();
            _providedServicesList = new Box();
            
            root.Add(_requiredServicesList);
            root.Add(_providedServicesList);

            var spacer = new VisualElement();

            spacer.style.height = 10;

            root.Add(spacer);
            
            Add(root);
        }

        public void SetContent(InfuseTypeInfo typeInfo, InfuseScriptableContext context)
        {
            int instanceCount = context.InstanceMap.GetInstanceSet(typeInfo.InstanceType).Count;
            
            _label.text = $"{InfuseEditorUtil.GetReadableTypeName(typeInfo.InstanceType)} ({instanceCount})";
            _label.style.color = typeInfo.Resolved ? Color.green : Color.red;
            
            _requiredServicesList.Clear();
            
            foreach (var service in typeInfo.RequiredServices)
            {
                var serviceLabel = new Label($"Requires : {InfuseEditorUtil.GetReadableTypeName(service)}");
                bool hasService = context.ServiceMap.Contains(service);

                serviceLabel.style.color = hasService ? Color.green : Color.red;
                
                _requiredServicesList.Add(serviceLabel);
            }

            _providedServicesList.Clear();
            
            foreach (var service in typeInfo.ProvidedServices)
            {
                var serviceLabel = new Label($"Provides : {InfuseEditorUtil.GetReadableTypeName(service)}");

                serviceLabel.style.color = (instanceCount > 0)  ? Color.green : Color.red;
                
                _providedServicesList.Add(serviceLabel);
            }
        }
    }
}
