Shader "Core/TreeLight"
{
	Properties
	{
		[NoScaleOffset]_EmissionTex ("Emission Texture", 2D) = "white" {}
		[HDR] _EmissionColor ("Miles", Color) = (0, 0, 0, 1)
		_PulseSpeed ("Pulse Speed", Float) = 1.0
		_MinValue ("Minimum Value", Float) = 1.0
	}
	SubShader
	{
	//Skybox = 0
	//Geometry = 1000
	//AlphaTest = 1500
	//Transparency = 2000
	//Transparency+100 = 2100
		Tags { 
			"RenderType" = "Opaque"
			"Queue" = "AlphaTest+200"
		}
		LOD 100
		
		Pass
		{
			//Start HLSL stuff
			HLSLPROGRAM
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//Vertex stuff (moving, maths and init and so)
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = half2(0.0, 1.0);
				UNITY_TRANSFER_FOG(o, o.vertex);
				
				return o;
			}

			//float4, half4, fixed4
			//0.000001, 0.001, 0.1

			//Declared and Used properties
			float4 _EmissionColor;
			float _PulseSpeed;
			half _MinValue;

			//Fragment/Pixel stuff (each pixel on screen: color, vertex color, transparency, whatever)
			fixed4 frag (v2f i) : SV_TARGET
			{
				_MinValue += 1;
				fixed4 color = _EmissionColor * (_MinValue + sin((_Time * _PulseSpeed).y));

				return color;
			}
			ENDHLSL
		}

	}
}