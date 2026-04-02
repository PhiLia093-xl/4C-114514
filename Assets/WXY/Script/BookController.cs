using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] int LastPage;

    public Image leftPage;
    public Image rightPage;
    public SpriteRenderer flipPage;
    private RectTransform rt;

    public Camera uiCamera; // ⭐主摄像机

    public Sprite[] pages;

    public AudioSource audioSource;
    public AudioClip flipSound;

    private int currentPageIndex = 0;
    private bool isFlipping = false;
    private bool isCooldown = false;

    private bool isRtoL = false;
    private bool isLtoR = false;

    private float animationDuration = 2f;
    private float halfDuration = 1f;

    private float flipCooldown = 0.5f;

    float _RY = 0;

    void Start()
    {
        rt = flipPage.GetComponent<RectTransform>();

        ResetRy();
        flipPage.enabled = false;
        UpdatePages();
    }

    public void NextPage()
    {
        if (isFlipping || isCooldown) return;
        if (currentPageIndex + 2 >= pages.Length) return;

        isFlipping = true;

        if (audioSource && flipSound)
            audioSource.PlayOneShot(flipSound);

        flipPage.sprite = rightPage.sprite;

        currentPageIndex += 2;
        UpdateRightPage();

        rt.localRotation = Quaternion.identity;
        flipPage.enabled = true;

        isRtoL = true;

        Invoke(nameof(UpdatePageMidForward), halfDuration);
        Invoke(nameof(AfterForwardFlip), animationDuration);
    }

    void UpdatePageMidForward()
    {
        flipPage.flipX = true;
        flipPage.sprite = pages[currentPageIndex];
    }

    void AfterForwardFlip()
    {
        UpdateLeftPage();

        flipPage.enabled = false;
        flipPage.flipX = false;

        isFlipping = false;
        isRtoL = false;

        ResetRy();
        StartCooldown();
    }

    public void PrevPage()
    {
        if (isFlipping || isCooldown) return;
        if (currentPageIndex - 2 < 0) return;

        isFlipping = true;
        isLtoR = true;

        if (audioSource && flipSound)
            audioSource.PlayOneShot(flipSound);

        flipPage.sprite = leftPage.sprite;

        currentPageIndex -= 2;
        UpdateLeftPage();

        rt.localRotation = Quaternion.identity;
        flipPage.enabled = true;

        Invoke(nameof(UpdatePageMidBackward), halfDuration);
        Invoke(nameof(AfterBackwardFlip), animationDuration);
    }

    void UpdatePageMidBackward()
    {
        flipPage.flipX = true;

        if (currentPageIndex + 1 < pages.Length)
            flipPage.sprite = pages[currentPageIndex + 1];
    }

    void AfterBackwardFlip()
    {
        UpdateRightPage();

        flipPage.enabled = false;
        flipPage.flipX = false;

        isFlipping = false;
        isLtoR = false;

        ResetRy();
        StartCooldown();
    }

    void StartCooldown()
    {
        isCooldown = true;
        Invoke(nameof(EndCooldown), flipCooldown);
    }

    void EndCooldown()
    {
        isCooldown = false;
    }

    void UpdatePages()
    {
        leftPage.sprite = pages[currentPageIndex];

        if (currentPageIndex + 1 < pages.Length)
            rightPage.sprite = pages[currentPageIndex + 1];
    }

    void UpdateLeftPage()
    {
        leftPage.sprite = pages[currentPageIndex];
    }

    void UpdateRightPage()
    {
        if (currentPageIndex + 1 < pages.Length)
            rightPage.sprite = pages[currentPageIndex + 1];

        if (currentPageIndex == LastPage - 1) { SaveManager.instance.SaveOnBookBeRead(ID); }
    }

    /// <summary>
    /// RtoL = -1, LtoR = 1
    /// </summary>
    void Rol(float RY, int toward)
    {
        if (uiCamera == null) return;

        // ⭐关键：摄像机旋转 + 你的Y轴翻页
        Quaternion camRot = uiCamera.transform.rotation;
        Quaternion flipRot = Quaternion.Euler(0, RY, 0);

        flipPage.transform.rotation = camRot * flipRot;

        // 保留你的pivot逻辑
        if (toward < 0)
            rt.pivot = new Vector2(0, 0.5f);
        else
            rt.pivot = new Vector2(1, 0.5f);
    }

    void ResetRy()
    {
        _RY = 0;

        // ⭐回正到摄像机方向（防止残留角度）
        if (uiCamera != null)
            flipPage.transform.rotation = uiCamera.transform.rotation;
    }

    private void Update()
    {
        if (!isFlipping) return;

        if (isRtoL)
        {
            _RY += 90 * Time.deltaTime;
            Rol(_RY, -1);
        }
        else if (isLtoR)
        {
            _RY -= 90 * Time.deltaTime;
            Rol(_RY, 1);
        }
    }
}