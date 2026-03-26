Shader "Custom/GridCoordinateSystem"
{
    Properties
    {
        _GridSize ("Grid Size (X, Y)", Vector) = (10, 10, 0, 0)
        _LineThickness ("Line Thickness", Range(0.01, 0.5)) = 0.05
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // 设置为透明渲染队列，不参与光照，不写入深度
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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

            float4 _GridSize;
            float _LineThickness;
            float4 _LineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 将 UV 坐标放大到网格倍数
                float2 gridUV = i.uv * _GridSize.xy;

                // 计算当前片段到最近整数坐标（网格线）的距离
                // frac(gridUV) 会在 [0, 1] 循环，再减去 0.5 取绝对值，找到中心点
                float2 dist = abs(frac(gridUV - 0.5) - 0.5);
                
                // 使用 fwidth 进行抗锯齿处理
                // 平滑宽度基于屏幕空间的变化率
                float2 fw = fwidth(gridUV);
                float2 lineAA = smoothstep(_LineThickness + fw, _LineThickness - fw, dist);

                // 处理边框：如果 UV 接近 0 或 1，则强制设为线条色
                // 这里使用很小的阈值来确保外边框始终存在
                float2 border = step(i.uv, 0.005) + step(0.995, i.uv);
                
                // 综合 X 轴和 Y 轴的线条
                float gridIntensity = max(lineAA.x, lineAA.y);
                gridIntensity = max(gridIntensity, max(border.x, border.y));

                // 最终颜色：背景完全透明，线条为纯白（或选定颜色）
                fixed4 col = _LineColor;
                col.a *= gridIntensity;

                return col;
            }
            ENDCG
        }
    }
}