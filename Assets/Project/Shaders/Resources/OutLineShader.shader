Shader "Custom/OutLineShader"
{
    Properties
    {
        _OutLineColor ("OutLineColor", Color) = (1,1,1,1)
        _OutLineSize ("OutLineSize", float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fixed4 _OutLineColor;
            float _OutLineSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + (v.normal * _OutLineSize));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_APPLY_FOG(i.fogCoord, _OutLineColor);
                return _OutLineColor;
            }
            ENDCG
        }
    }
}
