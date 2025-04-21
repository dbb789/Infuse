using UnityEngine;
using UnityEditor;

namespace Infuse.Editor
{
    [CustomEditor(typeof(InfuseContext))]
    public class InfuseContextEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
