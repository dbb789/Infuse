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

            var typeListFoldout = new Foldout
            {
                text = "Registered Types"
            };
            
            var typeList = new ScrollView
            {
                verticalScrollerVisibility = ScrollerVisibility.Auto,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };

            typeListFoldout.Add(typeList);
            root.Add(typeListFoldout);

            typeList.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }
                
                var context = (InfuseScriptableContext)target;
                
                typeList.Clear();

                foreach (var type in context.TypeMap.Types)
                {
                    var typeElementFoldout = new Foldout
                    {
                        text = type.InstanceType.ToString()
                    };

                    var typeElementBox = new Box();

                    typeElementBox.Add(new Toggle
                    {
                        label = "Resolved",
                        value = type.Resolved,
                        enabledSelf = false
                    });


                    foreach (var providedService in type.ProvidedServices)
                    {
                        var serviceLabel = new Label($"Provides : {providedService}");
                        typeElementBox.Add(serviceLabel);
                    }

                    typeElementFoldout.Add(typeElementBox);
                    typeList.Add(typeElementFoldout);
                }
                
            }).Every(1000);
            
            var instanceListFoldout = new Foldout
            {
                text = "Registered Instances"
            };

            var instanceList = new ScrollView
            {
                verticalScrollerVisibility = ScrollerVisibility.Auto,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };

            instanceListFoldout.Add(instanceList);
            root.Add(instanceListFoldout);

            instanceList.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                var context = (InfuseScriptableContext)target;

                instanceList.Clear();

                foreach (var instanceType in context.InstanceMap.Types)
                {
                    var instanceElement = new VisualElement();
                    
                    instanceElement.Add(new Label(instanceType.Name));
                    
                    var instances = context.InstanceMap.GetInstanceSet(instanceType);
                    
                    foreach (var instance in instances)
                    {
                        if (instance is UnityEngine.Object unityObject)
                        {
                            var objectField = new ObjectField
                            {
                                label = "Object",
                                objectType = instance.GetType(),
                                value = unityObject
                            };

                            instanceElement.Add(objectField);
                        }
                        else
                        {
                            var instanceLabel = new Label($"{instance}");

                            instanceElement.Add(instanceLabel);
                        }
                    }

                    instanceList.Add(instanceElement);
                }
            }).Every(1000);
            
            var serviceListFoldout = new Foldout
            {
                text = "Registered Services"
            };

            var serviceList = new ScrollView
            {
                verticalScrollerVisibility = ScrollerVisibility.Auto,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };

            serviceListFoldout.Add(serviceList);
            root.Add(serviceListFoldout);

            serviceList.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                var context = (InfuseScriptableContext)target;

                serviceList.Clear();

                foreach (var serviceType in context.ServiceMap.Types)
                {
                    var serviceElement = new VisualElement();

                    serviceElement.Add(new Label($"{serviceType}"));
                    serviceList.Add(serviceElement);
                }
            }).Every(1000);

            return root;
        }
    }
}
