Shader "Custom/TopEdgeHighlightShader"
{
    Properties
    {
        _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.05
        _HighlightColor ("Highlight Color", Color) = (1, 1, 0, 1) // Default to yellow.
        _Emission("Emission", Range(0,5)) = 2  // Default to a value of 2 for a brighter highlight.

    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

Pass
{
    Blend SrcAlpha OneMinusSrcAlpha
    ZWrite Off

    CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
            };

            float _EdgeWidth;
            float4 _HighlightColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // No need to transform the UV as we want raw values for the cube
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = half4(0, 0, 0, 0); // Default to transparent
                // Check if the normal is pointing upwards
                if (i.normal.y > 0.9) 
                {
                    // Check the edges based on UV and the provided edge width
                    if (i.uv.x < _EdgeWidth || i.uv.x > 1 - _EdgeWidth || i.uv.y < _EdgeWidth || i.uv.y > 1 - _EdgeWidth)
                    {
                        col = _HighlightColor;
                    }
                }
                return col;
            }
            ENDCG
        }
    }
}
