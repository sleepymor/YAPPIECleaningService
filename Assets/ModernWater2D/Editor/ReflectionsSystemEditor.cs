using UnityEngine;
using UnityEditor;

namespace Water2D
{


    [CustomEditor(typeof(ReflectionsSystem))]
    public class ReflectionsSystemEditor : Editor
    {

        GUILayoutOption[] def_options = new GUILayoutOption[2];

        private string temp_layer;
        private string temp_tag;

        private bool Options;
        private bool ReflectionOptions;
        private bool PivotOptions;
        private bool SceneOptions;
        private bool AddingReflectors;
        private bool Advanced;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ReflectionsSystem ref_cam = (ReflectionsSystem)target;

            GUILayout.Space(20);
            Options = EditorGUILayout.Foldout(Options, "Includes", GUIStyleUtils.DropDown(16) );
            if (Options)
            {
                if (ref_cam.overrideMainCamera.value = EditorGUILayout.Toggle("override main camera", ref_cam.overrideMainCamera.value))
                {
                    ref_cam.mainCamera = (Camera)EditorGUILayout.ObjectField("main camera rendering scene", ref_cam.mainCamera, typeof(Camera), true);
                }
                else ref_cam.mainCamera = Camera.main;

                ref_cam.reflectorMat = (Material)EditorGUILayout.ObjectField("material for reflected sprites", ref_cam.reflectorMat, typeof(Material), true);
                ref_cam.reflectionMat = (Material)EditorGUILayout.ObjectField("material for reflection surfaces", ref_cam.reflectionMat, typeof(Material), true);
            }

            GUILayout.Space(20);
            ReflectionOptions = EditorGUILayout.Foldout(ReflectionOptions, "Reflection Options", GUIStyleUtils.DropDown(16));
            if (ReflectionOptions)
            {
               ref_cam.textureResolution.value = EditorGUILayout.Slider("resolution of reflections", ref_cam.textureResolution.value, 0, 1);
               ref_cam.reflectionsSettings.originalColor.value = EditorGUILayout.Slider("original color alpha", ref_cam.reflectionsSettings.originalColor.value, 0, 1);
               ref_cam.reflectionsSettings.color.value = EditorGUILayout.ColorField("Color",ref_cam.reflectionsSettings.color.value );
               ref_cam.reflectionsSettings.alpha.value = EditorGUILayout.Slider("Alpha", ref_cam.reflectionsSettings.alpha.value,0,1f);
               ref_cam.reflectionsSettings.angle.value = EditorGUILayout.Slider("Angle", ref_cam.reflectionsSettings.angle.value, -90f, 90f);
               ref_cam.reflectionsSettings.tilt.value= EditorGUILayout.Slider("Tilt", ref_cam.reflectionsSettings.tilt.value,0f,90f);
            }

            GUIStyle gUIStyle = new GUIStyle(GUI.skin.toggle);
            gUIStyle.alignment = TextAnchor.MiddleLeft;
            gUIStyle.fontStyle = FontStyle.Bold;
            gUIStyle.fontSize = 14;

            GUILayout.Space(20);
            PivotOptions = EditorGUILayout.Foldout(PivotOptions, "Pivot Options", GUIStyleUtils.DropDown(16));
            if (PivotOptions)
            {
                ref_cam.pivotDetectionAlphaTreshold = EditorGUILayout.IntField("pivot-creation-alpha-threshold", ref_cam.pivotDetectionAlphaTreshold);
                ref_cam.defaultReflectionSprflipx.value = GUILayout.Toggle(ref_cam.defaultReflectionSprflipx.value, "reflections default x orientation");
            }

            GUILayout.Space(20);
            SceneOptions = EditorGUILayout.Foldout(SceneOptions, "Scene Options", GUIStyleUtils.DropDown(16));
            if (SceneOptions)
            {
                ref_cam.reflectionObjectsVisible.value = GUILayout.Toggle(ref_cam.reflectionObjectsVisible.value, "reflections visible in Hierarchy");
                ref_cam.cameraVisible.value = GUILayout.Toggle(ref_cam.cameraVisible.value, "camera visible in Inspector");
            }

            GUILayout.Space(20);
            AddingReflectors = EditorGUILayout.Foldout(AddingReflectors, "Adding Reflectors", GUIStyleUtils.DropDown(16));
            if (AddingReflectors)
            {


                temp_tag = GUILayout.TextField(temp_tag);
                if (GUILayout.Button("Turn every object with this tag into reflector"))
                {
                    var a = GameObject.FindGameObjectsWithTag(temp_tag);
                    foreach (var b in a) if (b.GetComponent<Reflector>() == null) b.AddComponent<Reflector>();
                }
                if (GUILayout.Button("Undo every object with this tag"))
                {
                    var a = GameObject.FindGameObjectsWithTag(temp_tag);
                    foreach (var b in a) if (b.GetComponent<Reflector>() != null) DestroyImmediate(b.GetComponent<Reflector>());
                }

            }

            GUILayout.Space(20);

        }
    }

}