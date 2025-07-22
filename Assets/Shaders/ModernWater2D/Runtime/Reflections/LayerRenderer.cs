using System;
using UnityEngine;

namespace Water2D
{

    public enum LayerRendererType
    {
        spriteRendererSingle,
        spriteRendererMultiple,
        screenSingle,
        screenMultiple
    }

    [Serializable]
    public class LayerRenderer  
    {
        [HideInInspector] [SerializeField] protected ModernWater2D water2D;
        [HideInInspector] [SerializeField] protected RenderTexture layerTexture;
        [HideInInspector] [SerializeField] protected LayerRendererType rendererType;
        [HideInInspector] [SerializeField] protected SpriteRenderer sr;
        [HideInInspector] [SerializeField] protected RenderTextureFormat format = RenderTextureFormat.ARGB32;
        [HideInInspector] [SerializeField] protected FilterMode fliterMode = FilterMode.Point;
        [HideInInspector] [SerializeField] protected float bitDepth = 0;
        [HideInInspector] [SerializeField] protected string layerName = "nl";
        [HideInInspector] [SerializeField] protected int layerMask = -1;
        [HideInInspector] [SerializeField] protected Camera mainCamera;
        [HideInInspector] [SerializeField] protected Camera CameraRenderingScene;
        [HideInInspector] [SerializeField] protected Transform holder;
        [HideInInspector] [SerializeField] protected Transform follow;
        [HideInInspector] [SerializeField] [Range(0f,1f)] protected float res;
        [HideInInspector] [SerializeField] [Range(1f,2f)] protected Vector2 scale = new Vector2(1f,1f);
        [HideInInspector] [SerializeField] int reflectionLayerIdx = -1;

        private bool _run;
        [HideInInspector][SerializeField] internal bool copyMainBackground = false;
        [HideInInspector][SerializeField] float lastOrtographicSize = 0f;
        [HideInInspector][SerializeField] float lastAspectRatio = 0f;

        public bool run 
        {
            get { return _run; }
            set 
            {
                if(value == true) { mainCamera.aspect = lastAspectRatio; mainCamera.orthographicSize = lastOrtographicSize; mainCamera.enabled = true; }
                else { lastAspectRatio = CameraRenderingScene.aspect * (scale.x/scale.y); lastOrtographicSize = CameraRenderingScene.orthographicSize * scale.y; mainCamera.orthographicSize = 0; mainCamera.enabled = false;   }
                _run = value;
            }
        }


        public RenderTexture LayerTexture() { return layerTexture; }

        /// <summary>
        /// Sets up a camera that covers a sprite renderer and renders one layer
        /// </summary>
        public void Setup(Camera mCamera, SpriteRenderer sr, Transform holder, string layerName, float resolution = 1, RenderTextureFormat format = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Point, float bitdepth = 0)
        {
            rendererType = LayerRendererType.spriteRendererSingle;
            mainCamera = holder.GetComponent<Camera>();
            CameraRenderingScene = mCamera;
            this.holder = holder;
            this.layerName = layerName;
            this.format = format;
            this.fliterMode = filterMode;
            this.bitDepth = bitdepth;
            this.res = resolution;
            this.sr = sr;
            mainCamera.aspect = sr.bounds.extents.x / sr.bounds.extents.y;

            StripCamera();
            mainCamera.orthographicSize = sr.bounds.size.y / 2f;
            mainCamera.backgroundColor = Color.clear;

            if (Screen.width == 0) return;
            if (layerTexture != null) layerTexture.Release();

            CreateRTSpriteRenderer(sr, mCamera);

#if UNITY_EDITOR
            if (!WaterLayers.LayerExists(layerName))
            {
                WaterLayers.CreateLayer(layerName);
            }
#endif
            reflectionLayerIdx = Obstructor.GetLayerIdx(layerName);

              mainCamera.depth = mCamera.depth-1;
            if (reflectionLayerIdx != -1)
            {
                int cmask = (1 << reflectionLayerIdx);
                mainCamera.cullingMask = cmask;
                mainCamera.targetTexture = layerTexture;
                RemoveLayerFromMainCamera();
            }

        }


        /// <summary>
        /// Sets up a camera that covers a sprite renderer and renders multiple layer
        /// </summary>
        public void Setup(Camera mCamera, SpriteRenderer sr, Transform holder, int layers, float resolution = 1, RenderTextureFormat format = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Point, float bitdepth = 0)
        {
            rendererType = LayerRendererType.spriteRendererMultiple;
            mainCamera = holder.GetComponent<Camera>();
            CameraRenderingScene = mCamera;
            this.holder = holder;
            this.layerMask = layers;
            this.format = format;
            this.fliterMode = filterMode;
            this.bitDepth = bitdepth;
            this.res = resolution;
            this.sr = sr;
            mainCamera.aspect = sr.bounds.extents.x / sr.bounds.extents.y;

            StripCamera();
            mainCamera.orthographicSize = sr.bounds.size.y / 2f;
            mainCamera.backgroundColor = Color.clear;

            if (Screen.width == 0) return;
            if (layerTexture != null) layerTexture.Release();

            CreateRTSpriteRenderer(sr, mCamera);

            mainCamera.cullingMask = layerMask;
            mainCamera.targetTexture = layerTexture;
            mainCamera.depth = mCamera.depth - 1;
        }

        private void CreateRT(SpriteRenderer sr, Camera mCamera, LayerRendererType type ) 
        {
            
            switch (type)
            {
                case LayerRendererType.spriteRendererSingle:
                    CreateRTSpriteRenderer(sr, mCamera);
                    break;
                case LayerRendererType.spriteRendererMultiple:
                    CreateRTSpriteRenderer(sr, mCamera);
                    break;
                case LayerRendererType.screenSingle:
                    CreateRTCamera(mCamera);
                    break;
                case LayerRendererType.screenMultiple:
                    CreateRTCamera(mCamera);
                    break;
            }
        }

        private void CreateRTSpriteRenderer(SpriteRenderer sr, Camera mCamera)
        {

            float perX = sr.bounds.size.x / (mCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - mCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x);
            float perY = sr.bounds.size.y / (mCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - mCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y);
            if (layerTexture != null) layerTexture.Release();
            if (layerTexture == null) layerTexture = new RenderTexture((int)(mCamera.scaledPixelWidth * res * perX * Mathf.Max(1f, scale.x)), (int)(mCamera.scaledPixelHeight * res * perY * Mathf.Max(1f, scale.y)), 32, format, 0);
            else
            {
                layerTexture.width = (int)(mCamera.scaledPixelWidth * res * perX * Mathf.Max(1f, scale.x) );
                layerTexture.height = (int)(mCamera.scaledPixelHeight * res * perY * Mathf.Max(1f, scale.y));
            }
            layerTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat;
            layerTexture.filterMode = FilterMode.Bilinear;
            layerTexture.enableRandomWrite = false;
            if (layerTexture == null) layerTexture.Create();
            mainCamera.targetTexture = layerTexture;
        }

        private void CreateRTCamera(Camera mCamera)
        {
   
            if (layerTexture != null) layerTexture.Release();
            if (layerTexture == null) layerTexture = new RenderTexture((int)((float)mCamera.scaledPixelWidth * res * Mathf.Max(1f, scale.x)), (int)((float)mCamera.scaledPixelHeight * res * Mathf.Max(1f, scale.y)), 32, format, 0);
            else
            {
                layerTexture.width = (int)((float)mCamera.scaledPixelWidth * res * Mathf.Max(1f, scale.x));
                layerTexture.height = (int)((float)mCamera.scaledPixelHeight * res * Mathf.Max(1f, scale.y));
            }
            layerTexture.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat;
            layerTexture.filterMode = FilterMode.Bilinear;
            layerTexture.enableRandomWrite = false;
            if (layerTexture == null) layerTexture.Create();
            mainCamera.targetTexture = layerTexture;

      
        }

        void StripCamera()
        {
            mainCamera.depthTextureMode = DepthTextureMode.None;
        }

        /// <summary>
        /// Sets up a camera that covers the screen and renders one layer 
        /// </summary>
        public void Setup(Camera mCamera, Transform holder, string layerName, Vector2 scale, float resolution = 1,  RenderTextureFormat format = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Point, float bitdepth = 0)
        {
            rendererType = LayerRendererType.screenSingle;
            mainCamera = holder.GetComponent<Camera>();
            CameraRenderingScene = mCamera;
            StripCamera();
            follow = mCamera.transform;
            this.holder = holder;
            this.layerName = layerName;
            this.format = format; 
            this.fliterMode = filterMode; 
            this.bitDepth = bitdepth;
            this.res = resolution;
            this.scale = scale;
            mainCamera.CopyFrom(mCamera);
            mainCamera.depth = mCamera.depth - 1;
            mainCamera.aspect = mCamera.aspect * (scale.x / scale.y);
            mainCamera.orthographicSize = mCamera.orthographicSize * this.scale.y;
            lastOrtographicSize = mainCamera.orthographicSize;
            lastAspectRatio = mainCamera.aspect;
            if (!copyMainBackground) mainCamera.backgroundColor = Color.clear;
            else mainCamera.backgroundColor = mCamera.backgroundColor;

            RTSetup();
        }

        /// <summary>
        /// Sets up a camera that covers the screen and renders multiple layers
        /// </summary>
        public void Setup(Camera mCamera, Transform holder, int layers, Vector2 scale, float resolution = 1, RenderTextureFormat format = RenderTextureFormat.ARGB32, FilterMode filterMode = FilterMode.Point, float bitdepth = 0)
        {
            rendererType = LayerRendererType.screenMultiple;
            mainCamera = holder.GetComponent<Camera>();
            CameraRenderingScene = mCamera;
            StripCamera();
            follow = mCamera.transform;
            this.holder = holder;
            this.layerMask = layers;
            this.format = format;
            this.fliterMode = filterMode;
            this.bitDepth = bitdepth;
            this.res = resolution;
            this.scale = scale;
            mainCamera.CopyFrom(mCamera);
            mainCamera.depth = mCamera.depth-1;
            mainCamera.aspect = mCamera.aspect * (scale.x / scale.y);
            mainCamera.orthographicSize = mCamera.orthographicSize * this.scale.y;
            lastOrtographicSize = mainCamera.orthographicSize;
            lastAspectRatio = mainCamera.aspect;
            if (!copyMainBackground) mainCamera.backgroundColor = Color.clear;
            else mainCamera.backgroundColor = mCamera.backgroundColor;


            RTSetupExtended();
        }

        private void RTSetupExtended()
        {
            if (Screen.width == 0) return;
            if (layerTexture != null) layerTexture.Release();
            CreateRTCamera(mainCamera);
            mainCamera.cullingMask = layerMask;
            mainCamera.targetTexture = layerTexture;
        }

        private void RTSetup()
        {
            if (Screen.width == 0) return;
            if (layerTexture != null) layerTexture.Release();
            CreateRTCamera(mainCamera);

#if UNITY_EDITOR
            if (!WaterLayers.LayerExists(layerName))
            {
                WaterLayers.CreateLayer(layerName);
            }
#endif

            reflectionLayerIdx = Obstructor.GetLayerIdx(layerName);

            if (reflectionLayerIdx != -1)
            {
                int cmask = (1 << reflectionLayerIdx);
                mainCamera.cullingMask = cmask;
                mainCamera.targetTexture = layerTexture;
                RemoveLayerFromMainCamera();
            }
        }

        public void Loop()
        {
            RemoveLayerFromMainCamera();
            FollowCamera();
            UpdateCameraSize();
        }

        private void UpdateCameraSize()
        {

            if (Mathf.Abs( mainCamera.orthographicSize - CameraRenderingScene.orthographicSize * scale.y ) > 0.01f) { lastOrtographicSize = mainCamera.orthographicSize = CameraRenderingScene.orthographicSize * scale.y; CreateRT(sr, CameraRenderingScene, rendererType);  }
            if (Mathf.Abs( mainCamera.aspect - CameraRenderingScene.aspect * (scale.x / scale.y)) > 0.01f) { lastAspectRatio = mainCamera.aspect = CameraRenderingScene.aspect * (scale.x / scale.y); CreateRT(sr, CameraRenderingScene, rendererType);  }
         
 
        }

        private void RemoveLayerFromMainCamera() 
        {
            if (follow == null) return;
            if ((follow.GetComponent<Camera>().cullingMask & (1 << reflectionLayerIdx)) != 0) follow.GetComponent<Camera>().cullingMask &= ~(1 << reflectionLayerIdx);
        }

        private void FollowCamera()
        {
            holder.position = follow.position;
            holder.rotation = follow.rotation;
            holder.SetGlobalScale(follow.lossyScale);
        }

        public void Release()
        {
            mainCamera.targetTexture = null;
            layerTexture.Release();
        }
    }

}