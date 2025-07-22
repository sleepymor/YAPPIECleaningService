using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Water2D
{

    [InitializeOnLoad]
    [CustomEditor(typeof(ModernWater2D))]
    public class ModernWater2DEditor : Editor
    {

        #region variables


        ModernWater2D water;

        private const int space1W = 20;
        private const int space2W = 14;
        private const int space3W = 8;

        private const int dropDown1W = 14;
        private const int dropDown2W = 12;
        private const int dropDown3W = 10;

        private static readonly Color bannerBackgroundColor = new Color(0.14f, 0.14f, 0.14f);
        private static readonly Color backgroundColor = new Color(0.18f, 0.18f, 0.18f);

        private static readonly Color sector1 = new Color(0.22f, 0.22f, 0.22f);
        private static readonly Color sector2 = new Color(0.18f, 0.25f, 0.18f);
        private static readonly Color sector3= new Color(0.18f, 0.25f, 0.25f);
        private static readonly Color sector4= new Color(0.4f, 0.20f, 0.25f);

        private AnimBool[] animBs = new AnimBool[64];
        public AnimBool GetAnimBs(int idx) { if (animBs[idx] == null) animBs[idx] = new AnimBool(); return animBs[idx]; }
        [SerializeField] private bool FancyEditor = false;
        [SerializeField] private TextureUtils.ResolutionEnum resEnum;

        #endregion

        #region utils


        Rect rect;
        Rect vrect;
        private void Banner() 
        {
            rect = GUILayoutUtility.GetRect(1, 1);
            vrect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(new Rect(rect.x - 13, rect.y - 1, rect.width + 17, vrect.height + 9), bannerBackgroundColor);

            GUIStyle style = new GUIStyle();
            style.stretchWidth = true;
            style.stretchHeight = true;
            style.alignment = TextAnchor.MiddleCenter;

            GUILayout.Space(10);
            GUILayout.Label(Resources.Load<Texture>("Sprites/editor/banner"), style);
            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
        }

        private void StartVB(Color bg)
        {
            rect = GUILayoutUtility.GetRect(1, 1);
            vrect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(new Rect(rect.x - 13, rect.y - 1, rect.width + 17, vrect.height + 9), bg);
        }

        private void EndVB() { EditorGUILayout.EndVertical(); }

        [SerializeField] List<int> layers = new List<int>();

        #endregion

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();


            base.OnInspectorGUI();
            water = (ModernWater2D)(target);

            Banner();

            //background color
            rect = GUILayoutUtility.GetRect(1, 1);
            vrect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(new Rect(rect.x - 13, rect.y - 1, rect.width + 17, vrect.height + 9), backgroundColor);

            //type = Foldout(type, "type", GUIStyleUtils.DropDown(dropDown1W), space1W);
            //if (type) TypeSettings();

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor,GetAnimBs(0), out var shouldDraw, "looks"))
            {
                if (shouldDraw) LooksSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(11), out var shouldDraw, "blurs"))
            {
                if (shouldDraw) blursSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(14), out var shouldDraw, "wet surface"))
            {
                if (shouldDraw) WetSurfaceSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(1), out var shouldDraw, "reflections"))
            {
                if (shouldDraw) reflectionsSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(2), out var shouldDraw, "obstructions"))
            {
                if (shouldDraw) obstructionSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(3), out var shouldDraw, "simulations"))
            {
                if (shouldDraw) simulationSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(15), out var shouldDraw, "surface waves"))
            {
                if (shouldDraw) surfaceWavesSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(4), out var shouldDraw, "editor"))
            {
                if (shouldDraw) editorSettings();
            }

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(5), out var shouldDraw, "utils"))
            {
                if (shouldDraw) utilsSettings();
            }

            EditorGUILayout.EndVertical();
           
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(water);
            }
        }

        [SerializeField][HideInInspector] private bool singleColor = false;
        [SerializeField][HideInInspector] private bool lightingWhenBlur = false;

        private void LooksSettings() 
        {

         

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor,GetAnimBs(6), out var shouldDraw, "alpha and tiling"))
            {
                if (shouldDraw)
                {
                    StartVB(sector1);
                    water.settings._waterSettings.baseAlpha.value = EditorGUILayout.Slider("base alpha", water.settings._waterSettings.baseAlpha.value, 0f, 1f, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.alphaTexture = (Texture2D)EditorGUILayout.ObjectField("alpha texture", water.settings._waterSettings.alphaTexture, typeof(Texture2D), true ,GUILayout.ExpandWidth(true));

                    EditorGUILayout.Space(5);

                    if (!water.settings._blurSettings.useBlur.value)
                    {
                        water.lightingWhenBlur = water.settings._waterSettings._useLighting.value = EditorGUILayout.Toggle("recieve URP Lighting", water.settings._waterSettings._useLighting.value, GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        water.settings._waterSettings._useLighting.value = EditorGUILayout.Toggle("recieve URP Lighting", water.settings._waterSettings._useLighting.value, GUILayout.ExpandWidth(true));
                        water.lightingWhenBlur=water.settings._waterSettings._useLighting.value;
                    }

                    EditorGUILayout.Space(5);

                    water.settings._waterSettings.tiling.value = EditorGUILayout.Vector2Field("tiling", water.settings._waterSettings.tiling.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.numOfPixels.value = EditorGUILayout.IntField("number of pixels", water.settings._waterSettings.numOfPixels.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.pixelPerfect.value = EditorGUILayout.Toggle("pixel perfect", water.settings._waterSettings.pixelPerfect.value, GUILayout.ExpandWidth(true));
                    water.overrideMainCamera.value = EditorGUILayout.Toggle("override main camera (with camera that renders the scene)", water.overrideMainCamera.value, GUILayout.ExpandWidth(true));

                    Camera c1 = water.cameraOverride;
                    if (water.overrideMainCamera.value)
                    {
                        water.cameraOverride = (Camera)EditorGUILayout.ObjectField(water.cameraOverride, typeof(Camera), true, GUILayout.ExpandWidth(true));
                    }
                    if (water.cameraOverride != c1) water.overrideMainCamera.onValueChanged.Invoke();

                    EndVB();
                }
            }


            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(26), out var shouldDraw, "coloring"))
            {
                if (shouldDraw)
                {
                    StartVB(sector1);
                    water.settings._waterSettings.coloringType = (ColoringType)EditorGUILayout.EnumPopup("coloringType", water.settings._waterSettings.coloringType);
                    if (water.settings._waterSettings.coloringType == ColoringType.distance_from_obstructors)
                    {
                        water.settings._waterSettings.depthMlp.value = EditorGUILayout.Slider("depthMlp", water.settings._waterSettings.depthMlp.value, 0f, 32f);
                        water.settings._waterSettings.colorGradient.value = EditorGUILayout.GradientField("depthColorGradient", water.settings._waterSettings.colorGradient.value);
                    }
                    else if (water.settings._waterSettings.coloringType == ColoringType.single_color)
                    {
                        water.settings._waterSettings.color.value = EditorGUILayout.ColorField("edges color", water.settings._waterSettings.color.value, GUILayout.ExpandWidth(true));
                        water.settings._waterSettings.depthColor.value = water.settings._waterSettings.color.value;
                    }
                    else if (water.settings._waterSettings.coloringType == ColoringType.two_colors)
                    {
                        water.settings._waterSettings.color.value = EditorGUILayout.ColorField("edges color", water.settings._waterSettings.color.value, GUILayout.ExpandWidth(true));
                        water.settings._waterSettings.depthColor.value = EditorGUILayout.ColorField("depth color", water.settings._waterSettings.depthColor.value, GUILayout.ExpandWidth(true));
                    }
                    else if (water.settings._waterSettings.coloringType == ColoringType.depthY)
                    {
                        water.settings._waterSettings.color.value = EditorGUILayout.ColorField("edges color", water.settings._waterSettings.color.value, GUILayout.ExpandWidth(true));
                        water.settings._waterSettings.depthColor.value = EditorGUILayout.ColorField("depth color", water.settings._waterSettings.depthColor.value, GUILayout.ExpandWidth(true));
                    }
                        EndVB();
                }
            }


            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(8), out var shouldDraw, "strips and foam"))
            {
                if (shouldDraw)
                {
          
                    StartVB(sector1);
                    water.settings._waterSettings.foamSpeed.value = EditorGUILayout.Vector2Field("speed", water.settings._waterSettings.foamSpeed.value, GUILayout.ExpandWidth(true));

                    water.settings._waterSettings.foamColor.value = EditorGUILayout.ColorField("foam color", water.settings._waterSettings.foamColor.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.foamSize.value = EditorGUILayout.FloatField("foam size", water.settings._waterSettings.foamSize.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.foamAlpha.value = EditorGUILayout.Slider("foam alpha", water.settings._waterSettings.foamAlpha.value, 0f, 1f, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.foamDensity.value = EditorGUILayout.Slider("foam density", water.settings._waterSettings.foamDensity.value, 0f, 1f, GUILayout.ExpandWidth(true));

                    EditorGUILayout.Space(10);

                    water.settings._waterSettings.sunStripsTexture = (Texture2D)EditorGUILayout.ObjectField("strips texture",water.settings._waterSettings.sunStripsTexture, typeof(Texture2D), true, GUILayout.ExpandWidth(true));
                    EditorGUILayout.Space(5);
                    water.settings._waterSettings.stripsAlpha.value = EditorGUILayout.Slider("strips alpha", water.settings._waterSettings.stripsAlpha.value, 0f, 1f, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.stripsSize.value = EditorGUILayout.FloatField("strips size", water.settings._waterSettings.stripsSize.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.stripsSpeed.value = EditorGUILayout.FloatField("strips speed", water.settings._waterSettings.stripsSpeed.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.stripsDensity.value = EditorGUILayout.FloatField("strips density", water.settings._waterSettings.stripsDensity.value, GUILayout.ExpandWidth(true));
                    EndVB();
                }
            }


            

            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(9), out var shouldDraw, "distortion"))
            {
                if (shouldDraw)
                {
                    StartVB(sector1);

                    var c = water.settings._waterSettings.distortionTexture;
                    water.settings._waterSettings.distortionTexture = (Texture2D)EditorGUILayout.ObjectField(water.settings._waterSettings.distortionTexture, typeof(Texture2D), true, GUILayout.ExpandWidth(true));
                    if (c != water.settings._waterSettings.surfaceTexture) water.settings._waterSettings.surfaceSpeed.onValueChanged.Invoke();

                    EditorGUILayout.Space(5);
                    water.settings._waterSettings.distortionSpeed.value = EditorGUILayout.Vector2Field("speed", water.settings._waterSettings.distortionSpeed.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.distortionStrength.value = EditorGUILayout.Vector2Field("strength", water.settings._waterSettings.distortionStrength.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.distortionTiling.value = EditorGUILayout.Vector2Field("tiling", water.settings._waterSettings.distortionTiling.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.distortionMinMax.value = EditorGUILayout.Vector2Field("min max color strength", water.settings._waterSettings.distortionMinMax.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.distortionColor.value = EditorGUILayout.ColorField("color of distortion waves", water.settings._waterSettings.distortionColor.value, GUILayout.ExpandWidth(true));
                    EndVB();
                }
            }


            using (new WaterLayoutUtils.FoldoutScope(FancyEditor, GetAnimBs(10), out var shouldDraw, "surface texture"))
            {
                if (shouldDraw)
                {
                    StartVB(sector1);
                    var c = water.settings._waterSettings.surfaceTexture;
                    water.settings._waterSettings.surfaceSprite = (SpriteRenderer)EditorGUILayout.ObjectField("surface sprite",water.settings._waterSettings.surfaceSprite, typeof(SpriteRenderer), true, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.surfaceTexture = (Texture2D)EditorGUILayout.ObjectField("surface texture",water.settings._waterSettings.surfaceTexture, typeof(Texture2D), true, GUILayout.ExpandWidth(true));
                    if (c != water.settings._waterSettings.surfaceTexture) water.settings._waterSettings.surfaceSpeed.onValueChanged.Invoke();

                    EditorGUILayout.Space(5);
                    water.settings._waterSettings.surfaceTiling.value = EditorGUILayout.Vector2Field("tiling", water.settings._waterSettings.surfaceTiling.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.surfaceSpeed.value = EditorGUILayout.Vector2Field("speed", water.settings._waterSettings.surfaceSpeed.value, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.surfaceAlpha.value = EditorGUILayout.Slider("alpha", water.settings._waterSettings.surfaceAlpha.value,0f,1f, GUILayout.ExpandWidth(true));
                    water.settings._waterSettings.useFoamSpeed.value = EditorGUILayout.Toggle("use foam speed as surface speed", water.settings._waterSettings.useFoamSpeed.value , GUILayout.ExpandWidth(true));

                    EndVB();
                }
            }

        }

        private void reflectionsSettings()
        {

            StartVB(sector1); water.enableReflections.value = GUILayout.Toggle(water.enableReflections.value, "enable"); EndVB();
            StartVB(sector2); water.settings._reflectionsSettings.enableTopDownReflections.value = GUILayout.Toggle(water.settings._reflectionsSettings.enableTopDownReflections.value, "enable top down reflections"); EndVB();
            StartVB(sector3); water.settings._reflectionsSettings.enablePlatformerReflections.value = GUILayout.Toggle(water.settings._reflectionsSettings.enablePlatformerReflections.value, "enable platformer reflections"); EndVB();
            StartVB(sector4); water.settings._reflectionsSettings.enableRaymarchedReflections.value = GUILayout.Toggle(water.settings._reflectionsSettings.enableRaymarchedReflections.value, "enable raymarched reflections"); EndVB();

            StartVB(sector1);
            water.settings._reflectionsSettings.textureResolution.value = EditorGUILayout.Slider("resolution", water.settings._reflectionsSettings.textureResolution.value, 0f, 1f, GUILayout.ExpandWidth(true));

            EditorGUILayout.Space(5);
            water.settings._reflectionsSettings.originalColor.value = EditorGUILayout.Slider("original color", water.settings._reflectionsSettings.originalColor.value, 0f, 1, GUILayout.ExpandWidth(true));
            water.settings._reflectionsSettings.alpha.value = EditorGUILayout.Slider("alpha", water.settings._reflectionsSettings.alpha.value, 0f, 1f, GUILayout.ExpandWidth(true));
            water.settings._reflectionsSettings.color.value = EditorGUILayout.ColorField("color", water.settings._reflectionsSettings.color.value, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(5);
            water.settings._reflectionsSettings.DistortionFPRH.value = EditorGUILayout.Toggle("Distortion FPRH", water.settings._reflectionsSettings.DistortionFPRH.value, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space(5);
            EndVB();

            if (water.settings._reflectionsSettings.enableTopDownReflections.value)
            {
                StartVB(sector2);
                water.settings._reflectionsSettings.defaultReflectionSprflipx.value = EditorGUILayout.Toggle("default reflection x-orientation", water.settings._reflectionsSettings.defaultReflectionSprflipx.value, GUILayout.ExpandWidth(true));
                water.settings._reflectionsSettings.angle.value = EditorGUILayout.Slider("angle", water.settings._reflectionsSettings.angle.value, -90, 90, GUILayout.ExpandWidth(true));
                water.settings._reflectionsSettings.tilt.value = EditorGUILayout.Slider("tilt", water.settings._reflectionsSettings.tilt.value, 0, 90, GUILayout.ExpandWidth(true));
                EditorGUILayout.Space(10);
                EndVB();
            }


            if (water.settings._reflectionsSettings.enablePlatformerReflections.value)
            {
                StartVB(sector3);
                water.settings._reflectionsSettings.customReflectionStart.value = EditorGUILayout.Toggle("custom reflections starting point", water.settings._reflectionsSettings.customReflectionStart.value, GUILayout.ExpandWidth(true));
                if (water.settings._reflectionsSettings.customReflectionStart.value) water.settings._reflectionsSettings.mirrorY.value = EditorGUILayout.Slider("reflections starting point", water.settings._reflectionsSettings.mirrorY.value, 0f, 5f, GUILayout.ExpandWidth(true));


                EditorGUILayout.Space(5);

                water.settings._reflectionsSettings.enableScrolling.value = EditorGUILayout.Toggle("enable infinite scrolling", water.settings._reflectionsSettings.enableScrolling.value, GUILayout.ExpandWidth(true));
                if (water.settings._reflectionsSettings.enableScrolling.value)
                {
                    water.settings._reflectionsSettings.playerPosition = (Transform)EditorGUILayout.ObjectField(water.settings._reflectionsSettings.playerPosition, typeof(Transform), true);
                    water.settings._reflectionsSettings.scrollingStrength.value = EditorGUILayout.Slider("scrolling strength (useful for parallax)", water.settings._reflectionsSettings.scrollingStrength.value, 0f, 10f, GUILayout.ExpandWidth(true));
                    EditorGUILayout.Space(5);
                }

                var list = water.settings._reflectionsSettings.layers;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("layers to render size", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(0);

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = EditorGUILayout.IntField("layer index : ", list[i]);
                }

                water.settings._reflectionsSettings.layers = list;
                water.settings._reflectionsSettings.usePerspective.value = EditorGUILayout.Toggle("use perspective", water.settings._reflectionsSettings.usePerspective.value, GUILayout.ExpandWidth(true));
                if (water.settings._reflectionsSettings.usePerspective.value)
                {
                    water.settings._reflectionsSettings.waterPerspective.value = EditorGUILayout.Vector2Field("water perspective", water.settings._reflectionsSettings.waterPerspective.value, GUILayout.ExpandWidth(true));
                    water.settings._reflectionsSettings.reflectionsPerspective.value = EditorGUILayout.Vector2Field("reflections perspective", water.settings._reflectionsSettings.reflectionsPerspective.value, GUILayout.ExpandWidth(true));
                }
                EditorGUILayout.Space(5);
                water.settings._reflectionsSettings.enableFalloff.value = EditorGUILayout.Toggle("enable falloff", water.settings._reflectionsSettings.enableFalloff.value, GUILayout.ExpandWidth(true));
                if (water.settings._reflectionsSettings.enableFalloff.value)
                {
                    water.settings._reflectionsSettings.falloffStart.value = EditorGUILayout.Slider("falloff start", water.settings._reflectionsSettings.falloffStart.value, 0f, 1f, GUILayout.ExpandWidth(true));
                    water.settings._reflectionsSettings.falloffStrength.value = EditorGUILayout.Slider("falloff strength", water.settings._reflectionsSettings.falloffStrength.value, 0f, 10f, GUILayout.ExpandWidth(true));
                }
                EditorGUILayout.Space(10);
                EndVB();
            }



            if (water.settings._reflectionsSettings.enableRaymarchedReflections.value)
            {
                StartVB(sector4);
                water.raymarchUnits = EditorGUILayout.FloatField("raymarch units", water.raymarchUnits);
                float worldH = Screen.height;
                float partOfW = (float)(water.raymarchUnits) /(water.cameraOverride ? water.cameraOverride.orthographicSize*2f : Camera.main.orthographicSize*2f );
                int pixels = (int)Mathf.Min(worldH*partOfW, 256f);

                water.settings._reflectionsSettings.raymarchSteps.value = pixels;
                water.settings._reflectionsSettings.raymarchFalloffStart.value = EditorGUILayout.Slider("falloff start", water.settings._reflectionsSettings.raymarchFalloffStart.value, 0f, 1f);
                water.settings._reflectionsSettings.raymarchFalloffEnd.value = EditorGUILayout.Slider("falloff end", water.settings._reflectionsSettings.raymarchFalloffEnd.value, water.settings._reflectionsSettings.raymarchFalloffStart.value, 1f);
              
                var list = water.settings._reflectionsSettings.raymarchlayers;
                int newCount = Mathf.Max(0, EditorGUILayout.IntField("layers to render size", list.Count));
                while (newCount < list.Count)
                    list.RemoveAt(list.Count - 1);
                while (newCount > list.Count)
                    list.Add(0);

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = EditorGUILayout.IntField("layer index : ", list[i]);
                }
                water.settings._reflectionsSettings.raymarchlayers = list;

                water.settings._reflectionsSettings.type2.value = EditorGUILayout.Toggle("type2 raymarching", water.settings._reflectionsSettings.type2.value);
                EndVB();
            }
        }

        private void blursSettings() 
        {
            StartVB(sector1);

            water.settings._blurSettings.useBlur.onValueChanged = water.OnBlurMaterialChanged;

            var b1 = water.settings._blurSettings.useBlur.value;
            water.settings._blurSettings.useBlur.value = EditorGUILayout.Toggle("enable", water.settings._blurSettings.useBlur.value);


            if (b1 != water.settings._blurSettings.useBlur.value) lightingWhenBlur = !water.settings._waterSettings._useLighting.value;

            if (water.settings._blurSettings.useFalloff.value = EditorGUILayout.Toggle("enable falloff",water.settings._blurSettings.useFalloff.value))
            {
                water.settings._blurSettings.falloffStart.value = EditorGUILayout.Slider("blur falloff start", water.settings._blurSettings.falloffStart.value, 0f, 1f);
                water.settings._blurSettings.falloffEnd.value = EditorGUILayout.Slider("blur falloff end", water.settings._blurSettings.falloffEnd.value, Mathf.Min(0f, water.settings._blurSettings.falloffStart.value), 1f);
                water.settings._blurSettings.falloffStrength.value = EditorGUILayout.Slider("blur falloff strength", water.settings._blurSettings.falloffStrength.value, 0f,3f);
            }

            BlurSettings.BlurType old = water.settings._blurSettings.blurType;
            water.settings._blurSettings.blurType = (BlurSettings.BlurType)EditorGUILayout.EnumPopup("type of blur",water.settings._blurSettings.blurType);
            if (old != water.settings._blurSettings.blurType) water.OnBlurMaterialChanged();

            switch (water.settings._blurSettings.blurType)
            {
                case BlurSettings.BlurType.box:
                    water.settings._blurSettings.boxSamplingRange.value = EditorGUILayout.IntSlider("sampling area" , water.settings._blurSettings.boxSamplingRange.value, 1, 32);
                    water.settings._blurSettings.boxStrength.value = EditorGUILayout.Slider("strength", water.settings._blurSettings.boxStrength.value,0f,1f);

                    break;
                case BlurSettings.BlurType.gaussian:
                    water.settings._blurSettings.gaussianSamplingRange.value = EditorGUILayout.IntSlider("sampling area", water.settings._blurSettings.gaussianSamplingRange.value, 1, 32);
                    water.settings._blurSettings.gaussianStrengthX.value = EditorGUILayout.FloatField("strength", water.settings._blurSettings.gaussianStrengthX.value);

                    break;
                case BlurSettings.BlurType.bokeh:
                    water.settings._blurSettings.bokehArea.value = EditorGUILayout.Slider("sampling area", water.settings._blurSettings.bokehArea.value,0f,0.01f);
                    water.settings._blurSettings.bokehQuality.value = EditorGUILayout.IntSlider("sampling quality", water.settings._blurSettings.bokehQuality.value, 1, 32);

                    water.settings._blurSettings.bokehGamma.value = EditorGUILayout.Slider("gamma", water.settings._blurSettings.bokehGamma.value, 1f, 32f);
                    water.settings._blurSettings.bokehHardness.value = EditorGUILayout.Slider("hardness", water.settings._blurSettings.bokehHardness.value, 0f, 1f);
                    water.settings._blurSettings.bokehRatio.value = EditorGUILayout.FloatField("x-y ratio", water.settings._blurSettings.bokehRatio.value);

                    break;
            }
            EndVB();
        }

        private void obstructionSettings()
        {
            StartVB(sector1);
            water.enableObstruction.value = GUILayout.Toggle(water.enableObstruction.value,"enable");
            water.settings._obstructorSettings.textureResolution.value = EditorGUILayout.Slider("resolution", water.settings._obstructorSettings.textureResolution.value, 0f, 1f, GUILayout.ExpandWidth(true));

            water.settings._waterSettings.obstructionAlpha.value = EditorGUILayout.Slider("alpha", water.settings._waterSettings.obstructionAlpha.value, 0f, 1f, GUILayout.ExpandWidth(true));
            water.settings._waterSettings.obstructionColor.value = EditorGUILayout.ColorField("color", water.settings._waterSettings.obstructionColor.value, GUILayout.ExpandWidth(true));
            water.settings._waterSettings.obstructionWidth.value = EditorGUILayout.FloatField("width", water.settings._waterSettings.obstructionWidth.value, GUILayout.ExpandWidth(true));

            EndVB();
        }

        private void simulationSettings()
        {
            StartVB(sector1);
            water.enableSimulation.value = GUILayout.Toggle(water.enableSimulation.value, "enable", GUILayout.ExpandWidth(true));
            if (water.enableSimulation.value && !water.enableObstruction.value) water.enableObstruction.value = true;

            var sim = water._waterSimulationType;
            water._waterSimulationType = (SimulationType)EditorGUILayout.EnumPopup("Simulation type :", water._waterSimulationType);
            if (water._waterSimulationType != sim) water.settings._simulationSettings.rainSpeed.onValueChanged.Invoke();
            if (water._waterSimulationType == SimulationType.advanced)
            {
                WaterSimulationAdvanced wsim = water.waterSimulation as WaterSimulationAdvanced;
            }


            EditorGUILayout.Space(10);

            if (water._waterSimulationType == SimulationType.advanced)
            {
                EditorGUILayout.LabelField("max simulation size : 2048 (4 194 304 cells)");
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("for 3 iterations :");
                EditorGUILayout.LabelField("high-end pc -> 2048");
                EditorGUILayout.LabelField("mid pc -> 1024");
                EditorGUILayout.LabelField("old pc -> 512");
                EditorGUILayout.LabelField("mid-mobiles -> 256");
            }
            else
            {
                EditorGUILayout.LabelField("max simulation size : 4096 (16 777 216 cells)");
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("for 3 iterations :");
                EditorGUILayout.LabelField("high-end pc -> 4096");
                EditorGUILayout.LabelField("mid pc -> 2048");
                EditorGUILayout.LabelField("old pc -> 1024");
                EditorGUILayout.LabelField("mid-mobiles -> 512");
            }

            EditorGUILayout.Space(10);

            int x;
            resEnum = TextureUtils.ToPowerOf2(water.settings._simulationSettings.resolution.value.x);
            resEnum = (TextureUtils.ResolutionEnum)EditorGUILayout.EnumPopup("resolution (x)", resEnum, GUILayout.ExpandWidth(true));
            if (water._waterSimulationType == SimulationType.advanced && resEnum == TextureUtils.ResolutionEnum._4096) resEnum = TextureUtils.ResolutionEnum._2048;
            x = resEnum.ToInt();

            water.settings._simulationSettings.resolution.value = new Vector2Int(x, Mathf.FloorToInt(x * 1 / (water.sr.bounds.size.x / water.sr.bounds.size.y)));
            EditorGUILayout.Space(10);

            water.settings._simulationSettings.normalStrength.value = EditorGUILayout.Slider("normals strength", water.settings._simulationSettings.normalStrength.value, 0f, 5f, GUILayout.ExpandWidth(true));
            water.settings._simulationSettings.waveColor.value = EditorGUILayout.ColorField("wave color", water.settings._simulationSettings.waveColor.value, GUILayout.ExpandWidth(true));
            water.settings._simulationSettings.waveColorMinMaxHeight.value = EditorGUILayout.Vector2Field("wave color minimum and max height", water.settings._simulationSettings.waveColorMinMaxHeight.value, GUILayout.ExpandWidth(true));

            EditorGUILayout.Space(10);

            water.settings._simulationSettings.waveHeight.value = EditorGUILayout.FloatField("wave height", water.settings._simulationSettings.waveHeight.value, GUILayout.ExpandWidth(true));
            water.settings._simulationSettings.dispersion.value = EditorGUILayout.Slider("water dispersion", water.settings._simulationSettings.dispersion.value,0.75f,1f, GUILayout.ExpandWidth(true));

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("iterations will increase the cost of simulation linearly, 3 recommended");
            water.settings._simulationSettings.iterations.value = EditorGUILayout.IntSlider("iterations", water.settings._simulationSettings.iterations.value, 1,16, GUILayout.ExpandWidth(true));

            EditorGUILayout.Space(5);
            water.settings._simulationSettings.enableRain.value = EditorGUILayout.Toggle("enable rain effect", water.settings._simulationSettings.enableRain.value);
            if (water.settings._simulationSettings.enableRain.value)
            {
                water.settings._simulationSettings.rainSpeed.value = EditorGUILayout.FloatField("rain speed", water.settings._simulationSettings.rainSpeed.value);
                water.settings._simulationSettings.rainWaveHeight.value = EditorGUILayout.FloatField("rain strength", water.settings._simulationSettings.rainWaveHeight.value);

                water.settings._simulationSettings.rainSizeX.value = EditorGUILayout.IntSlider("rain size X", water.settings._simulationSettings.rainSizeX.value,1,4);
                water.settings._simulationSettings.rainSizeY.value = EditorGUILayout.IntSlider("rain size Y", water.settings._simulationSettings.rainSizeY.value,1,4);

            }
            EndVB();

        }

        private void surfaceWavesSettings() 
        {
            StartVB(sector1);

            water.enableWavesSimulation.value = EditorGUILayout.Toggle("enable waves", water.enableWavesSimulation.value);
            water.settings._wavesSettings.wavePoints.value = EditorGUILayout.IntField("wave points", water.settings._wavesSettings.wavePoints.value);
            water.settings._wavesSettings.simulationSteps.value = EditorGUILayout.IntField("simulation steps", water.settings._wavesSettings.simulationSteps.value);
            EditorGUILayout.Space(5);
            water.settings._wavesSettings.waveHeight.value = EditorGUILayout.Slider("height", water.settings._wavesSettings.waveHeight.value, 0f, 0.5f);
            water.settings._wavesSettings.stringDampening.value = EditorGUILayout.Slider("dampening", water.settings._wavesSettings.stringDampening.value, 0.0f, 1f);
            water.settings._wavesSettings.stringSpread.value = EditorGUILayout.Slider("spread", water.settings._wavesSettings.stringSpread.value, 0f, 0.1f);
            water.settings._wavesSettings.stringStiffness.value = EditorGUILayout.Slider("stiffness", water.settings._wavesSettings.stringStiffness.value, 0f, 0.5f);
            EditorGUILayout.Space(5);
            water.settings._wavesSettings.splashForceMin.value = EditorGUILayout.FloatField("min kinetic force needed to splash", water.settings._wavesSettings.splashForceMin.value);
            water.settings._wavesSettings.splashForceMax.value = EditorGUILayout.FloatField("max kinetic force needed to splash", water.settings._wavesSettings.splashForceMax.value);
            water.settings._wavesSettings.splashVelMin.value = EditorGUILayout.FloatField("min velocity of splash waves", water.settings._wavesSettings.splashVelMin.value);
            water.settings._wavesSettings.splashVelMax.value = EditorGUILayout.FloatField("max velocity of splash waves", water.settings._wavesSettings.splashVelMax.value);
            water.settings._wavesSettings.splashNodesWidthMin.value = EditorGUILayout.IntSlider("min width of created splash", water.settings._wavesSettings.splashNodesWidthMin.value,1, water.settings._wavesSettings.wavePoints.value/4);
            water.settings._wavesSettings.splashNodesWidthMax.value = EditorGUILayout.IntSlider("max width of created splash", water.settings._wavesSettings.splashNodesWidthMax.value, water.settings._wavesSettings.splashNodesWidthMin.value, water.settings._wavesSettings.wavePoints.value/4);
            EditorGUILayout.Space(10);
            water.settings._wavesSettings.edgeColor.value = EditorGUILayout.ColorField("edge color",water.settings._wavesSettings.edgeColor.value);
            water.settings._wavesSettings.edgeColoringSize.value = EditorGUILayout.Slider("edge size",water.settings._wavesSettings.edgeColoringSize.value,0f,0.3f);
            water.settings._wavesSettings.edgeIgnoreTransparency.value = EditorGUILayout.Toggle("edge ignore transparency",water.settings._wavesSettings.edgeIgnoreTransparency.value);
            EditorGUILayout.Space(10);
            water.settings._wavesSettings.automaticWaves.value = EditorGUILayout.Toggle("auto waves", water.settings._wavesSettings.automaticWaves.value);
            water.settings._wavesSettings.waveDensity.value = EditorGUILayout.FloatField("auto speed", water.settings._wavesSettings.waveDensity.value);
            water.settings._wavesSettings.waveDensity2.value = EditorGUILayout.FloatField("auto density", water.settings._wavesSettings.waveDensity2.value);
            EditorGUILayout.Space(10);
            water.settings._wavesSettings.enableBuoyancy.value = EditorGUILayout.Toggle("enable rigidbody buoyancy", water.settings._wavesSettings.enableBuoyancy.value);
            water.settings._wavesSettings.enableRigidbodyCollisions.value = EditorGUILayout.Toggle("enable rigidbody collisions", water.settings._wavesSettings.enableRigidbodyCollisions.value);
            EditorGUILayout.Space(10);


            EndVB();
        }

        private void WetSurfaceSettings()
        {
            StartVB(sector1);

            EditorGUILayout.LabelField("You can use this settings to make the water act as a wet/rainy surface");
            EditorGUILayout.LabelField("Example is in the: 'wet surface' demo scene");

            water.settings._waterSettings.enableBelowWater.value = EditorGUILayout.Toggle("enable", water.settings._waterSettings.enableBelowWater.value);
            

            water.settings._waterSettings.belowWaterAlpha.value = EditorGUILayout.Slider("alpha (surface below water)", water.settings._waterSettings.belowWaterAlpha.value,0f,1f);
            water.settings._waterSettings.belowWaterDistortionStrength.value = EditorGUILayout.Slider("simulation/distortion strength", water.settings._waterSettings.belowWaterDistortionStrength.value,0f,1f);

            EndVB();
        }

        private void editorSettings()
        {
            StartVB(sector1);
            water.ManagersVisible.value = EditorGUILayout.Toggle("Managers Visible" , water.ManagersVisible.value);
            if (water.ManagersVisible.value) 
            {
                water.settings._reflectionsSettings.cameraVisible.value = EditorGUILayout.Toggle("reflection camera visible", water.settings._reflectionsSettings.cameraVisible.value);
                water.settings._obstructorSettings.cameraVisible.value = EditorGUILayout.Toggle("obstruction camera visible", water.settings._obstructorSettings.cameraVisible.value);
            }
            EditorGUILayout.Space(5);
            water.settings._reflectionsSettings.reflectionObjectsVisible.value = EditorGUILayout.Toggle("reflection objects visible", water.settings._reflectionsSettings.reflectionObjectsVisible.value);
            water.settings._obstructorSettings.obstructionObjectsVisible.value = EditorGUILayout.Toggle("obstruction objects visible", water.settings._obstructorSettings.obstructionObjectsVisible.value);
            EndVB();
        }

        GameObject set;


        private void utilsSettings()
        {
            StartVB(sector1);

            set = (GameObject)EditorGUILayout.ObjectField(set, typeof(GameObject), true);
            if(GUILayout.Button("copy settings"))
            {
                if (set.GetComponent<ModernWater2D>() != null)
                {
                    water.settings = set.GetComponent<ModernWater2D>().settings;
                    Debug.Log("water settings copied");
                }
                else Debug.LogError("couldn't load settings, " + set.name + "doesn't have ModernWater2D component");
                
            }
            EndVB();
        }

        #endregion
    }

}