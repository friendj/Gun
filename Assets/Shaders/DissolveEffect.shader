Shader"MyShader/DissolveEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // ������
        _Color("Color", Color) = (1, 1, 1, 1)  // ģ����ɫ
        _NoiseTex("Noise Tex", 2D) = "white"{}  // ��������
        _BurnAmount("Burn Amount", Range(0, 1)) = 0  // ȼ����, ֵԽ��ģ���ο�Խ��
        _LineWidth("Burn Line Width", Range(0, 0.2)) = 0  // ȼ�յ��������
        _BurnOuterColor("Burn Outer Color", Color) = (1, 0, 0, 1)  // ȼ�����������ɫ
        _ButnInnerColor("Burn Inner Color", Color) = (1, 0, 0, 1)  // ȼ�������ڲ���ɫ
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            sampler2D _NoiseTex;
            fixed _BurnAmount;
            fixed _LineWidth;
            fixed4 _BurnOuterColor;
            fixed4 _BurnInnerColor;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_Position;
                float3 normal : NORMAL;
                half2 uvMainTex : TEXCOORD0;
                half2 uvNoiseTex : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uvMainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvNoiseTex = TRANSFORM_TEX(v.texcoord, _NoiseTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                fixed noise = tex2D(_NoiseTex, i.uvNoiseTex).r;
                float factor = noise - _BurnAmount;
                clip(factor);
                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed4 aldebo = tex2D(_MainTex, i.uvMainTex) * _Color;
                fixed4 ambient = UNITY_LIGHTMODEL_AMBIENT * aldebo;
                fixed4 diffuse = _LightColor0 * aldebo * max(0, dot(i.normal, lightDir));
                fixed t = smoothstep(0, _LineWidth, factor);
                fixed4 burnColor = lerp(_BurnOuterColor, _BurnInnerColor, t);
                fixed4 finalColor = lerp(burnColor, ambient + diffuse, t);
                return fixed4(finalColor.rgb, 1);
            }
            ENDCG
        }
    }
}
