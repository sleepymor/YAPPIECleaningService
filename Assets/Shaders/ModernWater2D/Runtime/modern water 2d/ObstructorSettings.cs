using System;
using UnityEngine;
using UnityEngine.Events;

namespace Water2D
{
    [Serializable]
    public class ObstructorSettings
    {
        public Camera mainCamera;
        public WaterCryo<float> textureResolution = new WaterCryo<float>(1);
        public WaterCryo<bool> overrideMainCamera = new WaterCryo<bool>(false);
        public WaterCryo<bool> obstructionObjectsVisible = new WaterCryo<bool>(false);
        public WaterCryo<bool> cameraVisible = new WaterCryo<bool>(true);

        internal void onValueChanged(UnityAction onObstructionChanged)
        {
            textureResolution.onValueChanged = onObstructionChanged;
            overrideMainCamera.onValueChanged = onObstructionChanged;
            obstructionObjectsVisible.onValueChanged = onObstructionChanged;
            cameraVisible.onValueChanged = onObstructionChanged;
        }
    }

}