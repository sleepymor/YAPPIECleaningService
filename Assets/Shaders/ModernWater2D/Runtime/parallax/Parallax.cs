using UnityEngine;

namespace Water2D
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Parallax : MonoBehaviour
    {
        SpriteRenderer _sr;
        SpriteRenderer sr 
        {
            get { if (_sr == null) { _sr = GetComponent<SpriteRenderer>(); _sr.sprite = Sprite.Create((Texture2D)Resources.Load("Sprites/textures/512x512"), new Rect(0, 0, 512, 512), new Vector2(0.5f, 0.5f)); } return _sr; }
            set { _sr = value; }
        }

        Camera _cam;
        Camera cam
        {
            get { if (_cam == null) _cam = Camera.main; return _cam; }
            set { _cam = value; }
        }

        [Space(10)]

        [SerializeField] int pixelsPerUnity = 16;

        [SerializeField] float offsetX = 0f;
        [SerializeField] float offsetY = 0f;

        [SerializeField] float width = 1f;
        [SerializeField] float height = 1f;

        [Space(10)]

        [SerializeField] bool useSpeed = true;
        [SerializeField] float speed;

        [SerializeField] float minX;
        [SerializeField] float maxX;
        [SerializeField] Transform target;
        [SerializeField] Gradient colorOverY;
        [SerializeField] Texture2D spriteTexture;

        [Space(10)]

        [SerializeField] [Range(0f, 20f)] float gamma;
        [SerializeField] [Range(0f, 1f)] float hardness;
        [SerializeField] [Range(0f, 0.01f)] float area;
        [SerializeField] [Range(0f, 3f)] float ratio;
        [SerializeField] [Range(2,16)] int quality;


        private void OnEnable()
        {
            SetupSprite();
            SetupShader();
            SetTransform();
        }

        void Start()
        {
            SetupSprite();
            SetupShader();
        }

        void Update()
        {
            SetTransform();
            GetSpriteMateral().SetFloat("_targetY", target.transform.position.x);
        
        
        }

        Material GetSpriteMateral() 
        {
            if(sr.sharedMaterial == null || sr.sharedMaterial.name != ("parallax : " + name))
            {
                sr.sharedMaterial = new Material(Shader.Find("Shader Graphs/parallax"));
                sr.sharedMaterial.name = "parallax : " + name; 
            }
            return sr.sharedMaterial;
        }

        void SetupSprite()
        {
            if (sr.sprite == null) throw new System.NullReferenceException("Parallax.cs : sprite is not set");
            sr.ResizeSpriteToScreen(cam, width, height);
        }

        void SetupShader() 
        {
            if (spriteTexture == null) return;

            Material mat = GetSpriteMateral();
            mat.SetFloat("_w", (cam.orthographicSize * cam.aspect * 2) / ((float)spriteTexture.width / (float)pixelsPerUnity));
            mat.SetInt("_useSpeed", useSpeed ? 1 : 0);
            mat.SetFloat("_speed", speed);
            mat.SetFloat("_minY", minX);
            mat.SetFloat("_maxY", maxX);
            mat.SetFloat("_targetY", target.transform.position.x);
            mat.SetTexture("_colorOverY", colorOverY.To1DTexture(256));
            mat.SetTexture("_MainTex2", spriteTexture);

            mat.SetInt("_quality", quality);
            mat.SetFloat("_ratio", ratio);
            mat.SetFloat("_area", area);
            mat.SetFloat("_hardness", hardness);
            mat.SetFloat("_gamma", gamma);

        }

        void SetTransform() 
        {
            transform.position = cam.transform.position + new Vector3(offsetX, offsetY, 5);
            transform.rotation = cam.transform.rotation;
        }
    }

    public static class GradientFunctions
    {
        //Romano from UnityForums
        public static Texture2D To1DTexture(this Gradient grad, int resolution)
        {
            Texture2D tex = new Texture2D(resolution,1,TextureFormat.ARGB32, false);
            for (int i = 0; i < resolution; i++) tex.SetPixel(i, 0, grad.Evaluate(i / (float)resolution));
            tex.Apply();
            return tex;
        }
    }

    public static class SpriteFunctions
    {
        //Romano from UnityForums
        public static void ResizeSpriteToScreen(this SpriteRenderer sprite, Camera theCamera, float fitToScreenWidth,  float fitToScreenHeight)
        {
            var sr = sprite;
            if (sr == null) return;

            sprite.transform.localScale = Vector3.one;

            float width = sr.sprite.bounds.size.x;
            float height = sr.sprite.bounds.size.y;

            float worldScreenHeight = theCamera.orthographicSize * 2.0f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            if (fitToScreenWidth == 0) fitToScreenWidth = 0.00001f;
            if (fitToScreenHeight == 0) fitToScreenHeight = 0.00001f;
            sprite.transform.SetGlobalScale(new Vector2(worldScreenWidth / width * fitToScreenWidth, worldScreenHeight / height * fitToScreenHeight));
        }
    }
}