Shader "PixelMiner/UnlitBlock"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0.0, 1.0)) = 1.0 // Alpha property to control transparency
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue" = "Overlay"}    // Set the render type to Transparent
        //Tags { "RenderType"="Opaque" "Queue" = "Overlay"} 
        LOD 100

        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha // Standard alpha blending

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float2 uv2_ColorTex: TEXCOORD1; // Second set of UV coordinates
            };

            struct v2f
            {
                float2 uv_MainTex : TEXCOORD0;
                float2 uv2_ColorTex : TEXCOORD1; // Second set of UV coordinates
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            
            //In Unity's ShaderLab, _MainTex_ST is a set of texture coordinate scaling and offset values 
            // associated with the main texture (_MainTex). The "ST" convention comes from "scale" and "translate,"
            // which are used to control how the texture is mapped onto the 3D geometry.
            sampler2D _MainTex;
            sampler2D _ColorTex; // New texture
            float4 _MainTex_ST;
            float4 _ColorTex_ST;
            float _Alpha;
    

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
                o.uv2_ColorTex =  TRANSFORM_TEX(v.uv2_ColorTex, _ColorTex);    // Map the second set of UV coordinates for _SecondTex
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the main texture
                fixed4 col1 = tex2D(_MainTex, i.uv_MainTex);

                // Sample the color texture using  the second set of UV coordinates
                fixed4 col2 = tex2D(_ColorTex, i.uv2_ColorTex);

                // BLend the textures
                fixed3 mulCol = col1.rgb * col2;
                fixed4 finalColor = fixed4(mulCol, col1.a * _Alpha);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return col1 * col2;
            }
            ENDCG
        }
    }
}
