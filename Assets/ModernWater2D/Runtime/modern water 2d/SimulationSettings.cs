using System;
using UnityEngine;
using UnityEngine.Events;

namespace Water2D
{
    [Serializable]
    public class SimulationSettings
    {
        public WaterCryo<Vector2Int> resolution = new WaterCryo<Vector2Int>(new Vector2Int(1024, 1024));
        public ComputeShader waterCmp;
        public SpriteRenderer sr;
        public RenderTexture obstruction;
        public Camera mainCam;
        public WaterCryo<int> chunksX;
        public WaterCryo<int> chunksY;
        public WaterCryo<float> waveRad;
        public WaterCryo<float> waveHeight = new WaterCryo<float>(0.5f);
        public WaterCryo<float> dispersion = new WaterCryo<float>(0.98f);
        public WaterCryo<int> iterations = new WaterCryo<int>(4);
        public WaterCryo<float> simulationSpeed = new WaterCryo<float>(1f);

        public WaterCryo<bool> enableRain = new WaterCryo<bool>(false);
        public WaterCryo<float> rainWaveHeight = new WaterCryo<float>(0.4f);
        public WaterCryo<float> rainSpeed = new WaterCryo<float>(2.3f);
        public WaterCryo<int> rainSizeX = new WaterCryo<int>(2);
        public WaterCryo<int> rainSizeY = new WaterCryo<int>(2);
        public WaterCryo<float> normalStrength = new WaterCryo<float>(0.07f);

        public WaterCryo<Color> waveColor = new WaterCryo<Color>(Color.white);
        public WaterCryo<Vector2> waveColorMinMaxHeight = new WaterCryo<Vector2>(new Vector2(0f, 0.07f));

        internal void onValueChanged(UnityAction onOSimulationChanged)
        {
            simulationSpeed.onValueChanged = onOSimulationChanged;
            waveRad.onValueChanged = onOSimulationChanged;
            dispersion.onValueChanged = onOSimulationChanged;
            waveHeight.onValueChanged = onOSimulationChanged;
            iterations.onValueChanged = onOSimulationChanged;
            enableRain.onValueChanged = onOSimulationChanged;
            rainWaveHeight.onValueChanged = onOSimulationChanged;
            rainSpeed.onValueChanged = onOSimulationChanged;
            rainSizeX.onValueChanged = onOSimulationChanged;
            rainSizeY.onValueChanged = onOSimulationChanged;
            normalStrength.onValueChanged = onOSimulationChanged;
            waveColorMinMaxHeight.onValueChanged = onOSimulationChanged;
            waveColor.onValueChanged = onOSimulationChanged;
            chunksX.onValueChanged = onOSimulationChanged;
            chunksY.onValueChanged = onOSimulationChanged;
        }
    }
}