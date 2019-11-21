Shader "Custom/RealtimeUVProjection"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", COLOR) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;

	        uniform float4x4 _UVProjMatrix;	
			//float4 _UVProjExtra;		
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.x, v.vertex.y, v.vertex.z, 1.0)).xyz;				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 frustumPos = mul(_UVProjMatrix, float4(i.worldPos.xyz, 1.0) );
				float2 uv = frustumPos.xy;
				uv /= frustumPos.w;
				//uv *= _UVProjExtra.w;
				uv.y *= -1.0;
				uv = uv.xy*0.5 + 0.5;				

				fixed4 col = tex2D(_MainTex, uv);
				return col * _Color;
			}
			ENDCG
		}
	}
}
