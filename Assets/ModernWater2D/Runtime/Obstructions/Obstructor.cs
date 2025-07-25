using System;
using UnityEngine;

namespace Water2D
{

    [ExecuteInEditMode]
    public class Obstructor : MonoBehaviour
    {
        [SerializeField][HideInInspector] ObstructorSO _data;
        [SerializeField][HideInInspector][Range(0f, 1f)] public float height;
        [SerializeField][HideInInspector] SpriteRenderer sr;

        [SerializeField] [HideInInspector]
        public ObstructorSO data
        {
            get
            {
                if (_data == null || !IsValid()) CreateData();
                return _data;
            }
            set { _data = value; }
        }

        public void CreateData()
        {
            Transform obstructor = (_data != null && _data.child!=null ) ? _data.child : new GameObject("obstructor : " + name).transform;
            Transform source = transform;

#if UNITY_EDITOR
            if (!WaterLayers.LayerExists(ObstructorManager.rlayer)) WaterLayers.CreateLayer(ObstructorManager.rlayer);
#endif
            obstructor.gameObject.layer = Obstructor.GetLayerIdx(ObstructorManager.rlayer);

            obstructor.transform.parent = source;
            obstructor.transform.localPosition = Vector3.zero;
            obstructor.transform.localScale = Vector3.one;
            obstructor.rotation = source.rotation;

            // hides obstructors from scene view as the other camera renders them to another rtexture that is used in scene view anyway
            // (fixes duplicate sprites in scene view)
#if UNITY_EDITOR
            if (!ObstructorManager.GetInstance().obstructionObjectsVisible.value) UnityEditor.SceneVisibilityManager.instance.Hide(obstructor.gameObject, true);
#endif

            if (!ObstructorManager.GetInstance().obstructionObjectsVisible.value) { obstructor.gameObject.hideFlags  = HideFlags.HideInHierarchy; }

            SpriteRenderer sourceSr = GetComponent<SpriteRenderer>();
            SpriteRenderer obstructorSr;
            if(obstructor.GetComponent<SpriteRenderer>()==null) obstructor.gameObject.AddComponent<SpriteRenderer>();
            obstructorSr = obstructor.GetComponent<SpriteRenderer>();
            obstructorSr.color = sourceSr.color;
            obstructorSr.sortingLayerName = sourceSr.sortingLayerName;
            obstructorSr.sortingOrder = sourceSr.sortingOrder;
            obstructorSr.flipX = sourceSr.flipX;
            obstructorSr.flipY = sourceSr.flipY;
            obstructorSr.sprite = sourceSr.sprite;
            obstructorSr.material = ObstructorManager.instance.red;

            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            obstructorSr.GetPropertyBlock(prop);
            prop.SetFloat("_h", height);

            float texH = obstructorSr.sprite.texture.height;

            prop.SetFloat("_ss", (obstructorSr.sprite.texture.height <= obstructorSr.sprite.rect.height ? 0 : 1));
            prop.SetFloat("_minY", obstructorSr.sprite.rect.yMin / texH);
            prop.SetFloat("_maxY", obstructorSr.sprite.rect.yMax / texH);


            obstructorSr.SetPropertyBlock(prop);

            ObstructorSO SO = new ObstructorSO(source,obstructor,sourceSr,obstructorSr);
            data = SO;
        }





        public void Destroy() 
        {
            ObstructorManager.instance.RemoveObstructor(transform);
            if (_data != null && _data.child != null) DestroyImmediate(_data.child.gameObject);
            DestroyImmediate(this);
        }


        private bool IsValid()
        {
            if (_data.childSr == null) return false;
            if (_data.child == null) return false;
            if (_data.source == null) return false;
            if (_data.sourceSr == null) return false;
            if (_data.childSr.sprite == null) return false;
            return true;

        }

        protected void Awake()
        {
            ObstructorManager.GetInstance().AddObstructor(data);
        }

        private void OnEnable()
        {
            data.child.gameObject.layer = Obstructor.GetLayerIdx(ObstructorManager.rlayer);
            if(!Application.isPlaying)
            {
                CreateData();
            }
            UpdateMaterialData(); // material data resets after building, so this is needed
        }

        private void UpdateMaterialData()
        {
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            data.childSr.GetPropertyBlock(prop);
            prop.SetFloat("_h", height);

            float texH = data.childSr.sprite.texture.height;

            prop.SetFloat("_ss", (data.childSr.sprite.texture.height <= data.childSr.sprite.rect.height ? 0 : 1));
            prop.SetFloat("_minY", data.childSr.sprite.rect.yMin / texH);
            prop.SetFloat("_maxY", data.childSr.sprite.rect.yMax / texH);


            data.childSr.SetPropertyBlock(prop);
        }

        protected void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                ObstructorManager.instance.RemoveObstructor(transform);
                if (_data != null && _data.child != null) DestroyImmediate(_data.child.gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            Gizmos.color = Color.red;
            float x1 = (sr.bounds.center.x - sr.bounds.extents.x);
            float x2 = (sr.bounds.center.x + sr.bounds.extents.x);
            float y = (sr.bounds.center.y - sr.bounds.extents.y + (2 * sr.bounds.extents.y * height));
            Gizmos.DrawLine(new Vector2(x1, y), new Vector2(x2, y));
        }

        public static int GetLayerIdx(string layer)
        {
            return  LayerMask.NameToLayer(layer);
        }
    }

}