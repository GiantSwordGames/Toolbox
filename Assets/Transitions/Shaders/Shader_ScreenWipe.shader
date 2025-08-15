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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float2 screenPos : TEXCOORD1;
            };

            float _MinCutoff;
            float _MaxCutoff;
            float _Angle;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.texcoord * 2.0 - 1.0; // UV range from -1 to 1
                OUT.color = IN.color ;

                OUT.screenPos = ComputeScreenPos(OUT.vertex).xy;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float rad = radians(_Angle);
                float2 dir = float2(cos(rad), sin(rad));

                // Project UV onto direction vector
                float projected = dot(IN.uv, dir);

                // Normalize projection range
                float2 absDir = abs(dir);
                float maxProjection = absDir.x + absDir.y;
                projected = projected / maxProjection * 0.5 + 0.5;

                // Auto Feather Width (approx 1 screen pixel in UV space)
                float feather = fwidth(projected) * 1.5; // 1.5 pixels wide edge

                // Smoothstep anti-alias edges
                float edgeStart = smoothstep(_MinCutoff, _MinCutoff + feather, projected);
                float edgeEnd = 1.0 - smoothstep(_MaxCutoff - feather, _MaxCutoff, projected);

                float mask = edgeStart * edgeEnd;

                return fixed4(IN.color.rgb, IN.color.a * mask);
            }
            ENDCG
        }
    }
}
