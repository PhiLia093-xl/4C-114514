using UnityEngine;
using System.Collections;

[System.Serializable]
public class InteractableZone
{
    public string zoneName;

    [Header("触发范围")]
    public Vector2 xRange;
    public Vector2 zRange;

    [Header("交互对象")]
    public GameObject targetObject;

    [Header("摄像机目标变换")]
    public Vector3 targetPosition;
    public Vector3 targetRotation;
}

public class CameraController : MonoBehaviour
{
    [Header("漫游视角控制")]
    public float moveSpeed = 15f; // WASD 移动速度
    public float lookSpeed = 3f;  // 鼠标拖拽旋转速度

    [Header("区域配置列表")]
    public InteractableZone[] zones;

    [Header("通用设置")]
    public float transitionSpeed = 2.0f;

    // 状态记录
    private Vector3 beforePos;
    private Quaternion beforeRot;
    private bool isInteracting = false;
    private bool isMoving = false;
    private InteractableZone currentActiveZone;

    // 用于自由视角的内部旋转变量
    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        // 游戏开始时，同步当前摄像机的真实角度，防止第一次点击鼠标时镜头乱飞
        SyncRotationVariables();
    }

    void Update()
    {
        // 如果正在执行平滑移动，禁用所有操作
        if (isMoving) return;

        if (!isInteracting)
        {
            // 1. 自由漫游控制
            HandleFreeMovement();

            // 2. 检测交互
            if (Input.GetKeyDown(KeyCode.F))
            {
                InteractableZone zoneToEnter = CheckPlayerPosition();
                if (zoneToEnter != null)
                {
                    StartCoroutine(EnterInteraction(zoneToEnter));
                }
            }
        }
        else
        {
            // 如果正在交互状态，按 Esc 键退出
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(ExitInteraction());
            }
        }
    }

    // 处理 WASD 移动和鼠标拖拽旋转
    private void HandleFreeMovement()
    {
        // --- 鼠标拖拽旋转 ---
        // 0 代表鼠标左键，1 代表右键。你可以根据需求改为 Input.GetMouseButton(1)
        if (Input.GetMouseButton(0))
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");

            // 限制俯仰角在 -85 到 85 度之间，防止镜头“翻跟头”
            pitch = Mathf.Clamp(pitch, -85f, 85f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        // --- WASD 纯水平移动 ---
        float h = Input.GetAxis("Horizontal"); // A/D 键
        float v = Input.GetAxis("Vertical");   // W/S 键

        // 提取摄像机的前方和右方，并将 Y 轴设为 0，确保只在水平面上移动，不会飞天遁地
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        if (h != 0 || v != 0)
        {
            Vector3 moveDir = (forward * v + right * h).normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    private InteractableZone CheckPlayerPosition()
    {
        float x = transform.position.x;
        float z = transform.position.z;

        foreach (var zone in zones)
        {
            if (x >= zone.xRange.x && x <= zone.xRange.y &&
                z >= zone.zRange.x && z <= zone.zRange.y)
            {
                return zone;
            }
        }
        return null;
    }

    private IEnumerator EnterInteraction(InteractableZone zone)
    {
        isMoving = true;
        currentActiveZone = zone;

        // 记录进入交互前的位置和旋转
        beforePos = transform.position;
        beforeRot = transform.rotation;

        if (zone.targetObject != null) zone.targetObject.SetActive(true);

        yield return StartCoroutine(MoveCamera(zone.targetPosition, Quaternion.Euler(zone.targetRotation)));

        isInteracting = true;
        isMoving = false;
    }

    private IEnumerator ExitInteraction()
    {
        if (currentActiveZone == null) yield break;

        isMoving = true;

        if (currentActiveZone.targetObject != null) currentActiveZone.targetObject.SetActive(false);

        // 回到按下 F 键前的位置
        yield return StartCoroutine(MoveCamera(beforePos, beforeRot));

        // 【关键】退回原位后，重新同步 yaw 和 pitch 数值
        // 否则你退回来后一拖鼠标，镜头会瞬间闪回你按 F 时的欧拉角
        SyncRotationVariables();

        isInteracting = false;
        isMoving = false;
        currentActiveZone = null;
    }

    private IEnumerator MoveCamera(Vector3 destination, Quaternion destRotation)
    {
        float elapsedTime = 0;
        float duration = 1.0f / transitionSpeed;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t); // 平滑步进

            transform.position = Vector3.Lerp(startPos, destination, t);
            transform.rotation = Quaternion.Slerp(startRot, destRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        transform.rotation = destRotation;
    }

    // 同步内部旋转变量与摄像机实际旋转，防止数值断层导致镜头闪烁
    private void SyncRotationVariables()
    {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;

        // Unity 的欧拉角 x 有时会返回 350 之类的数值（代表 -10 度），这里将其标准化
        if (pitch > 180f) pitch -= 360f;
    }
}