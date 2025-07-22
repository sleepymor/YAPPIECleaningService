using UnityEngine;

namespace Water2D
{

    public class cameraManip : MonoBehaviour
    {

        [SerializeField] bool scrolling = false;
        [SerializeField] bool focus = false;

        [SerializeField] float scrollingSpeed = 0.1f;

        [SerializeField] Transform objF;
        [SerializeField] float baseFocus = 4f;

        float cachedSize;

        void Start()
        {
            cachedSize = Camera.main.orthographicSize;
        }


        void Update()
        {
            if (focus)
            {
                Camera.main.transform.position = objF.position + new Vector3(0, 0, -10);
                Camera.main.orthographicSize = cachedSize * baseFocus;

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    baseFocus /= 2;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    baseFocus *= 2;
                }
            }

            if (scrolling)
            {
                Camera.main.transform.position += new Vector3(scrollingSpeed * Time.deltaTime, 0, 0);
            }
        }
    }

}