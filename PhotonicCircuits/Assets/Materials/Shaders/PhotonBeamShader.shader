Shader "Unlit/PhotonBeamShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineLength ("Line Length", float) = 0.4
        _LineGapSize ("Line Gap Size", float) = 0.2
        _LineMoveSpeed ("Line Move Speed", float) = 1
        _DirectionX("Direction X", float) = 0
        _DirectionY("Direction Y", float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent"
          	"RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Stencil {
		        Ref 4
		        Comp NotEqual
		        Pass Replace
		    }

		    Cull Off
      		Lighting Off

		    Blend SrcAlpha OneMinusSrcAlpha  

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _LineLength;
            float _LineGapSize;
            float _LineMoveSpeed;
            float _DirectionX;
            float _DirectionY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _LineGapSize = _LineLength - _LineGapSize;
                //TODO FIX WIGGLE BUG
                fixed4 col;
                float zeroOffset;

                float axis;
                if(_DirectionX != 0) axis = i.worldPos.x;
                else axis = i.worldPos.y;

                float timeOffset = _Time.y * _LineMoveSpeed;

                if(_DirectionX > 0) timeOffset = -timeOffset;
                if(_DirectionY > 0) timeOffset = -timeOffset;

                if(abs(axis) <= _LineLength / 2)
                {
                    zeroOffset = _LineLength / 4;
                }

                if(fmod(abs(axis + timeOffset) + zeroOffset, _LineLength) < _LineGapSize)
                {
                    col = i.color;
                }
                else col = fixed4(0,0,0,0);
                return col;
            }
            ENDCG
        }
    }
}
