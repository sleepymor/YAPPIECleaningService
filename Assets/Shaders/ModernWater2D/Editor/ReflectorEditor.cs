using UnityEngine;
using UnityEditor;

namespace Water2D
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Reflector))]
    public class ReflectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var reflector = target as Reflector;
            Inspector(reflector);
            base.OnInspectorGUI();
        }

        void Inspector(Reflector reflector) 
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            GUILayout.Label("Reflection Adjustments", GUIStyleUtils.Label(14, "87F6FF"));

            reflector.flipX.value = EditorGUILayout.Toggle("flip reflection x-axis", reflector.flipX.value);
            foreach (Reflector r in targets) r.flipX = reflector.flipX;

            reflector.displacement.value = EditorGUILayout.Vector2Field("displacement", reflector.displacement.value);
            foreach (Reflector r in targets) r.displacement = reflector.displacement;

            reflector.additionalTilt.value = EditorGUILayout.Slider("additional tilt",reflector.additionalTilt.value, -90f, 90f);
            foreach (Reflector r in targets) r.additionalTilt = reflector.additionalTilt;

            GUILayout.Label("Raymarched Reflection", GUIStyleUtils.Label(14, "87F6FF"));

            bool oldR = reflector.raymarched;
            reflector.raymarched = EditorGUILayout.Toggle("raymarch",reflector.raymarched);
            if (oldR != reflector.raymarched) foreach (Reflector r in targets) { r.raymarched = reflector.raymarched; r.CreateData(); }

            int oldM = reflector.maxLength;
            reflector.maxLength = EditorGUILayout.IntField("max pixel length", reflector.maxLength);
            if (oldM != reflector.maxLength) foreach (Reflector r in targets) { r.maxLength = reflector.maxLength; r.CreateData(); }


            //reflector.MSP_ReflectionGenerator.value = EditorGUILayout.Toggle("MSP Reflector Generator", reflector.MSP_ReflectionGenerator.value);

            GUILayout.Space(10);
            GUILayout.Label("Pivot", GUIStyleUtils.Label(14, "DAF5FF"));

            reflector.pivotSourceMode = (ReflectionPivotSourceMode)EditorGUILayout.EnumPopup("reflection pivot source mode:", reflector.pivotSourceMode);
            foreach (Reflector r in targets) r.pivotSourceMode = reflector.pivotSourceMode;

            if (reflector.pivotSourceMode == ReflectionPivotSourceMode.custom_transform)
            {
                reflector.customPivot = (Transform)EditorGUILayout.ObjectField(reflector.customPivot, typeof(Transform), true);
                foreach (Reflector r in targets)  r.customPivot = reflector.customPivot;
              
            }

            GUILayout.Space(10);
            GUILayout.Label("Control", GUIStyleUtils.Label(14, "FFBFA0"));

            if (GUILayout.Button("Update Options"))
            {
                foreach (Reflector r in targets) { r.UpdateData(); EditorUtility.SetDirty(r); }
            }
            

            if (GUILayout.Button("CreateReflection"))
            {
                foreach (Reflector r in targets) {r.CreateData();  EditorUtility.SetDirty(r); }
}

            if (GUILayout.Button("DestroyReflection"))
            {
                foreach (Reflector r in targets) {r.DeleteData();   EditorUtility.SetDirty(r); }
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
