using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace Water2D
{

    public class ObstructorManager : WaterFeatureLayerRenderer
    {
        [SerializeField] [HideInInspector] public static ObstructorManager instance;

        #region Singleton

        private void Awake()
        {
            Singleton();
            UpdateReflectionsShader();
            GenTextures();
        }

        void Singleton()
        {
            if (instance == null) { instance = this; }
            else if (this == instance) return;
            else DestroyImmediate(gameObject);
        }

        public static ObstructorManager GetInstance()
        {
            if (instance == null)
            {
                if (FindObjectOfType<ObstructorManager>()!= null) instance = FindObjectOfType<ObstructorManager>();
                else throw new Exception("obstructor System Platformer Instance couldn't be found in the scene");
            }
            return instance;
        }
        #endregion

        #region Variables

        public void UpdateSettings(ObstructorSettings os)
        {
            obstructionObjectsVisible.value = os.obstructionObjectsVisible.value;
            cameraVisible.value = os.cameraVisible.value;
            _textureResolution.value = os.textureResolution.value;

            mainCamera = os.mainCamera;
        }


        [HideInInspector][SerializeField] public WaterCryo<bool> overrideMainCamera = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> obstructionObjectsVisible = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<bool> cameraVisible = new WaterCryo<bool>(false);
        [HideInInspector][SerializeField] public WaterCryo<float> _textureResolution = new WaterCryo<float>(1);
        [HideInInspector][SerializeField] public float textureResolution
        {
            get
            {
                if (!genSDF) return _textureResolution.value;
                else
                {
                    return 1920f / (float)Screen.width / 2f; 
                }
            }
        }
        [HideInInspector][SerializeField] public Camera cam;
        [HideInInspector][SerializeField] public Shader jfShader;
        [HideInInspector][SerializeField] public Shader uvShader;
        [HideInInspector][SerializeField] public Material jfMaterial;
        [HideInInspector][SerializeField] public Material uvMaterial;
        [HideInInspector][SerializeField] public RenderTexture jfRenderTexture1;
        [HideInInspector][SerializeField] public RenderTexture jfRenderTexture2;

        [HideInInspector][SerializeField] private Camera _mainCamera;
        [HideInInspector][SerializeField] public Camera mainCamera
        {
            get { if (_mainCamera == null) SetMainCam(); return _mainCamera; }
            set { _mainCamera = value; }
        }

        public const string rlayer = "Obstructors";
        public const string redMatPath = "Materials/Red";
        public const string red3dMatPath = "Materials/Red3d";
        

        private Material _red;
        private Material _red3d;
        public Material red
        {
            get { if (_red == null) _red = (Material)Resources.Load(redMatPath, typeof(Material));  return _red; }
            set { _red = value; }
        }

        public Material red3d
        {
            get { if (_red3d == null) _red3d = (Material)Resources.Load(red3dMatPath, typeof(Material)); return _red3d; }
            set { _red3d = value; }
        }

        public LayerRenderer layerRenderer
        {
            get
            {
                if (_layerRenderer == null)
                {
                    _layerRenderer = new LayerRenderer();
                    _layerRenderer.Setup(mainCamera, transform, rlayer, sizeMLP, textureResolution, RenderTextureFormat.RG16);
                }
                return _layerRenderer;
            }
            set { _layerRenderer = value; }
        }

        [SerializeField] Dictionary<Transform, ObstructorSO> _obstructors = new Dictionary<Transform, ObstructorSO>();
      
        public Vector2 sizeMLP
        {
            get
            {
                if (!genSDF) return new Vector2(1.25f, 1.25f);
                else return new Vector2(2f, 2f);
            }
        }

        Dictionary<Transform, ObstructorSO> obstructors
        {
            get { if (_obstructors == null) _obstructors = new Dictionary<Transform, ObstructorSO>(); return _obstructors; }
            set { _obstructors = value; }
        }


        // contains the created obstruction textures
        [SerializeField][HideInInspector] private static Dictionary<int, ObstructorPair> _obstructionSprites;
        // generate depth
        [SerializeField][HideInInspector] public bool genSDF;

        private static Dictionary<int, ObstructorPair> obstructionSprites
        {
            get { if (_obstructionSprites == null) _obstructionSprites = new Dictionary<int, ObstructorPair>(); return _obstructionSprites; }
            set { _obstructionSprites = value; }
        }

        #endregion

        #region Callbacks

        void SetCallbacks()
        {
            overrideMainCamera.onValueChanged = OnSettingsChangedScene;
            obstructionObjectsVisible.onValueChanged = OnSettingsChangedScene;
            cameraVisible.onValueChanged = OnSettingsChangedScene;

        }

        private void OnSettingsChangedScene()
        {
            GetAllObstructors();
            ObstructionObjectsVisible(obstructionObjectsVisible.value);
        }

        private void ObstructionObjectsVisible(bool value)
        {
            foreach (var t in obstructors) t.Value.child.gameObject.hideFlags = value ? HideFlags.None : HideFlags.HideInHierarchy;
        }

        #endregion

        #region Common 

        public void AddObstructor(ObstructorSO obs)
        {
            if (!obstructors.ContainsKey(obs.source)) obstructors.Add(obs.source, obs);
        }

        public void RemoveObstructor(Transform t)
        {
            if (obstructors.ContainsKey(t)) obstructors.Remove(t);
        }

        public void GetAllObstructors()
        {
            foreach (var r in GameObject.FindObjectsOfType<Obstructor>(true))
            {
                if (!obstructors.ContainsKey(r.transform)) AddObstructor(r.GetComponent<Obstructor>().data);
            }
        }

        private void SetMainCam() 
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
        } 

        #endregion

        #region Disable&Enable

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
            Singleton();
            SetCallbacks();
            if (mainCamera==null) mainCamera = Camera.main;
            layerRenderer.Setup(mainCamera, transform, rlayer, sizeMLP, textureResolution,  RenderTextureFormat.RG16);
            UpdateReflectionsShader();
            jfShader = Shader.Find("Hidden/Jump Flood");
        }


        private void Start()
        {
            GetAllObstructors();
        }

        private void OnDisable()
        {
            layerRenderer.Release();
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
        }

        #endregion

        private void GenTextures()
        {
            var obsTex = layerRenderer.LayerTexture();
            if (jfRenderTexture1 != null) jfRenderTexture1.Release();
            if (jfRenderTexture2 != null) jfRenderTexture2.Release();
            jfRenderTexture1 = new RenderTexture(obsTex.width/2, obsTex.height / 2, 24, RenderTextureFormat.ARGB32);
            jfRenderTexture2 = new RenderTexture(obsTex.width / 2, obsTex.height / 2, 24, RenderTextureFormat.ARGB32);
            jfRenderTexture1.enableRandomWrite = true;
            jfRenderTexture2.enableRandomWrite = true;
            jfRenderTexture1.filterMode = FilterMode.Point;
            jfRenderTexture2.filterMode = FilterMode.Point;
            jfRenderTexture1.wrapMode = TextureWrapMode.Repeat;
            jfRenderTexture2.wrapMode = TextureWrapMode.Repeat;
            jfRenderTexture1.Create();
            jfRenderTexture2.Create();
        }

        public void GenerateSDF() 
        {
            var obsTex = layerRenderer.LayerTexture();
            if (obsTex==null || Screen.width == 0 || Screen.height == 0) return;
            if (jfRenderTexture1 == null || jfRenderTexture1.width == 0 ) GenTextures();
            if (jfRenderTexture2 == null || jfRenderTexture2.width == 0) GenTextures();

            if(jfShader==null) jfShader = Shader.Find("Hidden/Jump Flood");
            if(uvShader==null) uvShader = Shader.Find("Hidden/UV Encoding");
            if (jfMaterial == null) jfMaterial = new Material(jfShader);
            if (uvMaterial == null) uvMaterial = new Material(uvShader);
          
            int iterations = Mathf.CeilToInt(Mathf.Log(jfRenderTexture1.width * jfRenderTexture1.height));

            Vector2 stepSize = new Vector2(jfRenderTexture1.width, jfRenderTexture1.height);

            Graphics.Blit(obsTex, jfRenderTexture1,uvMaterial);

            jfMaterial.SetVector("stepSize", stepSize);
            jfMaterial.SetVector("screenSize", stepSize);
            
        
            for (int i = 0; i < iterations; i++)
            {
                stepSize /= 2.0f;
           
                jfMaterial.SetVector("stepSize", stepSize);

                if (i % 2 == 0)
                    Graphics.Blit(jfRenderTexture1, jfRenderTexture2, jfMaterial);
                else
                    Graphics.Blit(jfRenderTexture2, jfRenderTexture1, jfMaterial);
            }

            if (iterations % 2 == 0) Graphics.Blit(jfRenderTexture1, jfRenderTexture2);


            if (!Shader.IsKeywordEnabled(WaterShaderIdsOBS.DepthTexture)) Shader.SetGlobalTexture(WaterShaderIdsOBS.DepthTexture, jfRenderTexture2);
        }

        #region Update

        protected override void Update()
        {
            base.Update();
            _layerRenderer.Loop();

            if (genSDF) GenerateSDF();
            if (!run) return;

            if (cam==null) cam = GetComponent<Camera>();
            cam.hideFlags = cameraVisible.value ? HideFlags.None : HideFlags.HideInInspector;

            UpdateObstructionSprites();
        }

       
        private void UpdateObstructionSprites()
        {
            foreach (var obs in obstructors)
            {
                if (obs.Key == null) continue;

                if (obs.Value.childSr == null || obs.Value.sourceSr == null) continue;
                if (obs.Value.childSr.flipX != obs.Value.sourceSr.flipX) obs.Value.childSr.flipX = obs.Value.sourceSr.flipX;

                //change/create and change sprite
                if (obs.Value.sourceSr.sprite != obs.Value.childSr.sprite)
                {
                    obs.Value.childSr.sprite = obs.Value.sourceSr.sprite;

                    MaterialPropertyBlock prop = new MaterialPropertyBlock();
                    obs.Value.childSr.GetPropertyBlock(prop);
                    float texH = obs.Value.childSr.sprite.texture.height;
                    prop.SetFloat("_ss", (obs.Value.childSr.sprite.texture.height <= obs.Value.childSr.sprite.rect.height ? 0 : 1));
                    prop.SetFloat("_minY", obs.Value.childSr.sprite.rect.yMin / texH);
                    prop.SetFloat("_maxY", obs.Value.childSr.sprite.rect.yMax / texH);
                    obs.Value.childSr.SetPropertyBlock(prop);
                }
            }
        }

        private void UpdateReflectionsShader()
        {
            if (!Shader.IsKeywordEnabled(WaterShaderIdsOBS.OBStexture)) Shader.SetGlobalTexture(WaterShaderIdsOBS.OBStexture, layerRenderer.LayerTexture());
        }

        #endregion

    }
}