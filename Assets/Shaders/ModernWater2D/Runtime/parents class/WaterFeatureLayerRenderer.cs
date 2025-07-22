#if UNITY_EDITOR
#endif
using System;
using UnityEngine;

namespace Water2D
{
    /// <summary>
    /// This class is used for water feature managers 
    /// Invidual water sources can connect to the manager and set the "run" variable to true
    /// Even If only one water source requires this manager to run, this manager will run
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    [Serializable]
    public abstract class WaterFeatureLayerRenderer : MonoBehaviour
    {
        [HideInInspector][SerializeField] protected LayerRenderer _layerRenderer;
        [HideInInspector][SerializeField] bool _run;
        [HideInInspector]
        [SerializeField]
        public bool run
        {
            get { return _run; }
            set
            {
                _layerRenderer.run = value;
                if (value) runTrue++;
                if (!value) runFalse++;
                _run = value;
            }
        }
        private int runTrue = 0;
        private int runFalse = 0;

        private void RunThisFrame()
        {
            int rt = runTrue;
            int rf = runFalse;
            runTrue = runFalse = 0;
            if (rt > 0) _run = true;
            else if (rt == 0 && rf > 0) _run = false;
        }

        protected virtual void Update()
        {
            RunThisFrame();
            _layerRenderer.run = run;
        }
    }

}