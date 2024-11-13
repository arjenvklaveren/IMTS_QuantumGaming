Shader "Custom/PhotonWaveShader"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _LineThickness ("Line Thickness", Range(0.005, 0.05)) = 0.01
        _MiddleDivideThickness ("Middle Thickness", Range(0.01, 0.03)) = 0.02
        _Wavelength ("Wavelength", Range(1.0, 10)) = 5.0
        _Amplitude ("Amplitude", Range(0.0, 1.0)) = 0.5
        _Phase ("Phase", Range(0, 6.28318)) = 0.0
        _LineWidthCorrection ("LineWidth Correction", Range(1.0, 3.0)) = 1.5
        _DrawDepthVisuals("Draw Depth Visuals", int) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Cull Off

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
            float _MiddleDivideThickness;
            float _Wavelength;
            float _Amplitude;
            float _Phase;
            float _LineWidthCorrection;
            float _DrawDepthVisuals;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _BackgroundColor;

                float x = i.uv.x * _Wavelength * 2.0 * UNITY_PI; 
                float sineWaveY = 0.5 + sin(x + _Phase) * _Amplitude * 0.375;

                float sineSlope = abs(cos(x + _Phase) * _Wavelength * _Amplitude);
                float adjustedThickness = _LineThickness * (1.0 + sineSlope);

                float distanceFromLine = abs(i.uv.y - sineWaveY);

                if(_DrawDepthVisuals == 1){
                    if((i.uv.y > sineWaveY + _LineThickness && i.uv.y < 0.5) || (i.uv.y < sineWaveY - _LineThickness && i.uv.y > 0.5)){
                        col = _LineColor * 0.5;

                        float stripePosition = fmod(i.uv.x, 0.02);

                        if (stripePosition < _LineThickness / 2)
                        {
                            col = _LineColor;
                        }
                    }
                }

                if(_DrawDepthVisuals == 0){
                    if(i.uv.y > (0.5 - _MiddleDivideThickness) && i.uv.y < (0.5 + _MiddleDivideThickness)) col = fixed4(0,0,0,1);
                }

                if (distanceFromLine < _LineThickness || distanceFromLine < (adjustedThickness / _LineWidthCorrection))
                {
                    col = _LineColor;
                }

                if(_DrawDepthVisuals == 1){
                    if(i.uv.y > (0.5 - _MiddleDivideThickness) && i.uv.y < (0.5 + _MiddleDivideThickness)) col = fixed4(0,0,0,1);
                }
                return col;
            }
            ENDCG
        }
    }
}