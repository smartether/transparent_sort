Shader "Unlit/Prestencil"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DrawLayerRef("DrawLayerRef", int) = 8
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		// mask object area
		Pass
		{
			stencil{
				ref 8
				comp Always
				ReadMask 15
				WriteMask 15
				Pass Replace
				Fail Replace
			}
            
			ZWrite Off
			ZTest Less
			ColorMask 0
			Cull Off
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);
				//clip(0.9699 - col.a);
				return 0.2;
			}
			ENDCG
		}
 
		// reset object area depth
		Pass
		{
			Blend One Zero
			ZWrite On
			ZTest Always
			ColorMask 0
			Cull Off
			stencil{
				ref 8
				comp LEqual
				ReadMask 15
				WriteMask 15
				Pass Keep
				Fail Keep
				ZFail Keep
			}
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.z = 1 * o.vertex.w;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return 0;
			}
			ENDCG
		}
		
		// draw occlusion info
		Pass
		{
			ZWrite On
			ZTest Less
			ColorMask 0
			Cull Off
			stencil{
				ref 8	// TO DO  limit max iterator depth 11(3 layer)
				comp LEqual
				ReadMask 15
				WriteMask 15
				Pass Keep
				Fail Keep
				ZFail IncrSat
			}
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(0.9699 - col.a);
				return 0.5;
			}
			ENDCG
		}

		// draw each transparent layer
		Pass
		{
			Tags { "RenderType"="Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Less
			ColorMask RGBA
			Cull Off
			stencil{
				ref [_DrawLayerRef] // {8,9,10,11} iterator each overlay layer like CT
				comp Equal
				ReadMask 15
				WriteMask 15
				Pass DecrSat
				Fail DecrSat
				//ZFail DecrSat
			}
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(0.9699 - col.a);
				return col;
			}
			ENDCG
		}


		// draw each Opaque layer
		Pass
		{
			Tags { "RenderType"="Opaque" }
			ZWrite On
			ZTest Less
			ColorMask RGBA
			Cull Off

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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a - 0.9699);
				return col;
			}
			ENDCG
		}




		// restore opaque depth
		Pass
		{
			Tags { "RenderType"="Opaque" }
			ZWrite On
			ZTest Less
			ColorMask 0
			Cull Off
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a - 0.9699);
				return col;
			}
			ENDCG
		}


		// shader
	}
}
