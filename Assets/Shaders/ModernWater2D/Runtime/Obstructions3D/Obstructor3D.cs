using UnityEngine;

namespace Water2D
{
    [ExecuteInEditMode]
    public class Obstructor3D : MonoBehaviour
    {
        [SerializeField][HideInInspector] Obstructor3DSO _data;
        [SerializeField][HideInInspector] public WaterCryo<float> height = new WaterCryo<float>(0.03f);


        [HideInInspector] public Obstructor3DSO data
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
       
            Transform obstructor = (_data != null && _data.obstructor != null) ? _data.obstructor : new GameObject("obstructor : " + name).transform;
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

            if (!ObstructorManager.GetInstance().obstructionObjectsVisible.value) { obstructor.gameObject.hideFlags = HideFlags.HideInHierarchy; }

            MeshRenderer sourceMr = GetComponent<MeshRenderer>();
            MeshFilter sourceMf = GetComponent<MeshFilter>();



            if (obstructor.GetComponent<MeshRenderer>() == null) obstructor.gameObject.AddComponent<MeshRenderer>();
            if (obstructor.GetComponent<MeshFilter>() == null) obstructor.gameObject.AddComponent<MeshFilter>();
            MeshRenderer obstructorMr = obstructor.GetComponent<MeshRenderer>();
            MeshFilter obstructorMf = obstructor.GetComponent<MeshFilter>(); ;


            obstructorMr.sharedMaterial = ObstructorManager.instance.red3d;
            obstructorMf.sharedMesh = sourceMf.sharedMesh ;


            Obstructor3DSO SO = new Obstructor3DSO(source,obstructor,sourceMr,obstructorMr,0.01f, -obstructorMf.sharedMesh.bounds.extents.y);
            _data = SO;

            UpdateMaterialData();
        }





        public void Destroy()
        {
   
            if (_data != null && _data.obstructor != null) DestroyImmediate(_data.obstructor.gameObject);
            DestroyImmediate(this);
        }


        private bool IsValid()
        {
  
            if (_data.obstructorRenderer == null) return false;
            if (_data.obstructor == null) return false;
            if (_data.source == null) return false;
            if (_data.sourceRenderer == null) return false;

            return true;

        }



        private void OnEnable()
        {
            data.obstructor.gameObject.layer = Obstructor.GetLayerIdx(ObstructorManager.rlayer);
             UpdateMaterialData(); // material data resets after building, so this is needed
        }

        private void UpdateMaterialData()
        {
            if (data == null) return;
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            data.obstructorRenderer.GetPropertyBlock(prop);
            prop.SetFloat("_size", 0.01f);
            float a = data.obstructor.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y;
            prop.SetFloat("_maxY", Mathf.Lerp(-a, a, height.value));
            data.obstructorRenderer.SetPropertyBlock(prop);
        }

        protected void OnDestroy()
        {
            if (!Application.isPlaying)
            {
              
                if (_data != null && _data.obstructor != null) DestroyImmediate(_data.obstructor.gameObject);
            }
        }

        public static int GetLayerIdx(string layer)
        {
            return LayerMask.NameToLayer(layer);
        }
    }

}