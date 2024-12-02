Shader "Custom/BlochFront"
{
    Properties
    {
        _SphereColor("SphereColor", Color) = (1,1,1,0)
        _LineColor("LineColor", Color) = (0,0,0,1)
        _LineWidth("LineWidth", Range(0.5, 1.5)) = 1
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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewWS : TEXCOORD2;
            };

            float4 _SphereColor;
            float4 _LineColor;
            float _LineWidth;
            int _DepthLines;

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                o.normalWS = mul((float3x3)unity_ObjectToWorld, v.normal);
                o.viewWS = normalize(_WorldSpaceCameraPos - worldPos.xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _SphereColor;

                if (abs(i.localPos.y) < _LineWidth / 100) 
                {
                    col = _LineColor;
                }

                if(_DepthLines == 1){
                    if(abs(i.localPos.x) > 0 && abs(i.localPos.z) < 0.005) { col = fixed4(0.5,0.5,0.5,0.5) + _SphereColor; col.a = 0.25; }
                    if(abs(i.localPos.z) > 0 && abs(i.localPos.x) < 0.005) { col = fixed4(0.5,0.5,0.5,0.5) + _SphereColor; col.a = 0.25; }
                    if(abs(i.localPos.y) > 0.499) {col = fixed4(0.5,0.5,0.5,0.5) + _SphereColor; col.a = 0.25; }
                }

                float fresnel = pow(1.0 - saturate(dot(i.normalWS, i.viewWS)), 1);
                float normWidth = 0.2 + ((_LineWidth - 0.5)/(1.5 - 0.5)) * (0.4 - 0.2);
                if(fresnel > (1.0 - normWidth)) col = _LineColor;

                return col;
            }
            ENDCG
        }
    }
}
