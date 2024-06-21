Shader "Unlit/HeatMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags{ "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        LOD 100

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members old_vertex)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 o_v : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.o_v = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3 colors[5];
            float pointRanges[5];

            float _coneData[4*32];
            float _coneCount = 0;
            
            void init()
            {
                colors[0] = float3(0,0,1);
                colors[1] = float3(0,1,1);
                colors[2] = float3(0,1,0);
                colors[3] = float3(1,1,0);
                colors[4] = float3(1,0,0);


                pointRanges[0] = 0;
                pointRanges[1] = 0.25;
                pointRanges[2] = 0.50;
                pointRanges[3] = 0.75;
                pointRanges[4] = 1.0;
            }
            
            float distsq(float3 a,float3 b)
            {
            
                float area_of_effect = 0.5f;
                float d = max(0.0,1 - distance(a,b));
                
                return d;
            }

            float3 HUEtoRGB(float H)
            {
                float R = abs(H * 6 - 3) - 1;
                float G = 2 - abs(H * 6 - 2);
                float B = 2 - abs(H * 6 - 4);
                return saturate(float3(R,G,B));
            }

            float3 HSLtoRGB(float3 HSL)
            {
              float3 RGB = HUEtoRGB(HSL.x);
              float C = (1 - abs(2 * HSL.z - 1)) * HSL.y;
              return (RGB - 0.5) * C + HSL.z;
            }
            
            float3 heatToColor(float weight)
            {
                float h = ((1-weight)*240)/360;
                float s = 1.0;
                float l = 0.5;
                return HSLtoRGB(float3(h,s,l));
            }
            float gauss_normalized(float x, float mean, float sd){
    
                float arg = -0.5*(x*(x-2*mean)/(2*sd*sd));
                                
                return exp(arg);
            } 

            fixed4 frag (v2f input) : SV_Target
            {
                init();
                fixed4 col = tex2D(_MainTex, input.uv);
                
                float totalweight = 0;
                float alphaChannel = 1;
                float3 v = float3(input.o_v[0],input.o_v[1],input.o_v[2]);
                for(int i = 0;i<_coneCount;i++)
                {
                    float3 n = float3(_coneData[i*4 + 0],_coneData[i*4 + 1],_coneData[i*4 + 2]);
                    float limit = _coneData[i*4+3];
                    
                    float f = acos(dot(v,n)/(length(v)*length(n)));
                    float degree = f*57.28; // Radian to Degree

                    if(abs(degree-limit)<(limit*0.3))
                    {
                        totalweight += gauss_normalized(abs(degree-limit),0,1);
                    }
                }
                float3 finalColor = heatToColor(totalweight);


                if((totalweight)<=0)
                {
                    return float4(0.5,0.5,0.5,totalweight);
                }
                return float4(1,0,0,totalweight);


            }
            ENDCG
        }
    }
}
