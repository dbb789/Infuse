using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Infuse.Collections;
using Infuse.TypeInfo;

namespace Infuse.Editor
{
    [CustomEditor(typeof(InfuseScriptableContext), true)]
    public class InfuseScriptableContextEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);
    
            root.Add(CreateRegisteredTypesFoldout());
            root.Add(CreateRegisteredServicesFoldout());
            root.Add(CreateRegisteredInstancesFoldout());

            return root;
        }
        
        private VisualElement CreateRegisteredTypesFoldout()
        {
            var foldout = new Foldout
            {
                text = "Registered Types"
            };
            
            var listView = new ListView
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };

            var context = (InfuseScriptableContext)target;
            var typeList = new List<InfuseTypeEntry>();
            
            listView.makeItem = () => new InfuseTypeEntryVisualElement();
        
            listView.bindItem = (element, i) =>
            {
                ((InfuseTypeEntryVisualElement)element).SetContent(typeList[i], context);
            };
            
            listView.itemsSource = typeList;
            
            listView.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                typeList.Clear();
                typeList.AddRange(context.TypeEntryMap.Entries.OrderBy(x => x.TypeInfo.InstanceType.FullName));
                listView.RefreshItems();
            }).Every(1000);

            foldout.Add(listView);
            
            return foldout;
        }

        private VisualElement CreateRegisteredServicesFoldout()
        {
            var foldout = new Foldout
            {
                text = "Registered Services"
            };

            var listView = new ListView();
            var context = (InfuseScriptableContext)target;
            var serviceList = new List<KeyValuePair<Type, object>>();
            
            listView.makeItem = () => new InfuseServiceVisualElement();
            
            listView.bindItem = (element, i) =>
            {
                var entry = serviceList.ElementAt(i);
                ((InfuseServiceVisualElement)element).SetContent(entry.Key, entry.Value);
            };
            
            listView.itemsSource = serviceList;

            listView.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                serviceList.Clear();
                serviceList.AddRange(context.ServiceMap.Services.OrderBy(x => x.Key.FullName));
                listView.RefreshItems();
            }).Every(1000);

            foldout.Add(listView);
            
            return foldout;
        }
                
        private VisualElement CreateRegisteredInstancesFoldout()
        {
            var foldout = new Foldout
            {
                text = "Registered Instances"
            };
                
            var listView = new ListView
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };
            
            var context = (InfuseScriptableContext)target;
            var instanceTypeList = new List<Type>();
            
            listView.makeItem = () => new InfuseInstanceListVisualElement();
            
            listView.bindItem = (element, i) =>
            {
                var instanceType = instanceTypeList[i];

                if (context.InstanceMap.TryGetInstanceSet(instanceType, out var instanceSet))
                {
                    ((InfuseInstanceListVisualElement)element).SetContent(instanceType, instanceSet.Instances.OrderBy(x => $"{x}"));
                }
            };
            
            listView.itemsSource = instanceTypeList;

            listView.schedule.Execute(() =>
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                instanceTypeList.Clear();
                instanceTypeList.AddRange(context.InstanceMap.Types.OrderBy(x => x.Name));
                listView.RefreshItems();
            }).Every(1000);
            
            foldout.Add(listView);

            return foldout;
        }
    }
}
