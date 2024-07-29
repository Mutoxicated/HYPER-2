Shader "Mutox/ParticleHDR"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="AlphaTest" }
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
                float4 color : COLOR;
                float custom1 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float hdrIntensity : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.hdrIntensity = v.custom1;
                return o;
            }

            float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color*i.color*float4(i.hdrIntensity, i.hdrIntensity, i.hdrIntensity, 1.0);
            }
            ENDCG
        }
    }
}