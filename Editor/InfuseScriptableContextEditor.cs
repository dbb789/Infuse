using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Infuse.Editor
{
    [CustomEditor(typeof(InfuseScriptableContext))]
    public class InfuseScriptableContextEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            root.Add(CreateRegisteredTypesFoldout());
            root.Add(CreateRegisteredServicesFoldout());
            root.Add(CreateRegisteredInstancesFoldout());

            return root;
        }
        
        private VisualElement CreateRegisteredServicesFoldout()
        {
            var foldout = new Foldout
            {
                text = "Registered Services"
            };
            
            var list = new VisualElement();

            foldout.Add(list);

            list.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }
                
                var context = (InfuseScriptableContext)target;
                
                list.Clear();

                foreach (var (serviceType, service) in context.ServiceMap.Services)
                {
                    var serviceElement = new InfuseServiceVisualElement();

                    serviceElement.SetContent(serviceType, service);
                    
                    list.Add(serviceElement);
                }
            }).Every(1000);

            return foldout;
        }
                
        private VisualElement CreateRegisteredTypesFoldout()
        {
            var foldout = new Foldout
            {
                text = "Registered Types"
            };
            
            var list = new VisualElement();

            foldout.Add(list);

            list.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }
                
                var context = (InfuseScriptableContext)target;
                
                list.Clear();

                foreach (var type in context.TypeMap.Types)
                {
                    var element = new InfuseTypeInfoVisualElement();

                    element.SetContent(type, context);
                    
                    list.Add(element);
                }
                
            }).Every(1000);

            return foldout;
        }
        
        private VisualElement CreateRegisteredInstancesFoldout()
        {
            var foldout = new Foldout
            {
                text = "Registered Instances"
            };
                
            var list = new VisualElement();
            
            foldout.Add(list);
            
            list.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }
                
                var context = (InfuseScriptableContext)target;
                
                list.Clear();

                foreach (var instanceType in context.InstanceMap.Types)
                {
                    var instanceTypeFoldout = new Foldout
                    {
                        text = InfuseEditorUtil.GetReadableTypeName(instanceType)
                    };
                    
                    var instanceList = new VisualElement();
                    var instances = context.InstanceMap.GetInstanceSet(instanceType);
                    
                    foreach (var instance in instances)
                    {
                        var element = new InfuseInstanceVisualElement();

                        element.SetContent(instance);
                        
                        instanceList.Add(element);
                    }

                    instanceTypeFoldout.Add(instanceList);
                    list.Add(instanceTypeFoldout);
                }
            }).Every(1000);
            
            return foldout;
        }
    }
}
