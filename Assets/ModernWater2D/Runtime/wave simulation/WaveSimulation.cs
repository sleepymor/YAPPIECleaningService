
using System;
using UnityEngine;

namespace Water2D
{
    [Serializable]
    public class WaveSimulation
    {
      

        [SerializeField][HideInInspector] GameObject WaterGO;
        [SerializeField][HideInInspector] SpriteRenderer WaterSr;

        [SerializeField][HideInInspector] BoxCollider2D collider;
        [SerializeField][HideInInspector] BuoyancyEffector2D buoyancy;
        [SerializeField][HideInInspector] WaveSimulationSettings settings;

        [SerializeField][HideInInspector] Texture2D heights;
        [SerializeField][HideInInspector] WaveSimNode[] nodes;

        float[] ld;
        float[] rd;

        private int res => settings.wavePoints.value;

        private void CreateT()
        {
            heights = new Texture2D(res, 1, TextureFormat.RFloat, 0, false);
        }

        private void SetNodes()
        {
            nodes = new WaveSimNode[res];
            for (int i = 0; i < res; i++)
            {
                nodes[i] = new WaveSimNode();
                nodes[i].vel = 0;
                nodes[i].force = 0;
                nodes[i].h = (1f - settings.waveHeight.value);
                nodes[i].ht = (1f - settings.waveHeight.value);
            }
        }

        private void SetTextureData()
        {
            for (int i = 0; i < res; i++)
            {
                heights.SetPixel(i, 0, new Color(nodes[i].h, 0, 0));
            }
            heights.Apply();

        }

        private void CreateColliders()
        {
            if (WaterGO.GetComponent<BoxCollider2D>() == null)
            {
                collider = WaterGO.AddComponent<BoxCollider2D>();
            }
            else collider = WaterGO.GetComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.usedByEffector = true;

            if (WaterGO.GetComponent<BuoyancyEffector2D>() == null)
            {
                buoyancy = WaterGO.AddComponent<BuoyancyEffector2D>();
            }
            else   buoyancy = WaterGO.GetComponent<BuoyancyEffector2D>();
        }

        public void SetSettings(GameObject go, SpriteRenderer water, WaveSimulationSettings settings)
        {
            this.settings = settings;
            this.WaterGO = go;
            this.WaterSr = water;
        }

        public void Setup()
        {
            CreateT();
            SetNodes();
            SetTextureData();
            SetTexture();
            if (settings.enableBuoyancy.value) CreateColliders();
            ld = new float[res];
            rd = new float[res];
        }

        private void AutoWaves()
        {
            for (int i = 0; i < res; i++)
            {
                nodes[i].h = (1f - settings.waveHeight.value) + (settings.waveHeight.value * (Mathf.Sin(settings.waveDensity2.value* (i/(float)res) + settings.waveDensity.value * Time.timeSinceLevelLoad )));
            }
        }

        private void SimulateSingle()
        {
            for (int i = 0; i < res; i++)
            {
                float y = nodes[i].h - nodes[i].ht;
                nodes[i].force = (-settings.stringStiffness.value * y)  ;
                nodes[i].vel += nodes[i].force;
                nodes[i].vel *= settings.stringDampening.value;
                nodes[i].h = nodes[i].h + nodes[i].vel;

            }
        }


        private void SimulateTogether()
        {
            for (int j = 0; j < settings.simulationSteps.value; j++)
            for (int i = 0; i < res; i++)
            {
                if (i > 0)
                {
                    ld[i] = settings.stringSpread.value * (nodes[i].h - (nodes[i - 1].h));
                    nodes[i - 1].vel += ld[i];
                }
                if (i < res - 1)
                {
                    rd[i] = settings.stringSpread.value * (nodes[i].h - (nodes[i + 1].h));
                    nodes[i + 1].vel += rd[i];
                }
            }
        }

        private void SetTexture()
        {
            WaterSr.sharedMaterial.SetTexture("_wavesHeight",heights);
        } 

        private void SetLevel(float l)
        {

            buoyancy.surfaceLevel =  (1 - 2f*settings.waveHeight.value) * l;
            collider.size =new Vector2( collider.size.x ,3f*(1 - 2f*settings.waveHeight.value) * l);
            collider.offset =new Vector2(0 ,-0.5f*(1 - 2f*settings.waveHeight.value) * l);
        }

        public void Start()
        {
            SetTexture();
        }

        public void Update(float buy_lev) 
        {
            if (settings.enableBuoyancy.value) SetLevel(buy_lev);
            if (settings.automaticWaves.value) AutoWaves();
            else
            {
                SimulateSingle();
                SimulateTogether();
            }
            SetTextureData();
        }

        //t - (0:1) float
        public void Collision(Collider2D c, float t)
        {
            var rb = c.GetComponent<Rigidbody2D>();
            float fa = settings.splashForceMin.value;
            float fe = settings.splashForceMax.value;
            float ft = Mathf.Lerp(0f, 1f, (( 0.5f*rb.mass * rb.linearVelocity.y * rb.linearVelocity.y) - fa)/ (fe-fa) );
            
            float speed = Mathf.Lerp(settings.splashVelMin.value, settings.splashVelMax.value, ft);
            int nodesNum = (int)Mathf.Lerp(settings.splashNodesWidthMin.value, settings.splashNodesWidthMax.value, ft);
            int pos = Mathf.CeilToInt(t * res);


            for (int i = pos - nodesNum / 2; i < pos + nodesNum / 2; i++)
            {
                if(i<pos) nodes[i].vel = -speed * (Mathf.SmoothStep(0f,1f, (i-pos+(nodesNum/2)) / (float)(nodesNum/2f)  ));
                else nodes[i].vel = -speed * (Mathf.SmoothStep(1f, 0f, (i - pos ) / (float)(nodesNum/2f)));

            }
        }
    }
}


