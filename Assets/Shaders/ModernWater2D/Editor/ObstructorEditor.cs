using UnityEditor;
using UnityEngine;

namespace Water2D
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Obstructor))]
    public class ObstructorEditor : Editor
    {
        SpriteRenderer sr;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();


            Obstructor ob = (Obstructor)target;
            float oldV = ob.height;
            ob.height = EditorGUILayout.Slider(ob.height, 0, 1);
            if (oldV != ob.height) foreach (Obstructor obs in targets) obs.height = ob.height;



            if (GUILayout.Button("Create")) foreach (Obstructor obs in targets) { obs.CreateData(); EditorUtility.SetDirty(obs); }
            if (GUILayout.Button("Destroy")) foreach (Obstructor obs in targets) { obs.Destroy(); EditorUtility.SetDirty(obs); }

            if (sr == null) sr = ob.transform.GetComponent<SpriteRenderer>();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(ob);
            }

        }

    }
}