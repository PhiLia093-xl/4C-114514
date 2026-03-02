using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public Image leftPage;
    public Image rightPage;
    public Image flipPage;

    public Sprite[] pages;

    public Animator flipAnimator;

    private int currentPageIndex = 0;
    private bool isFlipping = false;

    void Start()
    {
        flipPage.enabled = false;
        UpdatePages();
    }

    public void NextPage()
    {
        if (isFlipping) return;
        if (currentPageIndex + 2 >= pages.Length) return;

        isFlipping = true;

        flipPage.sprite = rightPage.sprite;
        flipPage.transform.localRotation = Quaternion.identity;
        flipPage.enabled = true;

        flipAnimator.SetTrigger("FlipForwardTrigger");

        Invoke(nameof(AfterForwardFlip), 0.5f);
    }

    void AfterForwardFlip()
    {
        currentPageIndex += 2;
        UpdatePages();

        flipPage.enabled = false;
        flipPage.transform.localRotation = Quaternion.identity;

        isFlipping = false;
    }

    public void PrevPage()
    {
        if (isFlipping) return;
        if (currentPageIndex - 2 < 0) return;

        isFlipping = true;

        currentPageIndex -= 2;

        flipPage.sprite = leftPage.sprite;
        flipPage.transform.localRotation = Quaternion.identity;
        flipPage.enabled = true;

        flipAnimator.SetTrigger("FlipBackwardTrigger");

        Invoke(nameof(AfterBackwardFlip), 0.5f);
    }

    void AfterBackwardFlip()
    {
        UpdatePages();

        flipPage.enabled = false;
        flipPage.transform.localRotation = Quaternion.identity;

        isFlipping = false;
    }

    void UpdatePages()
    {
        leftPage.sprite = pages[currentPageIndex];

        if (currentPageIndex + 1 < pages.Length)
            rightPage.sprite = pages[currentPageIndex + 1];
    }
}