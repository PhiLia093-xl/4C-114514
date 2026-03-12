using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public SpriteRenderer leftPage;
    public SpriteRenderer rightPage;
    public SpriteRenderer flipPage;

    public Sprite[] pages;

    public Animator flipAnimator;

    public AudioSource audioSource;
    public AudioClip flipSound;

    private int currentPageIndex = 0;
    private bool isFlipping = false;
    private bool isCooldown = false;

    private float animationDuration = 2f;
    private float halfDuration = 1f;

    // 新增翻页冷却
    private float flipCooldown = 0.5f;

    void Start()
    {
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

        flipPage.transform.localRotation = Quaternion.identity;
        flipPage.flipX = false;
        flipPage.enabled = true;

        flipAnimator.SetTrigger("FlipForwardTrigger");

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
        flipPage.transform.localRotation = Quaternion.identity;

        isFlipping = false;

        StartCooldown();
    }

    public void PrevPage()
    {
        if (isFlipping || isCooldown) return;
        if (currentPageIndex - 2 < 0) return;

        isFlipping = true;

        if (audioSource && flipSound)
            audioSource.PlayOneShot(flipSound);

        flipPage.sprite = leftPage.sprite;

        currentPageIndex -= 2;

        UpdateLeftPage();

        flipPage.transform.localRotation = Quaternion.identity;
        flipPage.enabled = true;

        flipPage.flipX = true;

        flipAnimator.SetTrigger("FlipBackwardTrigger");

        Invoke(nameof(UpdatePageMidBackward), halfDuration);
        Invoke(nameof(AfterBackwardFlip), animationDuration);
    }

    void UpdatePageMidBackward()
    {
        flipPage.flipX = false;
        if (currentPageIndex + 1 < pages.Length)
            flipPage.sprite = pages[currentPageIndex + 1];
    }

    void AfterBackwardFlip()
    {
        UpdateRightPage();
        flipPage.enabled = false;
        flipPage.transform.localRotation = Quaternion.identity;

        isFlipping = false;

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
    }
}