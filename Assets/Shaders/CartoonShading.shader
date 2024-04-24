Shader "Shaders/CartoonShading"
{
    Properties
    {
        _Color("Color Tint",Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Ramp("Ramp Texture",2D) = "white"{}    //��������
        _Outline("Outline",Range(0,0.1)) = 0.02 //�����߿��
        _Factor("Factor of Outline",Range(0,1)) = 0.5
        _OutlineColor("Outline Color",Color) = (0,0,0,1)    //��������ɫ
        _Specular("Specular",Color) = (1,1,1,1) //�߹ⷴ����ɫ
        _SpecularScale("Specular Scale",Range(0,0.1)) = 0.01    //���Ƽ���߹ⷴ��ʱʹ�õ���ֵ
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            //��Pass��Ⱦ���
            Pass
            {
                //��������֮����ظ�����
                NAME "OUTLINE"
                //���ֻ����Ⱦ���棬���������ߣ������޳�����
                Cull Front
                //�������д�룬��ֹ���彻��������߱�����Ⱦ�������ס
                ZWrite On
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                float _Outline;
                float _Factor;
                fixed4 _OutlineColor;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal:NORMAL;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                };

                //������ɫ��
                v2f vert(appdata v)
                {
                    v2f o;
                    float3 pos = UnityObjectToViewPos(v.vertex.xyz);
                    float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                    normal.z = -0.5;
                    pos = pos + normalize(normal) * _Outline;
                    o.vertex = UnityViewToClipPos(float4(pos,1.0));
                    return o;
                }

                //ƬԪ��ɫ��
                fixed4 frag(v2f i) : SV_Target
                {
                    //����������ɫ��Ⱦ��������
                    return fixed4(_OutlineColor.rgb,1);
                }
            ENDCG
        }
            //��Pass��Ⱦ��ͨ��ɫЧ������Ҫ���ð������ع���ģ����Ͻ�������
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                Cull Back
                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdbase

                #include "UnityCG.cginc"
                //������Ӱ��صĺ�
                #include "AutoLight.cginc"
                //����Ԥ��Ĺ��ձ�������_LightColor0
                #include "Lighting.cginc"

                fixed4 _Color;
                sampler2D _MainTex;
                sampler2D _Ramp;
                fixed4 _Specular;
                fixed _SpecularScale;
                float4 _MainTex_ST;

                struct appdata
                {
                    float4 vertex:POSITION;
                    float2 uv:TEXCOORD0;
                    float3 normal:NORMAL;
                    float4 tangent:TANGENT;
                };

                struct v2f
                {
                    float4 pos:SV_POSITION;
                    float2 uv:TEXCOORD0;
                    float3 worldNormal:TEXCOORD1;
                    float3 worldPos:TEXCOORD2;
                    SHADOW_COORDS(3)
                };

                //������ɫ��
                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                    o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
                    o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                    TRANSFER_SHADOW(o);

                    return o;
                }

                fixed4 frag(v2f i) :SV_Target
                {
                    //�����������ʸ�������й�һ������
                    fixed3 worldNormal = normalize(i.worldNormal);
                    fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                    fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                    fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);

                    //������ʷ�����
                    fixed4 c = tex2D(_MainTex,i.uv);
                    fixed3 albedo = c.rgb * _Color.rgb;

                    //���㻷������
                    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                    //���㵱ǰ���������µ���Ӱֵ
                    UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);

                    //�����������������ϵ����������Ӱֵ��˵õ�������ϵ��
                    fixed diff = dot(worldNormal,worldLightDir);
                    diff = (diff * 0.5 + 0.5) * atten;

                    //ʹ��������ϵ���Խ���������в������������ʷ����ʹ�����ɫ��ˣ��õ��������������
                    fixed3 diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp,float2(diff,diff)).rgb;

                    //����߹ⷴ��ϵ���������߹��Ե�Ĺ��ɽ��п���ݴ���ϵ��Խ�󣬹���Խ����
                    fixed spec = dot(worldNormal,worldHalfDir);
                    fixed w = fwidth(spec) * 2.0;

                    //����߹⣬��[-w,w]��Χ��ƽ����ֵ
                    fixed3 specular = _Specular.rgb * smoothstep(-w,w,spec - (1 - _SpecularScale)) * step(0.0001,_SpecularScale);

                    //���ػ����⣬��������պ͸߹ⷴ����յ��ӵĳɹ�
                    return fixed4(ambient + diffuse + specular,1.0);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}