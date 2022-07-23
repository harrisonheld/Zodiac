Shader "Unlit/PaletteSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Out1 ("Primary", Color) = (255, 0, 0, 255)
        _Out2 ("Secondary", Color) = (0, 255, 0, 255)
        _Out3 ("Tertiary", Color) = (0, 0, 255, 255)
        _In1 ("Red value 1",Integer) = 255
        _In2 ("Red value 2", Integer) = 200
        _In3 ("Red value 3", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Out1;
            float4 _Out2;
            float4 _Out3;
            int _In1;
            int _In2;
            int _In3;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                int alpha = col.a;
                int redval = col.r * 255;

                fixed4 returnColor;

                if(redval >= _In1)
                    returnColor = _Out1;
                else if(redval > _In2)
                    returnColor = _Out2;
                else if(redval >= _In3)
                    returnColor = _Out3;

                returnColor.a = alpha;
                return returnColor;
            }
            ENDCG
        }
    }
}
