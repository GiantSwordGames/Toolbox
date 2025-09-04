Shader "UI/ScreenWipe"
{
    Properties
    {
        _MinCutoff ("Min Cutoff", Range(0,1)) = 0.0
        _MaxCutoff ("Max Cutoff", Range(0,1)) = 1.0
        _Angle ("Angle", Range(0,360)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Ensure derivative functions (fwidth/ddy/ddx) are available
            #pragma target 3.0
            // or: #pragma require derivatives

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color   : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
                half4  color  : COLOR0;
            };

            float _MinCutoff;
            float _MaxCutoff;
            float _Angle;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord * 2.0 - 1.0; // [-1, 1]
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Use half/float for math to avoid precision loss on mobile
                half rad = radians(_Angle);
                half2 dir = half2(cos(rad), sin(rad));

                // Project UV onto direction vector
                half projected = dot(IN.uv, dir);

                // Normalize projection range so projected∈[0,1] at extremes
                half2 absDir = abs(dir);
                half maxProjection = absDir.x + absDir.y + 1e-5h; // avoid div-by-zero if angle is odd
                projected = projected / maxProjection * 0.5h + 0.5h;

                // Feather ≈ 1 pixel in UV space
                half feather = fwidth(projected) * 1.5h;

                // Smooth edge
                half edgeStart = smoothstep(_MinCutoff, _MinCutoff + feather, projected);
                half edgeEnd   = 1.0h - smoothstep(_MaxCutoff - feather, _MaxCutoff, projected);
                half mask = saturate(edgeStart * edgeEnd);

                return half4(IN.color.rgb, IN.color.a * mask);
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}
