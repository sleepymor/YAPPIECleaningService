using System;
using System.Collections.Generic;
using UnityEngine;

namespace Water2D
{
    [Serializable]
    public class ReflectionSettings 
    {
        public List<string> layers;
        public WaterCryo<float> angle = new WaterCryo<float>(0f);
        public WaterCryo<float> tilt = new WaterCryo<float>(0f);
        public WaterCryo<float> length = new WaterCryo<float>(1f);
        public WaterCryo<float> originalColor = new WaterCryo<float>(0.7f);
        public WaterCryo<Color> color = new WaterCryo<Color>(Color.white);
        public WaterCryo<float> alpha = new WaterCryo<float>(1);
        public WaterCryo<float> y = new WaterCryo<float>(1);
    }

}
