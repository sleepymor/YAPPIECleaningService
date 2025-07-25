using UnityEngine;

namespace Water2D
{
    public static class WaterTransformUtils

    {
        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            if (transform.lossyScale.x == 0 || transform.lossyScale.y == 0 || transform.lossyScale.z == 0) return;
            transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }
    }

}