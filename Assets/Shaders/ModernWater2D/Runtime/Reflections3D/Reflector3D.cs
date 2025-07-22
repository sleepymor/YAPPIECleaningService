using System;
using UnityEngine;


namespace Water2D
{
    [ExecuteInEditMode]
    [Serializable]
    public class Reflector3D : MonoBehaviour
    {
        [SerializeField]public WaterCryo<Vector3> displacement = new WaterCryo<Vector3>(new Vector3(0, 0, 0));
        [SerializeField]public WaterCryo<Vector3> rotation = new WaterCryo<Vector3>(new Vector3(0, 0, 0));
        [HideInInspector][SerializeField] Reflection3DSO _data;

        [HideInInspector]
        public Reflection3DSO data
        {
            get
            {
                if (_data == null || !IsValid()) CreateData();
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
            displacement.onValueChanged = CreateData;
            rotation.onValueChanged = CreateData;
        }

        private void Start()
        {
            data.reflection.gameObject.layer = Obstructor.GetLayerIdx(ReflectionsSystem.rlayer);
        }

        public void CreateData()
        {
            bool reuseData = (_data != null && _data.reflection != null);
            Transform reflection = (reuseData) ? _data.reflection : new GameObject("reflection : " + name).transform;

#if UNITY_EDITOR
            if (!ReflectionsSystem.GetInstanceTopDown().reflectionObjectsVisible.value) UnityEditor.SceneVisibilityManager.instance.Hide(reflection.gameObject, true);
#endif

            Transform source = transform;

#if UNITY_EDITOR
            if (!WaterLayers.LayerExists(ReflectionsSystem.rlayer)) WaterLayers.CreateLayer(ReflectionsSystem.rlayer);
#endif

            MeshFilter mf;
            MeshRenderer mr;
            if (!reuseData)
            {
                reflection.parent = transform;
                mf = reflection.gameObject.AddComponent<MeshFilter>();
                mr = reflection.gameObject.AddComponent<MeshRenderer>();
            }
            else
            {
                mf = reflection.GetComponent<MeshFilter>();
                mr = reflection.GetComponent<MeshRenderer>();
            }

            mf.sharedMesh = source.GetComponent<MeshFilter>().sharedMesh;
            mr.sharedMaterial = source.GetComponent<MeshRenderer>().sharedMaterial;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

            reflection.gameObject.layer = Obstructor.GetLayerIdx(ReflectionsSystem.rlayer);
            reflection.localPosition = Vector3.zero;
            reflection.localScale = Vector3.one;
            reflection.localRotation = Quaternion.identity;
            reflection.localRotation = Quaternion.Euler(0, 180, 180);
            reflection.localRotation *= Quaternion.Euler(rotation.value.x, rotation.value.y, rotation.value.z);
            
            reflection.localPosition -= new Vector3(0,0 ,0);
            reflection.localPosition +=displacement.value;

            _data.reflection = reflection;
            _data.source = transform;
            _data.displacement = displacement.value ;
        }

        private bool IsValid()
        {
            if (_data.reflection == null) return false;
            if (_data.source == null) return false;
            return true;

        }

        public void DeleteData()
        {
            if (_data == null) { DestroyPlus(this); return; }
            if (_data.reflection) DestroyImmediate(_data.reflection.gameObject);
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



        protected void OnDestroy()
        {
            if (!gameObject.scene.isLoaded) return;

            if (!Application.isPlaying)
            {
                DeleteData();
            }
        }

     
    }

}