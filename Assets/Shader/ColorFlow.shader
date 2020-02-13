Shader "Unlit/ColorFlow"
{
    Properties
    {
        _ColorFrom ("ColorFrom", Color) = (1,1,1,1)
        _ColorTo ("ColorTo", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed",float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 100

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
            float4 _ColorFrom;
            float4 _ColorTo;
            float _Speed;
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
                float smooth =0;
                float time = _Time.y;
                float2 uv = i.uv+float2(0,frac(time*_Speed));
                uv.y = fmod(uv.y,1);
                uv.x = fmod(uv.x,1);
                float4 colorGrad= mul(_ColorFrom,1-uv.y)+mul(_ColorTo,uv.y);
                  fixed4 col = tex2D(_MainTex, float2(uv.x,1-uv.y));
                // apply fog
              
                if(col.a)
                   {
                        return col;
                   }
                else
                 {
                  return colorGrad*(cos(frac(time*_Speed))+sin(1-frac(time*_Speed)))*0.9;   
                  }
                        
                       
            }
            ENDCG
        }
    }
}
