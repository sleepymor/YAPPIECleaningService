using UnityEngine;
using UnityEditor;

namespace Water2D
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Reflector3D))]
    public class ReflectorEditor3D : Editor
    {
        public override void OnInspectorGUI()
        {
            var reflector = target as Reflector3D;
            Inspector(reflector);
            base.OnInspectorGUI();
        }

        void Inspector(Reflector3D reflector)
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            GUILayout.Label("Reflection Adjustments", GUIStyleUtils.Label(14, "87F6FF"));


            reflector.displacement.value = EditorGUILayout.Vector3Field("displacement", reflector.displacement.value);
            foreach (Reflector3D r in targets) r.displacement = reflector.displacement;
            reflector.rotation.value = EditorGUILayout.Vector3Field("additional rotation", reflector.rotation.value);
            foreach (Reflector3D r in targets) r.rotation = reflector.rotation;

            GUILayout.Space(10);
            GUILayout.Label("Pivot", GUIStyleUtils.Label(14, "DAF5FF"));

            


            if (GUILayout.Button("Update Options"))
            {
                foreach (Reflector3D r in targets) { r.UpdateData(); EditorUtility.SetDirty(r); }
            }


            if (GUILayout.Button("CreateReflection"))
            {
                foreach (Reflector3D r in targets) { r.CreateData(); EditorUtility.SetDirty(r); }
            }

            if (GUILayout.Button("DestroyReflection"))
            {
                foreach (Reflector3D r in targets) { r.DeleteData(); EditorUtility.SetDirty(r); }
            }

            GUILayout.Space(10);
            GUILayout.Label("Debug Data", GUIStyleUtils.Label(12, "616163"));

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(reflector);
            }
        }
    }
}
