using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 20f;
    public float transitionDuration = 1.2f; // 平滑移动时间

    [Header("视角区域 1: BuildingSystem1")]
    public GameObject buildingSystem1;
    private readonly Vector3 targetPos1 = new Vector3(741.9f, 200f, 350f);
    private readonly Quaternion targetRot1 = Quaternion.Euler(50f, 0f, 0f);

    [Header("视角区域 2: 内廷中路")]
    public GameObject innerCourtMiddle; // 在检查器里把“内廷中路”物体拖到这里
    private readonly Vector3 targetPos2 = new Vector3(753f, 150f, 950f); // 图片中的位置
    private readonly Quaternion targetRot2 = Quaternion.Euler(57f, 0f, 0f); // 图片中的旋转

    // 内部状态变量
    private Vector3 beforePos;
    private Quaternion beforeRot;
    private GameObject currentActiveObject; // 记录当前通过 F 键激活了哪个物体
    private bool isLocked = false;
    private bool isTransitioning = false;

    void Update()
    {
        if (isTransitioning) return;

        if (!isLocked)
        {
            HandleStandardMovement();
            CheckForActivationInput();
        }
        else
        {
            CheckForDeactivationInput();
        }
    }

    void HandleStandardMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v);
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }

    void CheckForActivationInput()
    {
        if (!Input.GetKeyDown(KeyCode.F)) return;

        float x = transform.position.x;
        float z = transform.position.z;

        // 检测区域 1: BuildingSystem1
        if (x >= 626f && x <= 874f && z >= 350f && z <= 845f)
        {
            EnterSpecialView(buildingSystem1, targetPos1, targetRot1);
        }
        // 检测区域 2: 内廷中路 (新需求)
        else if (x >= 680f && x <= 826f && z >= 863f && z <= 1230f)
        {
            EnterSpecialView(innerCourtMiddle, targetPos2, targetRot2);
        }
    }

    // 进入特殊视角的通用方法
    void EnterSpecialView(GameObject objToActivate, Vector3 destination, Quaternion destRotation)
    {
        beforePos = transform.position;
        beforeRot = transform.rotation;
        currentActiveObject = objToActivate;

        if (currentActiveObject != null) currentActiveObject.SetActive(true);
        StartCoroutine(SmoothMove(destination, destRotation, true));
    }

    void CheckForDeactivationInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 关闭当前记录的激活物体
            if (currentActiveObject != null) currentActiveObject.SetActive(false);

            // 回到记录的 BeforePos
            StartCoroutine(SmoothMove(beforePos, beforeRot, false));
        }
    }

    IEnumerator SmoothMove(Vector3 destination, Quaternion destRotation, bool locking)
    {
        isTransitioning = true;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < transitionDuration)
        {
            transform.position = Vector3.Lerp(startPos, destination, elapsed / transitionDuration);
            transform.rotation = Quaternion.Slerp(startRot, destRotation, elapsed / transitionDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        transform.rotation = destRotation;
        isLocked = locking;
        isTransitioning = false;
    }
}