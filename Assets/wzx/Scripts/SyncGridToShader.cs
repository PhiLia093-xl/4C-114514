using UnityEngine;

[ExecuteInEditMode] // 允许在编辑模式下实时预览，方便调整位置和大小
public class SyncGridToShader : MonoBehaviour
{
    [Header("组件引用")]
    [Tooltip("拖入你的 Grid 物体")]
    public Grid gridComponent;
    [Tooltip("拖入你的 GridVisualization 物体")]
    public Renderer gridRenderer;

    [Header("Shader 属性名 (需与 Shader Graph 中一致)")]
    public string offsetPropertyName = "_GridOffset";
    public string sizePropertyName = "_CellSize";

    void Update()
    {
        // 确保引用没有丢失，并且材质存在
        if (gridComponent != null && gridRenderer != null && gridRenderer.sharedMaterial != null)
        {
            // 1. 同步坐标偏移：获取 Grid 的世界坐标，传递给 Shader
            Vector3 gridWorldPos = gridComponent.transform.position;
            gridRenderer.sharedMaterial.SetVector(offsetPropertyName, gridWorldPos);

            // 2. 同步网格大小：获取 Grid 组件里的 Cell Size (根据你的截图，这里取 x 轴的值 20)
            float cellSize = gridComponent.cellSize.x;
            gridRenderer.sharedMaterial.SetFloat(sizePropertyName, cellSize);

            // 注：如果你的网格是长方形(X和Z不同)，你需要把 Shader 里的 _CellSize 改成 Vector2，
            // 然后用这行代码替代上面那行：
            // gridRenderer.sharedMaterial.SetVector(sizePropertyName, new Vector4(gridComponent.cellSize.x, gridComponent.cellSize.z, 0, 0));
        }
    }
}