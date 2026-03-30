using UnityEngine;

public class CameraControlToggle : MonoBehaviour
{
    public CameraController cameraController;

    // 禁止摄像机控制（按钮1调用）
    public void DisableCamera()
    {
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
    }

    // 恢复摄像机控制（按钮2调用）
    public void EnableCamera()
    {
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
    }
}