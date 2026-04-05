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
    [HideInInspector] public static UnityEvent onBuildingModeEx = new();
    [Header("漫游视角控制")]
    public float moveSpeed = 15f;
    public float lookSpeed = 3f;

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

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        SyncRotationVariables();
    }

    void Update()
    {
        if (isMoving) return;

        if (!isInteracting)
        {
            HandleFreeMovement();

            if (Input.GetKeyDown(KeyCode.F))
            {
                InteractableZone zoneToEnter = CheckPlayerPosition();
                if (zoneToEnter != null)
                {
                    if (!SaveManager.instance.BookBeReadOrNot(zoneToEnter.ID))
                    {
                        Debug.Log($"无法打开{SaveManager.instance.books[zoneToEnter.ID]}的建造系统");
                        return;
                    }
                    StartCoroutine(EnterInteraction(zoneToEnter));
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onBuildingModeEx.Invoke();
                StartCoroutine(ExitInteraction());
            }
        }
    }

    private void HandleFreeMovement()
    {
        if (Input.GetMouseButton(0))
        {
            yaw += lookSpeed * Input.GetAxis("Mouse X");
            pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            pitch = Mathf.Clamp(pitch, -85f, 85f);
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float upDown = 0f;

        if (Input.GetKey(KeyCode.Q)) upDown += 1f;
        if (Input.GetKey(KeyCode.E)) upDown -= 1f;

        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 up = Vector3.up;

        if (h != 0 || v != 0 || upDown != 0)
        {
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
            t = t * t * (3f - 2f * t);

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