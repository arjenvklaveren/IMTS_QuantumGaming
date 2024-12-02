Shader "Custom/BlochGrid"
{
    Properties
    {
        _MainColor ("MainColor", Color) = (1,1,1,0)
        _LineColor ("LineColor", Color) = (1,1,1,0.25)
        _DrawLines ("DrawLines", int) = 1
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                return o;
            }

            fixed4 _MainColor;
            fixed4 _LineColor;
            int _DrawLines;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _MainColor;
                if(_DrawLines){
                    if(abs(i.localPos.x) < 0.005 || abs(i.localPos.z) < 0.005) { col = fixed4(0.5,0.5,0.5,0.5) + _LineColor; col.a = 0.25; }
                }
                return col;
            }
            ENDCG
        }
    }
}
