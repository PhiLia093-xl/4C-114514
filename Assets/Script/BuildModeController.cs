using UnityEngine;
using UnityEngine.UI; // 旧版 Text

public class BuildModeController : MonoBehaviour
{
    [Header("状态")]
    public bool canEnterBuildMode = false;
    private bool isInBuildMode = false;

    [Header("引用")]
    public MonoBehaviour cameraMoveScript; // 你的相机移动脚本
    public Transform buildViewPoint;       // 固定视角点
    
    

    void Update()
    {
       

        // 按 F 进入建造模式
        if (canEnterBuildMode && Input.GetKeyDown(KeyCode.F))
        {
            EnterBuildMode();
        }
    }

    public void EnterBuildMode()
    {
        isInBuildMode = true;

        // 关闭移动
        if (cameraMoveScript != null)
            cameraMoveScript.enabled = false;

        // 切换到固定视角
        transform.position = buildViewPoint.position;
        transform.rotation = buildViewPoint.rotation;

        // 显示鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
    }

    public void ExitBuildMode()
    {
        isInBuildMode = false;

        // 恢复移动
        if (cameraMoveScript != null)
            cameraMoveScript.enabled = true;

        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}