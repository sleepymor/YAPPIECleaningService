using System;
using UnityEngine.Events;

namespace Water2D
{
    [Serializable]
    public class BlurSettings
    {
        public enum BlurType
        {
            box = 0,
            gaussian = 1,
            bokeh = 2
        }

        public BlurType blurType = BlurType.gaussian;

        public WaterCryo<bool> useBlur = new WaterCryo<bool>(false);

        public WaterCryo<int> boxSamplingRange = new WaterCryo<int>(8);
        public WaterCryo<float> boxStrength = new WaterCryo<float>(1f);

        public WaterCryo<int> gaussianSamplingRange = new WaterCryo<int>(8);
        public WaterCryo<float> gaussianStrengthX = new WaterCryo<float>(2f);

        public WaterCryo<float> bokehArea = new WaterCryo<float>(0.003f);
        public WaterCryo<int> bokehQuality = new WaterCryo<int>(8);
        public WaterCryo<float> bokehGamma = new WaterCryo<float>(10.5f);
        public WaterCryo<float> bokehHardness = new WaterCryo<float>(0.3f);
        public WaterCryo<float> bokehRatio = new WaterCryo<float>(1.5f);

        public WaterCryo<bool> useFalloff = new WaterCryo<bool>(false);
        public WaterCryo<float> falloffStart = new WaterCryo<float>(0);
        public WaterCryo<float> falloffEnd = new WaterCryo<float>(1);
        public WaterCryo<float> falloffStrength = new WaterCryo<float>(1);

        internal void onValueChanged(UnityAction onOSimulationChanged)
        {
            useBlur.onValueChanged = onOSimulationChanged;
            boxSamplingRange.onValueChanged = onOSimulationChanged;
            gaussianSamplingRange.onValueChanged = onOSimulationChanged;
            gaussianStrengthX.onValueChanged = onOSimulationChanged;
            bokehGamma.onValueChanged = onOSimulationChanged;
            bokehHardness.onValueChanged = onOSimulationChanged;
            bokehArea.onValueChanged = onOSimulationChanged;
            bokehRatio.onValueChanged = onOSimulationChanged;
            bokehQuality.onValueChanged = onOSimulationChanged;
            useFalloff.onValueChanged = onOSimulationChanged;
            falloffStart.onValueChanged = onOSimulationChanged;
            falloffEnd.onValueChanged = onOSimulationChanged;
            falloffStrength.onValueChanged = onOSimulationChanged;
            boxStrength.onValueChanged = onOSimulationChanged;
        }
    }
}