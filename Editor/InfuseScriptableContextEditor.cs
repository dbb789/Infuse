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
                    
                    instanceElement.Add(new Label(GetReadableTypeName(instanceType)));
                    
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

                    serviceElement.Add(new Label(GetReadableTypeName(serviceType)));
                    serviceList.Add(serviceElement);
                }
            }).Every(1000);

            return root;
        }

        private VisualElement CreateRegisteredTypesFoldout()
        {
            var foldout = new Foldout
            {
                text = "Registered Types"
            };
            
            var list = new ScrollView
            {
                verticalScrollerVisibility = ScrollerVisibility.Auto,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };

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
                    list.Add(typeElementFoldout);
                }
                
            }).Every(1000);

            return foldout;
        }

        private static string GetReadableTypeName(Type type)
        {
            if (type == null)
            {
                return "null";
            }
            
            if (type.IsGenericType)
            {
                var fullName = type.GetGenericTypeDefinition().FullName;
                var genericName = fullName.Substring(0, fullName.IndexOf('`'));
                var types = string.Join(", ", type.GetGenericArguments().Select(GetReadableTypeName));
                
                return $"{genericName}<{types}>";
            }
            else
            {
                return type.FullName;
            }
        }
    }
}
