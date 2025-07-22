//source code I was basing the shader graph custom functions on

#define PI 3.14159265

#define vec2 float2
#define vec3 float3
#define vec4 float4

float Falloff(float y, float y0, float y1, float p)
{
	if( y<y0) return 0;
	if( y>y1) return 1;
	
	float t = (y - y0) / (y1-y0);
	return lerp(0,1*p,t);
}


float gauss(float x, float y, float sigma)
{
    return  1.0f / (2.0f * PI * sigma * sigma) * exp(-(x * x + y * y) / (2.0f * sigma * sigma));
}

struct pixel_data
{
	sampler2D tex;
	float2 uv;
	float4 texelSize;
};

float4 BoxBlur(pixel_data pinfo, float samplingArea)
{
	float4 o = 0;
	float sum = 0;
	float2 uvOffset;
	float weight = 1;

	for(int x = - samplingArea / 2; x <= samplingArea / 2; ++x)
	{
		for(int y = - samplingArea / 2; y <= samplingArea / 2; ++y)
		{
			uvOffset = pinfo.uv;
			uvOffset.x += ((x) * pinfo.texelSize.x);
			uvOffset.y += ((y) * pinfo.texelSize.y);
			o += tex2D(pinfo.tex, uvOffset) * weight;
			sum += weight;
		}
	}
	o *= (1.0f / sum);
	return o;
}

float4 GaussianBlur(pixel_data pinfo, float samplingArea, float sigmaX)
{
	float4 o = 0;
	float sum = 0;
	float2 uvOffset;
	float weight;
	float difxy;
	float sigma;

	for(int x = - samplingArea / 2; x <= samplingArea / 2; ++x)
	{
		for(int y = - samplingArea / 2; y <= samplingArea / 2; ++y)
		{	
			uvOffset = pinfo.uv;
			uvOffset.x += ((x) * pinfo.texelSize.x);
			uvOffset.y += ((y) * pinfo.texelSize.y);

			weight = gauss(x,y, sigmaX );
			o += tex2D(pinfo.tex, uvOffset) * weight;
			sum += weight;
		}
	}
	o *= (1.0f / sum);

	return o;
}


float4 BokehBlur(sampler2D tex, float size, int res, vec2 uv, float ratio, float strength, float gamma)
{
	float div = 0.0;
    float3 accumulate = float3(0.0,0.0,0.0);
    
    for(int iy = 0; iy < res; iy++)
    {
        float y = (float(iy) / float(res))*2.0 - 1.0;
        for(int ix = 0; ix < res; ix++)
        {
            float x = (float(ix) / float(res))*2.0 - 1.0;
            float2 p = float2(x, y);
            float i = smoothstep(1.0, strength, distance(p, float2(0.0,0.0)));
            
            div += i;
            accumulate += pow( tex2D(tex, uv+p*size*float2(1.0, ratio)).xyz, float3(gamma.xxx) ) * i;
        }
    }
    
    return float4( pow(accumulate / float3(div.xxx), float3( 1 / gamma.xxx)).xyz , tex2D(tex, uv).w );
}


