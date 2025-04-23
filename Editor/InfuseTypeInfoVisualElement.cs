using UnityEngine;
using UnityEngine.UIElements;
using Infuse.Collections;

namespace Infuse.Editor
{
    public class InfuseTypeInfoVisualElement : VisualElement
    {
        private Foldout _foldout;
        private VisualElement _requiredServicesList;
        private VisualElement _providedServicesList;
        
        public InfuseTypeInfoVisualElement()
        {
            var root = new VisualElement();
            
            _foldout = new Foldout();
            
            _requiredServicesList = new Box();
            _providedServicesList = new Box();
            
            _foldout.Add(_requiredServicesList);
            _foldout.Add(_providedServicesList);

            root.Add(_foldout);
            Add(root);
        }

        public void SetContent(InfuseTypeInfo typeInfo, InfuseScriptableContext context)
        {
            _foldout.text = InfuseEditorUtil.GetReadableTypeName(typeInfo.InstanceType);
            
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
                
                _providedServicesList.Add(serviceLabel);
            }
        }
    }
}
