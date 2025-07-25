using UnityEditor;
using UnityEngine;

namespace Water2D
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ObstructorTilemap))]
    public class ObstructorTilemapEditor : Editor
    { 
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            ObstructorTilemap ob = (ObstructorTilemap)target;

            if (GUILayout.Button("Create")) foreach (ObstructorTilemap obs in targets) { obs.CreateData(); EditorUtility.SetDirty(obs); }
            if (GUILayout.Button("Destroy")) foreach (ObstructorTilemap obs in targets) { obs.Destroy(); EditorUtility.SetDirty(obs); }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(ob);
            }
        }
    }
}