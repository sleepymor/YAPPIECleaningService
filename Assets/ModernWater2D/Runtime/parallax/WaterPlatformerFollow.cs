using UnityEngine;

namespace Water2D
{
    [ExecuteAlways]
    public class WaterPlatformerFollow : MonoBehaviour
    {
        [SerializeField] Camera cam;
        [SerializeField] float x_offset;


        void Update() => Follow();


        void LateUpdate() => Follow();


        void Follow() 
        {
   
            transform.position = new Vector3(cam.transform.position.x + x_offset, transform.position.y, transform.position.z) ;
        }
    }
    

}