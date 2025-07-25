Shader "Water2D/Simulations/process"
{
    Properties
    { 
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION; 
                float2 uv : TEXCOORD0; 
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0; 
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            uniform sampler2D _NState;
            uniform sampler2D _ObstructionTex;

            uniform float4 OstructionTexPos;
            uniform float4 OstructionTexRes;
            uniform float4 texPos;

            uniform float2 resolution;

            uniform float waveRad;
            uniform float dispersion;
            uniform float waveHeight;
            uniform float diffusionSize;
            uniform float rainWaveH;
            uniform float rainSpeed;
            uniform float timeFromStart;
            uniform float rainSizeX;
            uniform float rainSizeY;
            uniform float enableRain;
            uniform float animate;


            float hash12(float2 co) 
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            float2 GetObsUVs(float2 uv)
            {
                float2 res;

                float distX = OstructionTexPos.z - OstructionTexPos.x;
                float distY = OstructionTexPos.w - OstructionTexPos.y;

                float4 uvs;
                uvs.x =  (texPos.x - OstructionTexPos.x) / distX;
                uvs.y =  (texPos.y - OstructionTexPos.y) / distY;
                uvs.z =  (texPos.z - OstructionTexPos.x) / distX;
                uvs.w =  (texPos.w - OstructionTexPos.y) / distY;

                res.x = lerp(uvs.x,uvs.z,uv.x);
                res.y = lerp(uvs.y,uvs.w,uv.y);
    
                return res.xy;
            }

            float2x2 rot(float a)
            {
                float c = cos(a);
                float s = sin(a);
                return float2x2(c, s, -s, c);
            }

            float2 frag (v2f i) : SV_Target
            {
                //old h
                float2 data = tex2D(_NState,i.uv).xy;
                //new h

                float new_s = data.y;
                float org_s = data.x;
                float gather = 0.0;
                 float mp = 1.0;
                const float take = 0.20;
                const float takeCorner = 0.05;

                float sum = 0;
                float2 stepSize = float2(1.0/resolution.x,1.0/resolution.y) ;
                float2 stepSizeObs = float2(1.0 / OstructionTexRes.x, 1.0 / OstructionTexRes.y) * 1.5f ;

                sum += tex2D(_NState, (i.uv + float2(stepSize.x,0.0) ) ).x  ;
                sum += tex2D(_NState, i.uv + float2(-stepSize.x,0.0)  ).x  ;
                sum += tex2D(_NState, i.uv + float2(0.0,stepSize.y) ).x  ;
                sum += tex2D(_NState, i.uv + float2(0.0,-stepSize.y) ).x  ;
                sum *= take;

                sum += tex2D(_NState, i.uv + float2(stepSize.x,stepSize.y)  ).x* takeCorner;
                sum += tex2D(_NState, i.uv + float2(-stepSize.x,stepSize.y) ).x* takeCorner;
                sum += tex2D(_NState, i.uv + float2(stepSize.x,-stepSize.y) ).x* takeCorner;
                sum += tex2D(_NState, i.uv + float2(-stepSize.x,-stepSize.y)).x* takeCorner;

                sum -= org_s;

                //new height
                float newH = org_s * 2 - new_s + sum;

                //dispersion
                newH *= dispersion;
                i.uv *= OstructionTexRes;
                i.uv = floor(i.uv);
                i.uv /= OstructionTexRes;

                //add obstructors
                float2 obsUvs = GetObsUVs(i.uv);
                if(obsUvs.x > 0.0 && obsUvs.x < 1.0 && obsUvs.y > 0.0 && obsUvs.y < 1.0 )
                {
                     const int iter = 16; 
                     const float GoldenAngle = 2.399996 ; 
                    const float radius = 0.25;
        
                float obsC;
                float r = 1.;
                float2x2 G = rot(GoldenAngle);
                float2 offset = float2(radius, 0.);
        
                    for (int i = 0; i < iter; ++i)
                    {
                        r += 1. / r;
                        offset = mul(G,  offset);
                        mp *=  tex2D(_ObstructionTex, obsUvs + offset * (r - 1.) * stepSizeObs).x;
                    }
            

                    obsC = tex2D(_ObstructionTex, obsUvs);
                    newH -= obsC * mp * waveHeight;
 
    }

                if(enableRain)
                {
                    for(int x = -rainSizeX; x < rainSizeX+1; x++)
                    {
                        for(int y = -rainSizeY; y < rainSizeY+1; y++)
                        {
                            int rand = floor( hash12( frac(timeFromStart/2) * (i.uv + ( float2(x,y) * stepSize)  ) ).x * 10000 / rainSpeed );
                            if(rand==1){newH -= rainWaveH; }
                        }
                    }
                }

    //return obsUvs.xy;
    return float2(newH, org_s);
    //return float2(tex2D(_ObstructionTex, obsUvs + float2(0.005,0.005)).x, tex2D(_ObstructionTex, obsUvs).x);
}           

            ENDCG
        }
    }
}