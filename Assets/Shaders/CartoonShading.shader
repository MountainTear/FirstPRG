Shader "Shaders/CartoonShading"
{
    Properties
    {
        _Color("Color Tint",Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Ramp("Ramp Texture",2D) = "white"{}    //渐变纹理
        _Outline("Outline",Range(0,0.1)) = 0.02 //轮廓线宽度
        _Factor("Factor of Outline",Range(0,1)) = 0.5
        _OutlineColor("Outline Color",Color) = (0,0,0,1)    //轮廓线颜色
        _Specular("Specular",Color) = (1,1,1,1) //高光反射颜色
        _SpecularScale("Specular Scale",Range(0,0.1)) = 0.01    //控制计算高光反射时使用的阈值
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            //此Pass渲染描边
            Pass
            {
                //命名用于之后可重复调用
                NAME "OUTLINE"
                //描边只用渲染背面，挤出轮廓线，所以剔除正面
                Cull Front
                //开启深度写入，防止物体交叠处的描边被后渲染的物体盖住
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

                //顶点着色器
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

                //片元着色器
                fixed4 frag(v2f i) : SV_Target
                {
                    //用轮廓线颜色渲染整个背面
                    return fixed4(_OutlineColor.rgb,1);
                }
            ENDCG
        }
            //此Pass渲染卡通着色效果，主要运用半兰伯特光照模型配合渐变纹理
            Pass
            {
                Tags{"LightMode" = "ForwardBase"}
                Cull Back
                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdbase

                #include "UnityCG.cginc"
                //引入阴影相关的宏
                #include "AutoLight.cginc"
                //引入预设的光照变量，如_LightColor0
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

                //顶点着色器
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
                    //计算各个方向矢量并进行归一化处理
                    fixed3 worldNormal = normalize(i.worldNormal);
                    fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                    fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                    fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);

                    //计算材质反射率
                    fixed4 c = tex2D(_MainTex,i.uv);
                    fixed3 albedo = c.rgb * _Color.rgb;

                    //计算环境光照
                    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                    //计算当前世界坐标下的阴影值
                    UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);

                    //计算半兰伯特漫反射系数，并和阴影值相乘得到漫反射系数
                    fixed diff = dot(worldNormal,worldLightDir);
                    diff = (diff * 0.5 + 0.5) * atten;

                    //使用漫反射系数对渐变纹理进行采样，最后与材质反射率光照颜色相乘，得到最后的漫反射光照
                    fixed3 diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp,float2(diff,diff)).rgb;

                    //计算高光反射系数，并将高光边缘的过渡进行抗锯齿处理，系数越大，过渡越明显
                    fixed spec = dot(worldNormal,worldHalfDir);
                    fixed w = fwidth(spec) * 2.0;

                    //计算高光，在[-w,w]范围内平滑插值
                    fixed3 specular = _Specular.rgb * smoothstep(-w,w,spec - (1 - _SpecularScale)) * step(0.0001,_SpecularScale);

                    //返回环境光，漫反射光照和高光反射光照叠加的成果
                    return fixed4(ambient + diffuse + specular,1.0);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}