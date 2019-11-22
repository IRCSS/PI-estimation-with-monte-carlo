Shader "Unlit/points"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off

		Pass
		{
			// =======================================================

			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag 
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float3 color  : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

	        StructuredBuffer<float4> _ParticleDataBuff;
			                 float   _frameCount;
            #define sin60 0.866025

			float rand(float seed) {
				return frac(sin(seed *1012.)*422.7);
			}

			v2f vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
			{
				v2f o;
				float3 pos   = _ParticleDataBuff[inst];

				//pos.xy = float2(rand(inst + _frameCount) , rand(inst + 11.+ _frameCount))*2. -1.;

				float3 right = float3(1, 0, 0);
				float3 up    = float3(0, -1, 0);
				float size   = 0.01;	

				[branch] switch (id) {
				case 0: pos = pos + right*size      ; break;
				case 1: pos = pos - right*size      ; break;
				case 2: pos = pos + up   *size*sin60; break;
				};

				pos     += -up*size*sin60*0.5;
				o.color  = lerp( float3(1., 0., 0.), float3(0., 1., 0.), step(1.0, length(pos.xy)));
				o.vertex = float4(pos, 1.);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = i.color.xyzz;
				return col;
			}
			ENDCG
		}
	}
}
