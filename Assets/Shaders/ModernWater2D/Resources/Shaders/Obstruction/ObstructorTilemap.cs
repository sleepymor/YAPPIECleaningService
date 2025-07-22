using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Water2D
{

    [RequireComponent(typeof(TilemapRenderer))]
    [RequireComponent(typeof(Tilemap))]
    [ExecuteInEditMode]
    [Serializable]
    public class ObstructorTilemap : MonoBehaviour
    {
        [SerializeField][HideInInspector][Range(0f, 1f)] public float height;
        [SerializeField][HideInInspector] public GameObject obstructor;


        public void CreateData()
        {
            Transform obstructor = (this.obstructor != null) ? this.obstructor.transform : new GameObject("obstructor : " + name).transform;
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

            TilemapRenderer sourceTR = GetComponent<TilemapRenderer>();
            Tilemap sourceT= GetComponent<Tilemap>();
          
            
            TilemapRenderer obstructorTR;
            Tilemap obstructorT;


            if (obstructor.GetComponent <Tilemap>() == null) obstructor.gameObject.AddComponent<Tilemap>();
            if (obstructor.GetComponent <TilemapRenderer>() == null) obstructor.gameObject.AddComponent<TilemapRenderer>();

            obstructorTR = obstructor.GetComponent<TilemapRenderer>();
            obstructorT = obstructor.GetComponent<Tilemap>();

            obstructorT.GetCopyOf(sourceT);
            obstructorTR.sharedMaterial = ObstructorManager.instance.red;

            var T = sourceT;


                for (int x = T.cellBounds.xMin; x < T.cellBounds.xMax; x++)
                {
                    for (int y = T.cellBounds.yMin; y < T.cellBounds.yMax; y++)
                    {
                        Vector3 Worldpos = T.CellToWorld(new Vector3Int(x, y, (int)T.transform.position.y));
                        Vector3Int TMPos3 = obstructorT.WorldToCell(Worldpos);

                        if (T.GetTile(TMPos3) != null)
                        {
                        
                            obstructorT.SetTile(TMPos3, T.GetTile(TMPos3));
                        }
                    }
                }

   

            this.obstructor = obstructor.gameObject;
        }





        public void Destroy()
        {
            DestroyImmediate(obstructor);
            DestroyImmediate(this);
        }

     
        private void OnEnable()
        {
            if (this.obstructor == null) CreateData();
            this.obstructor.gameObject.layer = Obstructor.GetLayerIdx(ObstructorManager.rlayer);
            if (!Application.isPlaying)
            {
                CreateData();
            }
        }

    

        protected void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                ObstructorManager.instance.RemoveObstructor(transform);
            }
        }

  

        public static int GetLayerIdx(string layer)
        {
            return LayerMask.NameToLayer(layer);
        }
    }

}
