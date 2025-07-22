using UnityEngine;

namespace Water2D
{

    public class WaterSimulation
    {
        public virtual void Setup(SimulationSettings value) { }
        public virtual void UpdateSettings(SimulationSettings value) { }
        public virtual void UpdLoop() { }
        public virtual void Loop() { }
        public virtual void OnGizmos() { }
        public virtual RenderTexture GetRT() => null;
    }
}