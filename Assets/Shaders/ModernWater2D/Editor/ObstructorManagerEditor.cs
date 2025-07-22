using UnityEditor;
using UnityEngine;

namespace Water2D
{
    [CustomEditor(typeof(ObstructorManager))]
    public class ObstructorManagerEditor : Editor
    {
        bool Options;
        bool SceneOptions;
        bool Graphics;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ObstructorManager obsm = (ObstructorManager)target;

            GUILayout.Space(20);
            Options = EditorGUILayout.Foldout(Options, "Includes", GUIStyleUtils.DropDown(16));
            if (Options)
            {
                if (obsm.overrideMainCamera.value = EditorGUILayout.Toggle("override main camera", obsm.overrideMainCamera.value))
                {
                    obsm.mainCamera = (Camera)EditorGUILayout.ObjectField("main camera rendering scene", obsm.mainCamera, typeof(Camera), true);
                }
            }

            GUILayout.Space(20);
            Graphics = EditorGUILayout.Foldout(Graphics, "Graphics", GUIStyleUtils.DropDown(16));
            if (Graphics)
            {
                obsm._textureResolution.value = EditorGUILayout.Slider("resolution of reflections", obsm._textureResolution.value, 0, 1);
            }

            GUILayout.Space(20);
            SceneOptions = EditorGUILayout.Foldout(SceneOptions, "Scene Options", GUIStyleUtils.DropDown(16));
            if (SceneOptions)
            {
                obsm.obstructionObjectsVisible.value = GUILayout.Toggle(obsm.obstructionObjectsVisible.value, "obstructors visible in Hierarchy");
                obsm.cameraVisible.value = GUILayout.Toggle(obsm.cameraVisible.value, "camera visible in Inspector");
            }

            GUILayout.Space(20);
        }
    }
}