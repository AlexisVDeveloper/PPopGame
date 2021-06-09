Shader "Hidden/CameraFadeout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TransitionTex ("Transition Texture", 2D) = "white" {}
		_Color("ScreenColor", Color) = (1,1,1,1)
        _CutOff ("CutOff", Range(0,1)) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _TransitionTex;
            fixed _CutOff;
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 trans = tex2D(_TransitionTex, i.uv);
                fixed4 col = tex2D(_MainTex, i.uv);
                
                if(trans.r < _CutOff)
                    return lerp(col, _Color, _CutOff);

                return col;
            }
            ENDCG
        }
    }
}
