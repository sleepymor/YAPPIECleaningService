
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Water2D
{
    [Serializable]
    public class WaveSimulationSettings
    {
        public WaterCryo<bool> automaticWaves = new WaterCryo<bool>(true);
        public WaterCryo<bool> enableBuoyancy = new WaterCryo<bool>(true);
        public WaterCryo<bool> enableRigidbodyCollisions = new WaterCryo<bool>(true);
        
        public WaterCryo<int> wavePoints = new WaterCryo<int>(128);
        public WaterCryo<int> simulationSteps = new WaterCryo<int>(4);
        public WaterCryo<float> waveDensity = new WaterCryo<float>(1f);
        public WaterCryo<float> waveDensity2 = new WaterCryo<float>(1f);
        public WaterCryo<float> waveHeight = new WaterCryo<float>(0.1f);
        public WaterCryo<float> stringDampening = new WaterCryo<float>(0.03f);
        public WaterCryo<float> stringSpread = new WaterCryo<float>(0.006f);
        public WaterCryo<float> stringStiffness = new WaterCryo<float>(0.1f);

        public WaterCryo<float> edgeColoringSize = new WaterCryo<float>(0.0f);
        public WaterCryo<Color> edgeColor = new WaterCryo<Color>(Color.white);
        public WaterCryo<bool> edgeIgnoreTransparency = new WaterCryo<bool>(false);

        public WaterCryo<float> splashForceMin = new WaterCryo<float>(0);
        public WaterCryo<float> splashForceMax = new WaterCryo<float>(10f);
        public WaterCryo<float> splashVelMin = new WaterCryo<float>(0);
        public WaterCryo<float> splashVelMax = new WaterCryo<float>(1f);
        public WaterCryo<int> splashNodesWidthMin = new WaterCryo<int>(1);
        public WaterCryo<int> splashNodesWidthMax = new WaterCryo<int>(6);

        public void OnValueChanged(UnityAction action) 
        {
            edgeIgnoreTransparency.onValueChanged = action;
            edgeColoringSize.onValueChanged = action;
            edgeColor.onValueChanged = action;
            wavePoints.onValueChanged = action;
            automaticWaves.onValueChanged = action;
            enableBuoyancy.onValueChanged = action;
            enableRigidbodyCollisions.onValueChanged = action;
            waveDensity.onValueChanged = action;
            waveDensity2.onValueChanged = action;
            waveHeight.onValueChanged = action;
            stringDampening.onValueChanged = action;
            stringSpread.onValueChanged = action;
            stringStiffness.onValueChanged = action;
            splashForceMin.onValueChanged = action;
            splashForceMax.onValueChanged = action;
            splashVelMin.onValueChanged = action;
            splashVelMax.onValueChanged = action;
            splashNodesWidthMin.onValueChanged = action;
            splashNodesWidthMax.onValueChanged = action;
        }
    }
}


