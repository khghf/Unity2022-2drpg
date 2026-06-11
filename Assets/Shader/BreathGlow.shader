Shader "Unlit/BreathGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Color("图片颜色",Color)=(0,1,0,1)
        _GlowColor("发光颜色", Color) = (0, 1, 0,1) 
        _GlowIntensity("发光强度", Range(1, 10)) = 3.0   

        _BreathSpeed("呼吸速度", Range(0.1, 5)) = 1.0   
        _BreathAmount("呼吸幅度", Range(0, 1)) = 0.3    
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float4 _GlowColor;
            float _GlowIntensity;
            float _GlowRange;

            float _BreathSpeed;
            float _BreathAmount;
            float _EnableBreath;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                float4 col = texColor;
                col.rgb=_Color.rgb;

                float breath = _BreathAmount + _BreathAmount * sin(_Time.y * _BreathSpeed);

                float glow =pow(_GlowIntensity , breath);
                // 叠加发光
                col.rgb += _GlowColor.rgb * glow * _GlowColor.a;

                col.rgb *= col.a;

                return col;
            }
            ENDCG
        }
    }
}