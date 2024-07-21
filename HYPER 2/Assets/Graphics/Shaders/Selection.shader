Shader "Mutox/Selection"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MaxHeight("Max Height", float) = 1.0
        _MinHeight("Min Height", float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        //ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            inline float invLerp(float a,float b,float v) {
                return (v-a)/(b-a);
            }

            struct appdata
            {
                float4 uv : TEXCOORD0;
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float height : TEXCOORD1;
            };
            
            float4 _Color;
            float _MaxHeight;
            float _MinHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.height = invLerp(_MinHeight, _MaxHeight, v.vertex.y);

                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                return float4(1.0, 1.0, 1.0, clamp(i.height, 0, 1));
            }
            ENDCG
        }
    }
}
