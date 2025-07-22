using LeafUtils;
using UnityEngine;
using UnityEngine.Profiling;

namespace Water2D
{
    public class WaterSimulationSimple : WaterSimulation
    {

        private Material _waveShader;
        private Material waveShader
        {
            get { if (_waveShader == null) _waveShader = new Material(Shader.Find("Water2D/Simulations/process")); return _waveShader; }
            set { _waveShader = value; }
        }

        [SerializeField] private RenderTexture CurrentState, Temporary;
        [SerializeField] private RenderTexture ObstructionTex;
        [SerializeField] private Vector4 ObstructionTexPos;
        [SerializeField] private Vector2Int resolution;
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private float waveRad = 0.005f;
        [SerializeField] private float waveHeight = 1f;
        [SerializeField] private float dispersion = 0.98f;
        [SerializeField] private float simSpeed = 0.98f;
        [SerializeField] private int iterations = 3;
        [SerializeField] private int diffusionSize = 3;

        [SerializeField] private float rainSpeed = 1f;
        [SerializeField] private float rainWaveH = 1f;

        [SerializeField] private int rainSizeX = 1;
        [SerializeField] private int rainSizeY = 1;
        [SerializeField] private bool enableRain = false;


        [SerializeField][HideInInspector] Camera _mainCam;
        Camera mainCam
        {
            set { _mainCam = value; }
            get { if (_mainCam == null) _mainCam = ObstructorManager.instance.cam; return _mainCam; }
        }

        public void Setup(Camera mainCam, float simSpeed, Vector2Int resolution, SpriteRenderer sr, RenderTexture obstruction, float rainSpeed = 1, int rainSizeX = 1, int rainSizeY = 1, float rainWaveH = 1, float waveRad = 0.005f, float waveHeight = 1f, float dispersion = 0.98f, int iterations = 3, bool enableRain = false)
        {
            this.resolution = new Vector2Int(resolution.x, (int)(resolution.x * (1f / (sr.bounds.size.x / sr.bounds.size.y))));

            this.mainCam = mainCam;
            this.sr = sr;
            this.ObstructionTex = obstruction;
            this.waveRad = waveRad;
            this.waveHeight = waveHeight;
            this.dispersion = dispersion;
            this.iterations = iterations;
            this.rainSpeed = rainSpeed;
            this.rainWaveH = rainWaveH;
            this.rainSizeX = rainSizeX;
            this.rainSizeY = rainSizeY;
            this.enableRain = enableRain;
            this.simSpeed = simSpeed; 

            Init();
        }



        public override RenderTexture GetRT()
        {
            return Temporary;
        }

        void InitTex(out RenderTexture rt)
        {
            if (resolution.x == 0 || resolution.y == 0) resolution = new Vector2Int(1024, (int)(1f / (sr.bounds.size.x / sr.bounds.size.y) * 1024f));

            rt = new RenderTexture(resolution.x, resolution.y, 1, RenderTextureFormat.RGHalf);
            rt.enableRandomWrite = false;
            rt.depth = 24;
            rt.filterMode = FilterMode.Bilinear;
            rt.Create();
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.clear);
        }

        void Init()
        {
            if (CurrentState != null) CurrentState.Release();
            if (CurrentState != null) CurrentState.Release();

            InitTex(out CurrentState);
            InitTex(out Temporary);
        }

        void CreateIfNull()
        {
            if (CurrentState == null) InitTex(out CurrentState);
            if (Temporary == null) InitTex(out Temporary);
        }

        Vector4 GetObstructionPositions()
        {
            Vector2 v1, v2;
            if (mainCam != ObstructorManager.instance.cam) mainCam = ObstructorManager.instance.cam;
            if (!mainCam.orthographic)
            {
                v1 = new Vector2(-0f, -0f);
                v2 = new Vector2(1f, 1f);
            }
            else
            {

     
                v1 = mainCam.ViewportToWorldPoint(new Vector3( 0f ,0f, 0f));
                v2 = mainCam.ViewportToWorldPoint(new Vector3( 1f , 1f, 0f));


            }
            
            return new Vector4(v1.x, v1.y, v2.x, v2.y);
        }


        private Camera GetCameraRenderingScreen()
        {
            if (mainCam == null) mainCam = ObstructorManager.instance.cam;
            return mainCam;
        }

        Vector4 GetTexturePositions()
        {
            
            Camera cam = GetCameraRenderingScreen();
            Vector3 v1, v2;
            if (!cam.orthographic)
            {
                v1 = cam.WorldToViewportPoint(new Vector3(sr.bounds.min.x, sr.bounds.min.y, sr.bounds.center.z));
                v2 = cam.WorldToViewportPoint(new Vector3(sr.bounds.max.x, sr.bounds.max.y, sr.bounds.center.z));
            }
            else
            {
                v1 = sr.bounds.min;
                v2 = sr.bounds.max;

            }
            return new Vector4(v1.x, v1.y, v2.x, v2.y);
        }

        void Render()
        {
            //blit here
            RenderUtils.RenderToRT2D(new Rect(0, 0, resolution.x, resolution.y), waveShader, Temporary);
            Graphics.CopyTexture(Temporary, CurrentState);
        }

        Vector4 TexturePos = Vector4.zero;
        Vector4 lastTexturePos = Vector4.zero;

        public override void UpdLoop()
        {
            lastTexturePos = TexturePos;
            TexturePos = GetObstructionPositions();
        }

        public Vector4 FutureSight() 
        {
            return TexturePos  ;
        }

        public override void Loop()
        {
            
            ObstructionTex = ObstructorManager.instance.layerRenderer.LayerTexture();
            waveShader.SetTexture("_ObstructionTex", ObstructionTex);
            waveShader.SetVector("OstructionTexPos", FutureSight());
       
            waveShader.SetVector("OstructionTexRes", new Vector4(ObstructionTex.width, ObstructionTex.height));
            waveShader.SetVector("texPos", GetTexturePositions() );

            CreateIfNull();

            waveShader.SetTexture("_NState", CurrentState);
            waveShader.SetVector("resolution", new Vector4(resolution.x, resolution.y));
            waveShader.SetFloat("waveRad", waveRad);
            waveShader.SetFloat("dispersion", dispersion);
            waveShader.SetFloat("waveHeight", waveHeight);
            waveShader.SetFloat("diffusionSize", diffusionSize);
            waveShader.SetFloat("enableRain", enableRain ? 1 : 0);
            waveShader.SetFloat("rainSpeed", rainSpeed);
            waveShader.SetFloat("rainWaveH", rainWaveH);
            waveShader.SetFloat("rainSizeX", rainSizeX);
            waveShader.SetFloat("rainSizeY", rainSizeY);
            waveShader.SetFloat("timeFromStart", Time.timeSinceLevelLoad);

            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

            sr.GetPropertyBlock(propBlock);
            propBlock.SetVector("_simUvs", new Vector4(0f, 0f, 1f, 1f));
            sr.SetPropertyBlock(propBlock);

            Profiler.BeginSample("Rendering Water Simulation");
            for (int i = 0; i < iterations; i++) Render();
            Profiler.EndSample();

           
        }


        public override void Setup(SimulationSettings value)
        {
            Setup(value.mainCam, value.simulationSpeed.value, value.resolution.value, value.sr, value.obstruction, value.rainSpeed.value, value.rainSizeX.value, value.rainSizeY.value, value.rainWaveHeight.value, value.waveRad.value, value.waveHeight.value, value
                .dispersion.value, value.iterations.value, value.enableRain.value);
        }

        public override void UpdateSettings(SimulationSettings value)
        {
            Setup(value);
        }
    }
}