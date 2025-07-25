using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


namespace Water2D
{

#if UNITY_EDITOR

    [CustomEditor(typeof(Spawner))]
    public class spawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Spawner spawner = (Spawner)target;
            if (GUILayout.Button("Spawn")) spawner.Spawn();
            if (GUILayout.Button("Destroy")) spawner.DestroyC();
        }
    }

#endif

    public class Spawner : MonoBehaviour
    {
        [SerializeField] float areaX;
        [SerializeField] float areaY;

        [SerializeField] int num;
        [SerializeField] bool gizmos;

        [SerializeField] bool randomOffset;
        [SerializeField] Vector4 offsetMinMax;

        [SerializeField] List<GameObject> objects = new List<GameObject>();
        int c = 0;

        public void Spawn()
        {
            int sq = Mathf.FloorToInt(Mathf.Sqrt((float)num));

            float stepY = areaY / sq;
            float stepX = areaX / sq;

            areaX -= stepX / 2;
            areaY += stepY / 2;

            Vector2 start = new Vector2(-areaX / 2, areaY / 2);

            for (int i = 0; i < sq; i++)
            {
                start = new Vector2(start.x, areaY / 2);
                for (int j = 0; j < sq; j++)
                {
                    start -= new Vector2(0, stepY);
                    SpawnObj(start);
                }
                start += new Vector2(stepX, 0);
            }

            areaX += stepX / 2;
            areaY -= stepY / 2;
        }
        public void DestroyC()
        {
            foreach (var c in transform.GetComponentsInChildren<Transform>().ToArray()) if (c != transform && c.gameObject != null) DestroyImmediate(c.gameObject);
        }

        void SpawnObj(Vector2 pos)
        {
            if (objects.Count == 0) return;
            Instantiate(objects[Random.Range(0, objects.Count)], pos, Quaternion.identity, transform);
            c++;
        }

        private void OnDrawGizmos()
        {
            if (gizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(Vector3.zero, new Vector3(areaX, areaY));
            }
        }
    }

}