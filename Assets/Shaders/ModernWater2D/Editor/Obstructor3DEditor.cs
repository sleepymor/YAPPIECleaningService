using UnityEditor;
using UnityEngine;

namespace Water2D
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Obstructor3D))]
    public class Obstructor3DEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();


            Obstructor3D ob = (Obstructor3D)target;
          
            float oldH = ob.height.value;
            ob.height.value = EditorGUILayout.Slider("height", ob.height.value, 0f, 1f);
            if(oldH != ob.height.value)
            {
                foreach (Obstructor3D obs in targets) obs.height.value = ob.height.value;
            }

            if (GUILayout.Button("Create")) foreach (Obstructor3D obs in targets) { obs.CreateData(); EditorUtility.SetDirty(obs); }
            if (GUILayout.Button("Destroy")) foreach (Obstructor3D obs in targets) { obs.Destroy(); EditorUtility.SetDirty(obs); }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(ob);
            }

        }

    }

}