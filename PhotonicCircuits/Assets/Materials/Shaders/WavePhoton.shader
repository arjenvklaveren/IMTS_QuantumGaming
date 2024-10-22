Shader "Custom/PhotonWaveShader"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _LineThickness ("Line Thickness", Range(0.005, 0.05)) = 0.01
        _Wavelength ("Wavelength", Range(1.0, 10)) = 5.0
        _Amplitude ("Amplitude", Range(0.0, 1.0)) = 0.5
        _Phase ("Phase", Range(0, 6.28318)) = 0.0
        _Test ("Test", Range(1.0, 10.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 _LineColor;
            fixed4 _BackgroundColor;
            float _LineThickness;
            float _Wavelength;
            float _Amplitude;
            float _Phase;
            float _Test;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _BackgroundColor;

                float x = i.uv.x * _Wavelength * 2.0 * UNITY_PI; 
                float sineWaveY = 0.5 + sin(x + _Phase) * _Amplitude * 0.25;

                float sineSlope = abs(cos(x + _Phase) * _Wavelength * _Amplitude);
                float adjustedThickness = _LineThickness * (1.0 + sineSlope);

                float distanceFromLine = abs(i.uv.y - sineWaveY);

                if(i.uv.y > 0.49 && i.uv.y < 0.51) col = fixed4(0,0,0,1);

                if (distanceFromLine < _LineThickness || distanceFromLine < (adjustedThickness / _Test))
                {
                    col = _LineColor;
                }

                return col;
            }
            ENDCG
        }
    }
}