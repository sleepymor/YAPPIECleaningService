using System;

namespace Water2D
{

    [Serializable]
    public class ModernWater2DSettings 
    {
        public ObstructorSettings _obstructorSettings = new ObstructorSettings();
        public ReflectionsSettings _reflectionsSettings = new ReflectionsSettings();
        public WaterSettings _waterSettings = new WaterSettings();
        public SimulationSettings _simulationSettings = new SimulationSettings();
        public WaveSimulationSettings _wavesSettings = new WaveSimulationSettings();
        public BlurSettings _blurSettings = new BlurSettings();
    }

}
