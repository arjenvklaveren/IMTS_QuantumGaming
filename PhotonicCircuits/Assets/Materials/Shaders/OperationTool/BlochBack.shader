Shader "Custom/BlochBack"
{
    Properties
    {
        _SphereColor("SphereColor", Color) = (1,1,1,0)
        _LineColor("LineColor", Color) = (0,0,0,1)
        _LineWidth("LineWidth", Range(0.5, 1.5)) = 1
        _LineGap("LineGap", Range(10.0,15.0)) = 15
        _DepthLines("DepthLines", int) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            float4 _SphereColor;
            float4 _LineColor;
            float _LineWidth;
            float _LineGap;
            int _DepthLines;

            float GetAngle360(float2 v1, float2 v2){
                float angleRadians = atan2(v1.x * v2.y - v1.y * v2.x, dot(v1, v2));
                float angleDegrees = degrees(angleRadians);
                if (angleDegrees < 0)
                {
                    angleDegrees += 360.0;
                }
                return angleDegrees;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(0,0,0,0);



                if (abs(i.localPos.y) < _LineWidth / 100) 
                {
                    float4 localCamPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                    float angle = GetAngle360(float2(i.localPos.x, i.localPos.z), float2(0.0,1.0));
                    float angleNorm = angle / 360.0;

                    if(fmod(angle, 20.0) < _LineGap){
                        col = _LineColor;
                    } 
                }

                if(_DepthLines == 1){
                    if(abs(i.localPos.x) > 0 && abs(i.localPos.z) < 0.005) { col = fixed4(0.5,0.5,0.5,0.5) + _SphereColor; col.a = 0.25; }
                    if(abs(i.localPos.z) > 0 && abs(i.localPos.x) < 0.005) { col = fixed4(0.5,0.5,0.5,0.5) + _SphereColor; col.a = 0.25; }
                    if(abs(i.localPos.y) > 0.499) {col = fixed4(0.5,0.5,0.5,0.5) + _SphereColor; col.a = 0.25; }
                }

                return col;
            }
            ENDCG
        }
    }
}
