using UnityEngine;

namespace Water2D
{

    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class SurfaceRenderingManager : WaterFeatureLayerRenderer
    {
        #region Singleton

        public static SurfaceRenderingManager instance;

        private void Awake()
        {
            Singleton();
        }

        protected override void Update()
        {
            base.Update();
        }

        void Singleton()
        {
            if (instance == null) { instance = this; }
            else if (this == instance) return;
            else DestroyImmediate(gameObject);
        }

        #endregion

        public LayerRenderer layerRenderer
        {
            get
            {
                if (_layerRenderer == null)
                {
                    int cullingMask = Camera.main.cullingMask;
                    cullingMask &= ~(1 << Obstructor.GetLayerIdx(ModernWater2D.srLayer));
                    cullingMask &= ~(1 << Obstructor.GetLayerIdx(ModernWater2D.sr2Layer));
                    _layerRenderer = new LayerRenderer();
                    layerRenderer.Setup(Camera.main, transform,  cullingMask,Vector2.one,1f);
                }
                return _layerRenderer;
            }
            set { _layerRenderer = value; }
        }



        public void SetupLayerRenderer(Camera cameraRenderingScreen)
        {
            int cullingMask = cameraRenderingScreen.cullingMask;
            cullingMask &= ~(1 << Obstructor.GetLayerIdx(ModernWater2D.srLayer));
            cullingMask &= ~(1 << Obstructor.GetLayerIdx(ModernWater2D.sr2Layer));
            layerRenderer.Setup(cameraRenderingScreen, transform, cullingMask, Vector2.one, 1f);
            Shader.SetGlobalTexture(WaterShaderIdsSUR.surfaceTexture, layerRenderer.LayerTexture());
        }

        private void OnDisable()
        {
            layerRenderer.Release();
        }
    }

}