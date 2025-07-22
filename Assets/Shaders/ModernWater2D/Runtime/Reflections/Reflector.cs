using System;
using System.Collections.Generic;
using UnityEngine;


namespace Water2D
{
    [ExecuteInEditMode]
    [Serializable]
    public class Reflector : MonoBehaviour
    {

        [SerializeField] [HideInInspector] public ReflectionPivotSourceMode pivotSourceMode;
        [SerializeField] [HideInInspector] public WaterCryo<bool> flipX = new WaterCryo<bool>(false);
        [SerializeField] [HideInInspector] public Transform customPivot;

        [SerializeField][HideInInspector] public WaterCryo<bool > MSP_ReflectionGenerator = new WaterCryo<bool>(false);
        [SerializeField][HideInInspector] public WaterCryo<Vector2 > displacement = new WaterCryo<Vector2>(new Vector2(0,0));
        [SerializeField][HideInInspector] public WaterCryo<float > additionalTilt = new WaterCryo<float>(0);

        [SerializeField][HideInInspector] public bool raymarched = false;
        [SerializeField][HideInInspector] public int maxLength = 10000;
        [SerializeField][HideInInspector] [Range(0f, 1f)] public float fadeStrength = 0;
        [SerializeField][HideInInspector] public Dictionary<Sprite, Sprite> spritesDictionary = new Dictionary<Sprite, Sprite>();

        [HideInInspector] [SerializeField] ReflectionSO _data;

        ReflectionsSystem rsReference;

        [HideInInspector] public ReflectionSO data 
        {
            get 
            {
                if (_data == null || !IsValid() ) CreateData();
                return _data;
            }   
            set { _data = value; }  
        }

        private void OnEnable()
        {
            SetCallbacks();
        }

        public void SetCallbacks()
        {
            flipX.onValueChanged = SettingsChanged;
            additionalTilt.onValueChanged = SettingsChanged;
            displacement.onValueChanged = SettingsChanged;
            MSP_ReflectionGenerator.onValueChanged = AlgorithmChanged;
        }

        private void AlgorithmChanged() 
        {
            data.MSP_ReflectionGenerator = MSP_ReflectionGenerator.value;
            CreateData();
        }

        private void SettingsChanged() 
        {
            data.additionalTilt = additionalTilt.value;
            data.displacement = displacement.value;
            data.flipX = flipX.value;
            data.MSP_ReflectionGenerator = MSP_ReflectionGenerator.value;

            if (ReflectionsSystem.instanceTopDown != null)
            {
                ReflectionsSystem.update_extended = true;
                ReflectionsSystem.instanceTopDown.AddReflector(data);
            }

        }


        Sprite GetRaymarchTexture(Sprite original)
        {

            Texture2D raymarchedTexture = new Texture2D(original.texture.width, original.texture.height*2);

            Color[] colors = original.texture.GetPixels();
            Color[] newColors = new Color[colors.Length*2];

            int w1 = original.texture.width;
            int w2 = original.texture.width;
            int h1 = original.texture.height;
            int h2 = original.texture.height*2;

          

            for (int x = 0; x < w1; x++)
                for (int y = 0; y < h1; y++) newColors[x + w1 * y] = Color.clear;
            for (int x = 0; x < w1; x++)
                for (int y = h1; y < h2; y++) newColors[x + w1 * y] = colors[x + w1* (y-h1)];


            for (int x = 0; x < w1; x++)
                for (int y = 0; y < h2; y++)
                    if (newColors[x + w1 * y].a < 0.1f)
                    {
                        for (int y1 = y+1; y1 < h2; y1++)
                        {
                            if (newColors[x + w1 * y1].a > 0.1f)
                            {
                                if((y1 + (y1 - y)) < h2 && (y1-y) <= maxLength) newColors[x + w1 * y] = newColors[x + w1 * (y1 + (y1-y))];
                                break;
                            }
                        }
                    }

            for (int x = 0; x < w1; x++)
                for (int y = h1; y < h2; y++) if (colors[x + w1 * (y-h1)].a  > 0.1f) newColors[x + w1 * y] = Color.clear;

            raymarchedTexture.SetPixels(newColors);
            raymarchedTexture.Apply();

       

            Sprite newSprite = Sprite.Create(raymarchedTexture, new Rect(0,0, raymarchedTexture.width, raymarchedTexture.height), original.pivot / new Vector2(raymarchedTexture.width, original.texture.height), original.pixelsPerUnit);
            spritesDictionary[original] = newSprite;
            newSprite.name = original.name + "raymarched";





            return spritesDictionary[original];
        }


        private void Start()
        {
            data.reflection.gameObject.layer = Obstructor.GetLayerIdx(ReflectionsSystem.rlayer);
        }

        public void CreateData()
        {
            bool reuseData = (_data != null && _data.reflection != null);
            Transform reflectionPivot = (reuseData) ? _data.reflectionPivot : new GameObject("reflection_pivot : " + name).transform;
            Transform reflection = (reuseData) ? _data.reflection : new GameObject("reflection : " + name).transform;

            // hides reflections from scene view as the other camera renders them to another rtexture that is used in scene view anyway
            // (fixes duplicate sprites in scene view)
#if UNITY_EDITOR
            if (!ReflectionsSystem.GetInstanceTopDown().reflectionObjectsVisible.value) UnityEditor.SceneVisibilityManager.instance.Hide(reflection.gameObject,true);
            if (!ReflectionsSystem.GetInstanceTopDown().reflectionObjectsVisible.value) UnityEditor.SceneVisibilityManager.instance.Hide(reflectionPivot.gameObject, true);   
#endif

            Transform source = transform;

            if (!ReflectionsSystem.GetInstanceTopDown().reflectionObjectsVisible.value) { reflectionPivot.gameObject.hideFlags =  reflection.gameObject.hideFlags = HideFlags.HideInHierarchy; }

#if UNITY_EDITOR
            if (!WaterLayers.LayerExists(ReflectionsSystem.rlayer)) WaterLayers.CreateLayer(ReflectionsSystem.rlayer);
#endif
            reflection.gameObject.layer = Obstructor.GetLayerIdx(ReflectionsSystem.rlayer);


            if(!reuseData) reflectionPivot.parent = transform;
            if(!reuseData) reflection.parent = reflectionPivot;

            reflectionPivot.localScale = Vector3.one;

            SpriteRenderer sourceSr = GetComponent<SpriteRenderer>();
            SpriteRenderer reflectionSr = reuseData ? reflection.GetComponent<SpriteRenderer>() : reflection.gameObject.AddComponent<SpriteRenderer>();
            reflectionSr.color = sourceSr.color;
            reflectionSr.material = ReflectionsSystem.instanceTopDown.reflectorMat;
            reflectionSr.sharedMaterial = ReflectionsSystem.instanceTopDown.reflectorMat;
            reflectionSr.sortingLayerName = sourceSr.sortingLayerName;
            reflectionSr.sortingOrder = sourceSr.sortingOrder;
            reflectionSr.flipX = sourceSr.flipX;
            reflectionSr.flipY = !raymarched ? sourceSr.flipY : !sourceSr.flipY;
            reflectionSr.sprite = !raymarched ? (MSP_ReflectionGenerator.value ? CreateMSPSprite(sourceSr.sprite) : sourceSr.sprite) : GetRaymarchTexture(sourceSr.sprite);
            ReflectionSO reflectionSO = new ReflectionSO(source, reflectionPivot, pivotSourceMode, reflection, sourceSr, reflectionSr, flipX.value, displacement.value, MSP_ReflectionGenerator.value, additionalTilt.value,raymarched);
            reflectionSO.customPivot = customPivot;
            data = reflectionSO;
            data.raymarched = raymarched;

            ReflectionsSystem.instanceTopDown.AddReflector(data);
        }

        private Sprite CreateMSPSprite(Sprite org)
        {
            //create new texture
            Texture2D newTexture = new Texture2D(org.texture.width, org.texture.height, TextureFormat.ARGB32, false);
            newTexture.filterMode = org.texture.filterMode;
            newTexture.Apply();
            Sprite cpy = Sprite.Create(newTexture, org.rect, new Vector2(org.pivot.x / org.texture.width, org.pivot.y / org.texture.height), org.pixelsPerUnit);


            //get y0 for draging pixels
            int y0 = 0;
            Color32[] colors = org.texture.GetPixels32();
            int width = org.texture.width;
            for (int i = 0; i < colors.Length; i++)
                if (colors[i].a > 100)
                {
                    y0 =  (i / width);
                    break;
                }
          
            //grab pixels to y0
            for (int x = 0; x < org.texture.width; x++) {
                bool started = false;
                int c = y0;
                for (int y = y0; y < org.texture.height; y++)
                {
                    int idx = x + (org.texture.width * y);
                    if (colors[idx].a > 100) started = true;
                    if (started)
                    {
                        colors[x + (org.texture.width*c) ] = colors[idx];
                        colors[idx] = Color.clear;
                        c++;
                    }
                }
            }

            //apply changes
            cpy.texture.SetPixels32(colors);
            cpy.texture.Apply();
            return cpy;
        }

        private bool IsValid() 
        {
            if (_data.reflection == null) return false;
            if (_data.reflectionPivot == null) return false;
            if (_data.reflectionSr == null) return false;
            if (_data.source == null) return false;
            if (_data.sourceSr == null) return false;
            return true;
            
        }

        public void DeleteData()
        {
            if (_data == null) { DestroyPlus(this); return; } 
            if(_data.reflection) DestroyImmediate(_data.reflection.gameObject);
            if(_data.reflectionPivot) DestroyImmediate(_data.reflectionPivot.gameObject);
            _data = null;
        }

        public void UpdateData() 
        {
            CreateData();
        }

        void DestroyPlus(UnityEngine.Object obj) 
        {
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }
        
        protected void Awake()
        {
            if (getRF() == null) return;
            getRF().AddReflector(data);
        }

        protected void OnDisable()
        {
            //getRF().RemoveReflector(data);
            //DeleteData();
        }

        protected void OnDestroy()
        {
            if (!gameObject.scene.isLoaded) return;

            if (!Application.isPlaying)
            { 
                getRF().RemoveReflector(data);
                DeleteData();
            }
        }

        private ReflectionsSystem getRF() 
        {
            if (rsReference == null) rsReference = ReflectionsSystem.GetInstanceTopDown();
            return rsReference;
        }
    }

}