using UnityEngine;

namespace Water2D
{

    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class WaterSortingLayerSetter : MonoBehaviour
    {
        [SerializeField] string layer;
        [SerializeField] int order;

        private void OnEnable()
        {
            SRSetup();
        }


        private int GetSortingOrder(string layer) 
        {
            if (layer == "default" && GetComponent<SpriteRenderer>().sortingOrder < 5 ) return 0;
            if (layer == "Reflection_Plane") return 5;
            if (layer == "plat") return 10;
            if (layer == "Objects") return 15;
            if (layer == "Reflection_Plane2") return 20;
            if (layer == "droplets") return 25;
            return 0;
        }

        private void SRSetup()
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "default";
            GetComponent<SpriteRenderer>().sortingOrder = GetSortingOrder(layer);
        }

    }

}