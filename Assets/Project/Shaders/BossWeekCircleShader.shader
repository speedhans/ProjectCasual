Shader "Custom/BossWeekCircleShader"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_MainUV("MainUV", Vector) = (0,0,0,0)
		_Color("Color", Color) = (1,1,1,1)
		_CircularTexture("CircularTexture", 2D) = "white" {}
		_MinAngle("MinAngle", float) = 0.0
		_MaxAngle("MaxAngle", float) = 0.0
	}
	SubShader
	{
		Tags{ "RenderPipeline" = "LightweightPipeline"}
		Tags
		{
			"RenderPipeline" = "LightweightPipeline"
			"RenderType" = "Transparent"
			"Queue" = "Transparent+0"
		}
		Pass
		{

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

			Cull Off

			ZWrite OFf

			HLSLPROGRAM
		// Required to compile gles 2.0 with standard srp library
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x
		#pragma target 2.0

		#pragma vertex vert
		#pragma fragment frag

		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA


		#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

	#if ETC1_EXTERNAL_ALPHA
		TEXTURE2D(_AlphaTex); SAMPLER(sampler_AlphaTex);
		float _EnableAlphaTexture;
	#endif
		float4 _RendererColor;

		CBUFFER_START(UnityPerMaterial)
		float2 _MainUV;
		CBUFFER_END

		float4 _Color;
		float4 _ForwardVector;
		float _MinAngle;
		float _MaxAngle;
		float4 _AreaColor;

		TEXTURE2D(_MainTexture); SAMPLER(sampler_MainTexture); float4 _MainTexture_TexelSize;
		TEXTURE2D(_CircularTexture); SAMPLER(sampler_CircularTexture); float4 _CircularTexture_TexelSize;

		struct VertexDescriptionInputs
		{
			float3 ObjectSpacePosition;
		};

		struct SurfaceDescriptionInputs
		{
		};


		struct VertexDescription
		{
			float3 Position;
		};

		VertexDescription PopulateVertexData(VertexDescriptionInputs IN)
		{
			VertexDescription description = (VertexDescription)0;
			description.Position = IN.ObjectSpacePosition;
			return description;
		}

		struct SurfaceDescription
		{
			float4 Color;
		};

		SurfaceDescription PopulateSurfaceData(SurfaceDescriptionInputs IN)
		{
			SurfaceDescription surface = (SurfaceDescription)0;
			surface.Color = IsGammaSpace() ? float4(1, 1, 1, 1) : float4 (SRGBToLinear(float3(1, 1, 1)), 1);
			return surface;
		}

		struct GraphVertexInput
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float4 texcoord0 : TEXCOORD0;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};


		struct GraphVertexOutput
		{
			float4 position : POSITION;
			float4 VertexColor : COLOR;
			half4 uv0 : TEXCOORD3;

		};

		GraphVertexOutput vert(GraphVertexInput v)
		{
			GraphVertexOutput o = (GraphVertexOutput)0;
			float3 WorldSpacePosition = mul(UNITY_MATRIX_M,v.vertex).xyz;
			float4 VertexColor = v.color;
			float4 uv0 = v.texcoord0;
			float3 ObjectSpacePosition = mul(UNITY_MATRIX_I_M,float4(WorldSpacePosition,1.0)).xyz;

			VertexDescriptionInputs vdi = (VertexDescriptionInputs)0;
			vdi.ObjectSpacePosition = ObjectSpacePosition;

			VertexDescription vd = PopulateVertexData(vdi);

			v.vertex.xyz = vd.Position;
			VertexColor = v.color;
			o.position = TransformObjectToHClip(v.vertex.xyz);

			o.VertexColor = VertexColor;
			o.uv0 = uv0;

			return o;
		}

		half4 frag(GraphVertexOutput IN) : SV_Target
		{
			float4 color = SAMPLE_TEXTURE2D(_MainTexture, sampler_MainTexture, IN.uv0.xy) * _Color;
			float4 circular = SAMPLE_TEXTURE2D(_CircularTexture, sampler_CircularTexture, IN.uv0.xy);
			float4 uv0 = IN.uv0;

			SurfaceDescriptionInputs surfaceInput = (SurfaceDescriptionInputs)0;
			SurfaceDescription surf = PopulateSurfaceData(surfaceInput);

			surf.Color = color;
	#if ETC1_EXTERNAL_ALPHA
			float4 alpha = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, IN.uv0.xy);
			surf.Color.a = lerp(surf.Color.a, alpha.r, _EnableAlphaTexture);
	#endif
			clip(_MaxAngle - circular.r);

			return surf.Color;
		}

		ENDHLSL
	}
	}
		FallBack "Hidden/InternalErrorShader"
}
