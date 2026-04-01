using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class InteractableZone
{
    public string zoneName;
    [Header("区域ID")]
    public int ID;

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
    [HideInInspector]public static UnityEvent onBuildingModeEx = new();
    [Header("漫游视角控制")]
    public float moveSpeed = 15f; // WASD 和 QE 移动速度
    public float lookSpeed = 3f;  // 鼠标拖拽旋转速度

    [Header("区域配置列表")]
    public InteractableZone[] zones;

    [Header("通用设置")]
    public float transitionSpeed = 2.0f;

    // 状态记录
    private Vector3 beforePos;
    private Quaternion beforeRot;
    public bool isInteracting = false;
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
                    if (!SaveManager.instance.BookBeReadOrNot(zoneToEnter.ID)) 
                    {
                        Debug.Log($"无法打开{SaveManager.instance.books[zoneToEnter.ID]}的建造系统，" +
                            $"因为{SaveManager.instance.books[zoneToEnter.ID]}的书籍还没有读完");
                        return;
                    }
                    StartCoroutine(EnterInteraction(zoneToEnter));
                }
            }
        }
        else
        {
            // 如果正在交互状态，按 Esc 键退出
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onBuildingModeEx.Invoke();
                StartCoroutine(ExitInteraction());
            }
        }
    }

    // 处理 WASD 移动、QE 升降和鼠标拖拽旋转
    private void HandleFreeMovement()
    {
        // --- 鼠标拖拽旋转 ---
        if (Input.GetMouseButton(0))
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");

            // 限制俯仰角在 -85 到 85 度之间，防止镜头“翻跟头”
            pitch = Mathf.Clamp(pitch, -85f, 85f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        // --- WASD 水平移动与 QE 垂直升降 ---
        float h = Input.GetAxis("Horizontal"); // A/D 键
        float v = Input.GetAxis("Vertical");   // W/S 键
        float upDown = 0f;                     // Q/E 键垂直控制

        // 检测 Q 升高，E 降低
        if (Input.GetKey(KeyCode.Q)) upDown += 1f;
        if (Input.GetKey(KeyCode.E)) upDown -= 1f;

        // 提取摄像机的前方和右方，并将 Y 轴设为 0，确保只在水平面上移动
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        // 绝对向上的向量
        Vector3 up = Vector3.up;

        // 如果有任何方向的输入
        if (h != 0 || v != 0 || upDown != 0)
        {
            // 将水平和垂直的移动意图组合起来，并归一化（防止同时按 W 和 Q 时斜向移动速度过快）
            Vector3 moveDir = (forward * v + right * h + up * upDown).normalized;
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

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator ExitInteraction()
    {
        if (currentActiveZone == null) yield break;

        isMoving = true;

        if (currentActiveZone.targetObject != null) currentActiveZone.targetObject.SetActive(false);

        // 回到按下 F 键前的位置
        yield return StartCoroutine(MoveCamera(beforePos, beforeRot));

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

    private void SyncRotationVariables()
    {
        Vector3 angles = transform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;

        if (pitch > 180f) pitch -= 360f;
    }
}