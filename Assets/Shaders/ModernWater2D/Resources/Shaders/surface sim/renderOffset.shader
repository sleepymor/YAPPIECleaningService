Shader "Water2D/Simulations/offset"
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
            
            uniform sampler2D _tex;
            uniform float2 delta;

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv += delta;
                return tex2D(_tex,i.uv);
            }

            ENDCG
        }
    }
}