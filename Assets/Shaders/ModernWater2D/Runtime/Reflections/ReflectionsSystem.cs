using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Water2D
{
    [Serializable]
    public class ReflectionsSystem : WaterFeatureLayerRenderer
    {
        #region Singleton
        [SerializeField][HideInInspector] public static ReflectionsSystem instanceTopDown;
        [SerializeField][HideInInspector] public static ReflectionsSystem instanceRayMarch;
        [SerializeField][HideInInspector] public static ReflectionsSystem instancePlatformer;

        public ReflectionsSystem(bool topdown) => this.topdown = topdown;

        [SerializeField][HideInInspector] bool startupQF = false;
        private void Awake()
        {
            Singleton();
            UpdateReflectionsShader();
            SetCallbacks();
        }

        void Singleton()
        {
            if (!startupQF) { startupQF = true; return; }

            if (raymarch)
            {

                if (instanceRayMarch == null) { instanceRayMarch = this; }
                else if (this == instanceRayMarch) return;
                else DestroyImmediate(gameObject);
            }
            else if (topdown)
            {

                if (instanceTopDown == null) { instanceTopDown = this; }
                else if (this == instanceTopDown) return;
                else DestroyImmediate(gameObject);
            }
            else 
            {
                if (instancePlatformer == null) { instancePlatformer = this; }
                else if (this == instancePlatformer) return;
                else DestroyImmediate(gameObject);
            }
        }

        public static ReflectionsSystem GetInstanceTopDown() 
        {
            return instanceTopDown;
        }

        public static ReflectionsSystem GetInstancePlatformer()
        {

            return instancePlatformer;
        }


        public static ReflectionsSystem GetInstanceRayMarch()
        {
            return instanceRayMarch;
        }

        #endregion

        #region Variables

        [HideInInspector] public int pivotDetectionAlphaTreshold = 122;

        [HideInInspector] [SerializeField] public WaterCryo<bool> overrideMainCamera = new WaterCryo<bool>(false);
        [HideInInspector] [SerializeField] public WaterCryo<bool> reflectionObjectsVisible = new WaterCryo<bool>(false);
        [HideInInspector] [SerializeField] public WaterCryo<bool> cameraVisible = new WaterCryo<bool>(false);
        [HideInInspector] [SerializeField] public WaterCryo<bool> defaultReflectionSprflipx = new WaterCryo<bool>(false);
        [HideInInspector] [SerializeField] public Camera mainCamera;
        [HideInInspector] [SerializeField] public Camera reflectionCamera;
        [HideInInspector] [SerializeField] public Material reflectorMat;
        [HideInInspector] [SerializeField] public Material reflectionMat;
        [HideInInspector] [SerializeField] public WaterCryo<float> textureResolution = new WaterCryo<float>(1);
        [HideInInspector][SerializeField] public int layers;

        public const string rlayer = "Reflections";
        public const string reflectorMatPath = "Materials/reflector_mat";
        public const string reflectionMatPath = "Materials/reflection_mat";

        [HideInInspector][SerializeField] public bool topdown = false;
        [HideInInspector][SerializeField] public bool raymarch = false;

        [HideInInspector] public int reflectionLayerIdx;

        LayerRenderer layerRenderer 
        {
            get 
            {
                if (_layerRenderer == null) 
                { 
                    _layerRenderer = new LayerRenderer();
                    if(mainCamera == null) mainCamera = Camera.main;
                    if(textureResolution==null) textureResolution = new WaterCryo<float>(1);
                    if (topdown)_layerRenderer.Setup(mainCamera, transform, rlayer, Vector2.one, textureResolution.value);
                    else _layerRenderer.Setup(mainCamera, transform, layers, new Vector2(1f,1.5f), textureResolution.value);
                }
                return _layerRenderer; 
            }
            set { _layerRenderer = value; } 
        }
        

        [SerializeField] [HideInInspector] ReflectionSettings _reflectionsSettings;
        [SerializeField] [HideInInspector] public ReflectionSettings reflectionsSettings
        {
            get { if(_reflectionsSettings == null) _reflectionsSettings = new ReflectionSettings(); return _reflectionsSettings; }
            set { _reflectionsSettings = value; }   
        }

        #endregion Variables

        #region Dictionaries

        // caster transform and reflector object
        [SerializeField] Dictionary<Transform, ReflectionSO> _reflectors = new Dictionary<Transform, ReflectionSO>();
        Dictionary<Transform, ReflectionSO> reflectors
        {
            get { if (_reflectors == null) _reflectors = new Dictionary<Transform, ReflectionSO>(); return _reflectors; }
            set { _reflectors = value; }
        }

      

        [SerializeField] Dictionary<Texture2D, Vector2> _pivots = new Dictionary<Texture2D, Vector2>();
        Dictionary<Texture2D, Vector2> pivots
        {
            get { if (_pivots == null) _pivots = new Dictionary<Texture2D, Vector2>(); return _pivots; }
            set { _pivots = value; }
        }

        [SerializeField] Dictionary<Sprite, Vector2> _pivotsSH = new Dictionary<Sprite, Vector2>();
        Dictionary<Sprite, Vector2> pivotsSH
        {
            get { if (_pivotsSH == null) _pivotsSH = new Dictionary<Sprite, Vector2>(); return _pivotsSH; }
            set { _pivotsSH = value; }
        }

        #endregion Dictionaries

        #region Common

        public void AddReflector(ReflectionSO r)
        {
            if (!reflectors.ContainsKey(r.source)) reflectors.Add(r.source, r);
            else
            {
                reflectors.Remove(r.source);
                reflectors.Add(r.source, r);
            }
            UpdateReflection(r);
        }

        public void RemoveReflector(ReflectionSO r)
        {
            if (reflectors.ContainsKey(r.source)) reflectors.Remove(r.source);
        }

        public void GetAllReflectors()
        {
            foreach (var r in GameObject.FindObjectsOfType<Reflector>(true))
            {
                if (!reflectors.ContainsKey(r.transform)) AddReflector(r.data);
            }
        }

        public void UpdateAllReflectors() 
        {
            UpdateReflectionsPhysics();
            UpdateReflectionsShader();
        }


        public void ReflectionObjectsVisible(bool visible)
        {
            foreach (var r in reflectors)
            {
                r.Value.reflection.hideFlags = (visible ? HideFlags.None : HideFlags.HideInHierarchy);
                r.Value.reflectionPivot.hideFlags = (visible ? HideFlags.None : HideFlags.HideInHierarchy);
            }
        }

        public void UpdateSettings(ReflectionsSettings reflectionsSettings, bool topdown)
        {
            this.reflectionsSettings.originalColor.value = reflectionsSettings.originalColor.value;
            this.reflectionsSettings.length.value = reflectionsSettings.length.value;
            this.reflectionsSettings.color.value = reflectionsSettings.color.value;
            this.reflectionsSettings.angle.value = reflectionsSettings.angle.value;
            this.reflectionsSettings.alpha.value = reflectionsSettings.alpha.value;
            this.reflectionsSettings.tilt.value = reflectionsSettings.tilt.value;
            this.reflectionsSettings.y.value = reflectionsSettings.mirrorY.value;
            if (cameraVisible != null )
            {
                cameraVisible.value = reflectionsSettings.cameraVisible.value;
                defaultReflectionSprflipx.value = reflectionsSettings.defaultReflectionSprflipx.value;
                mainCamera = reflectionsSettings.mainCamera;
                reflectionObjectsVisible.value = reflectionsSettings.reflectionObjectsVisible.value;
                overrideMainCamera.value = reflectionsSettings.overrideMainCamera.value;
                textureResolution.value = reflectionsSettings.textureResolution.value;
            }
                //create layers bitmask
            layers = 0;
            if (!topdown && !raymarch)
            {
                foreach (int bit in reflectionsSettings.layers)
                {
                    layers |= (1 << bit);
                }
            }
            else if(raymarch)
            {
                foreach (int bit in reflectionsSettings.raymarchlayers)
                {
                    layers |= (1 << bit);
                }
            }
        }


        void SetCallbacks()
        {
            if (overrideMainCamera == null) return;
            overrideMainCamera.onValueChanged = OnSettingsChanged;
            reflectionObjectsVisible.onValueChanged = OnSettingsChangedScene;
            reflectionsSettings.length.onValueChanged = OnSettingsChanged;
            reflectionsSettings.originalColor.onValueChanged = OnSettingsChanged;
            reflectionsSettings.tilt.onValueChanged = OnSettingsChanged;
            reflectionsSettings.angle.onValueChanged = OnSettingsChanged;
            reflectionsSettings.color.onValueChanged = OnSettingsChanged;
            reflectionsSettings.alpha.onValueChanged = OnSettingsChanged;
        }

        private void OnSettingsChangedScene()
        {
            GetAllReflectors();
            ReflectionObjectsVisible(reflectionObjectsVisible.value);
        }

        private void OnSettingsChanged()
        {
            update_extended = true;
            GetAllReflectors();
            UpdateReflections();
        }


        #endregion

        #region Enable&Disable

        public void OnEnable()
        {
            if (mainCamera == null) mainCamera = Camera.main;
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
           
     
            if (topdown) _layerRenderer.Setup(mainCamera, transform, rlayer, Vector2.one, textureResolution.value);
            else  _layerRenderer.Setup(mainCamera, transform, layers, new Vector2(1f, 1.5f), textureResolution.value);
            OnSettingsChanged();
            SetupVariables();
            reflectionCamera = GetComponent<Camera>();
        }

        private void SetupVariables()
        {
            if (reflectorMat == null)
            {

                if (Resources.Load(reflectorMatPath, typeof(Material)) as Material == null) Debug.LogError("Material 'reflectorMat' doesn't exist in path : " + reflectorMatPath);
                else reflectorMat = Resources.Load(reflectorMatPath, typeof(Material)) as Material;
            }
            if (reflectionMat == null)
            {
                if (Resources.Load(reflectionMatPath, typeof(Material)) as Material == null) Debug.LogError("Material 'reflectionMat' doesn't exist in path : " + reflectionMatPath);
                else reflectionMat = Resources.Load(reflectionMatPath, typeof(Material)) as Material;
            }
        }

        private void OnDisable()
        {
            layerRenderer.Release();
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
        }

        private void OnDestroy()
        {
       
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
        }

        #endregion

        #region Updates

        private void Start()
        {
            GetAllReflectors();
        }

        protected override void Update()
        {
            base.Update();
            if (!run) return;
            if (this == null) return;
            if (reflectionCamera != null) reflectionCamera.hideFlags = (cameraVisible.value ? HideFlags.None : HideFlags.HideInInspector);
            layerRenderer.Loop();


            if (!topdown)
            {
                if (mainCamera == null) mainCamera = Camera.main;
                transform.position = mainCamera.transform.position + new Vector3(0, mainCamera.orthographicSize * 0.5f, 0);
                return;
            }

            if (!topdown && !raymarch)
            {
                UpdateReflectionsPhysics();
            }
        }


        private void UpdateReflections()
        {

            update_extended = true;
            UpdateReflectionsShader();
            UpdateReflectionsPhysics();
        }


        Transform[] cleaner = new Transform[50000];
        int cleanIdx = 0;


        private void UpdateReflectionsPhysics()
        {
            if(cleaner==null) cleaner = new Transform[50000];

            foreach (var reflectorPair in reflectors)
            {
                if (reflectorPair.Key == null || reflectorPair.Value==null ||  reflectorPair.Value.reflectionPivot == null || reflectorPair.Value.reflection == null)
                {
                    cleaner[cleanIdx] = reflectorPair.Key;
                    cleanIdx++;
                    continue;
                }
                UpdateReflection(reflectorPair.Value);
            }

            //clean
            for(int i = 0; i < cleanIdx; i++) reflectors.Remove(cleaner[i]);
            cleanIdx = 0;
            update_extended = false;
        }

        public static bool update_extended = false;

        private void UpdateReflection(ReflectionSO reflection)
        {
            if (!reflection.raymarched && (reflection.reflectionSr.sprite != reflection.sourceSr.sprite)  ) reflection.reflectionSr.sprite = ( reflection.MSP_ReflectionGenerator ? reflection.reflectionSr.sprite : reflection.sourceSr.sprite) ;
            
            SetReflectionXOrientation(reflection);

            if (reflection.reflection.localScale != Vector3.one) reflection.reflection.localScale = Vector3.one;

            if (!update_extended) return;

            if (reflection.reflectionPivot.rotation != Quaternion.Euler(_reflectionsSettings.tilt.value + reflection.additionalTilt, reflection.reflectionPivot.rotation.y, 180 + _reflectionsSettings.angle.value)) reflection.reflectionPivot.rotation = Quaternion.Euler(_reflectionsSettings.tilt.value + reflection.additionalTilt, reflection.reflectionPivot.rotation.y,180+_reflectionsSettings.angle.value);

            SetReflectionPivotPos(reflection);
        }

        private void UpdateReflectionsShader()
        {
            if(!Shader.IsKeywordEnabled(WaterShaderIdsREF.color)) Shader.SetGlobalColor(WaterShaderIdsREF.color, reflectionsSettings.color.value);
            if(!Shader.IsKeywordEnabled(WaterShaderIdsREF.orgColor)) Shader.SetGlobalFloat(WaterShaderIdsREF.orgColor, reflectionsSettings.originalColor.value);
            if (!Shader.IsKeywordEnabled(WaterShaderIdsREF.alpha)) Shader.SetGlobalFloat(WaterShaderIdsREF.alpha, reflectionsSettings.alpha.value);
            if (!Shader.IsKeywordEnabled(WaterShaderIdsREF.reflectionsTexture) && topdown) Shader.SetGlobalTexture(WaterShaderIdsREF.reflectionsTexture, layerRenderer.LayerTexture());
            else if (!Shader.IsKeywordEnabled(WaterShaderIdsREF.reflectionsTexture3) && raymarch) Shader.SetGlobalTexture(WaterShaderIdsREF.reflectionsTexture3, layerRenderer.LayerTexture());
            else if (!Shader.IsKeywordEnabled(WaterShaderIdsREF.reflectionsTexture2) && !raymarch && !topdown) Shader.SetGlobalTexture(WaterShaderIdsREF.reflectionsTexture2, layerRenderer.LayerTexture());
        }


        private void SetReflectionXOrientation(ReflectionSO reflection)
        {
            if (reflection.reflectionSr.flipX == ((defaultReflectionSprflipx.value ? reflection.flipX : !reflection.flipX) ? !reflection.sourceSr.flipX : reflection.sourceSr.flipX)) return;
            reflection.reflectionSr.flipX =  ( (defaultReflectionSprflipx.value ? reflection.flipX : !reflection.flipX) ? !reflection.sourceSr.flipX : reflection.sourceSr.flipX);
        }

        private void SetReflectionPivotPos(ReflectionSO reflection)
        {

            //pivot pos
            switch (reflection.reflectionPivotSourceMode)
            {
                case ReflectionPivotSourceMode.auto:
                    reflection.reflectionPivot.localPosition =  - ( reflection.source.lossyScale * GetSpritePivot(reflection.sourceSr.sprite)) + reflection.displacement;
                    if (reflection.raymarched) { reflection.reflection.localPosition = Vector3.zero; break; }
                    else reflection.reflection.localPosition = GetSpritePivot(reflection.reflectionSr.sprite) ;
                    break;
                case ReflectionPivotSourceMode.sprite_pivot:
                    reflection.reflectionPivot.localPosition = reflection.displacement ;
                    reflection.reflection.localPosition = Vector2.zero;
                    break;
                case ReflectionPivotSourceMode.custom_transform:
                    reflection.reflectionPivot.position = (Vector2)reflection.customPivot.position + reflection.displacement;
                    reflection.reflection.localPosition = (Vector2)reflection.reflectionPivot.position - (Vector2)reflection.customPivot.position ;
                    break;
            }
        }

        #endregion

        #region Pivot


        private bool IsSpriteFromSpriteSheet(Sprite s)
        {
            if (s.rect.width >= s.texture.width && s.rect.height >= s.texture.height)
                return false;
            return true;

        }


        public Vector2 GetSpritePivotSpriteSheet(Sprite org)
        {

            //sprite sheets
            if (!pivotsSH.ContainsKey(org))
            {
                //delete spritesheet texture from pivots if it was placed by accident, as it will overshadow our pivots.
                if (pivots.ContainsKey(org.texture))
                    pivots.Remove(org.texture);

                Vector2 pos = new Vector2(0, 0.5f);
                var col = org.texture.GetPixelData<Color32>(0);
                Color32[] colors = new Color32[(int)org.rect.width * ((int)org.rect.height + 1)];

                int wX = (int)org.rect.xMax - (int)org.rect.xMin;

                for (int y = (int)org.rect.yMin; y < (int)org.rect.yMax; y++)
                    for (int x = (int)org.rect.xMin; x < (int)org.rect.xMax; x++)
                    {
                        colors[(x - (int)org.rect.xMin) + ((y - (int)org.rect.yMin) * wX)] = col[x + (y * (int)org.texture.width)];
                    }
                int width = (int)org.rect.width;

                //get the pivot by iterating sprite texture and getting the average of the last row of pixels
                for (int i = 0; i < colors.Length; i++)
                    if (colors[i].a > pivotDetectionAlphaTreshold)
                    {
                        pos = new Vector2(org.rect.width/2f, i / width);
                        break;
                    }

                pos = new Vector2(Mathf.Lerp(0, 1, pos.x / (float)org.rect.width), Mathf.Lerp(0, 1, pos.y / (float)org.rect.height));

                Vector2 pivot = new Vector2(org.pivot.x / org.rect.width, org.pivot.y / org.rect.height);
                Vector2 offsetVec = pivot - pos;

                //multiply the pivot displacement vector by world sprite size
                Vector3 spriteNewPivotOffset = new Vector3(org.bounds.size.x * offsetVec.x, org.bounds.size.y * offsetVec.y);
                pivotsSH[org] = spriteNewPivotOffset;
            }

            return pivotsSH[org];
        }

        /// <summary>
        /// automatically calculates a new pivot on the bottom center of the sprite, so you don't need to do this manually
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public Vector2 GetSpritePivot(Sprite org)
        {

            if (IsSpriteFromSpriteSheet(org))
                return GetSpritePivotSpriteSheet(org);

            if (!pivots.ContainsKey(org.texture))
            {
                Vector2 pos = new Vector2(0, 0.5f);
                Color32[] colors = org.texture.GetPixels32();
                int width = org.texture.width;

                //get the pivot by iterating sprite texture and getting the average of the last row of pixels
                for (int i = 0; i < colors.Length; i++)
                    if (colors[i].a > pivotDetectionAlphaTreshold)
                    {
                        pos = new Vector2( (float)org.rect.width/2f , i / width);
                        break;
                    }

                pos = new Vector2(Mathf.Lerp(0, 1, pos.x / (float)org.rect.width), Mathf.Lerp(0, 1, pos.y / (float)org.rect.height));

                Vector2 pivot = new Vector2(org.pivot.x / org.texture.width, org.pivot.y / org.texture.height);
                Vector2 offsetVec = pivot - pos;

                //multiply the pivot displacement vector by world sprite size
                Vector3 spriteNewPivotOffset = new Vector3(org.bounds.size.x * offsetVec.x, org.bounds.size.y * offsetVec.y);
                pivots[org.texture] = spriteNewPivotOffset;
            }

            return pivots[org.texture];
        }


        #endregion Pivot
    }

}