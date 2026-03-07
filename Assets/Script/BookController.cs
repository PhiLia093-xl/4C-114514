using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public SpriteRenderer leftPage;
    public SpriteRenderer rightPage;
    public SpriteRenderer flipPage;

    public Sprite[] pages;

    public Animator flipAnimator;

    private int currentPageIndex = 0;
    private bool isFlipping = false;

    private float animationDuration = 2f;
    private float halfDuration = 1f;

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
        flipPage.flipX = false;
        flipPage.enabled = true;

        flipAnimator.SetTrigger("FlipForwardTrigger");

        
        Invoke(nameof(UpdatePageMidForward), halfDuration);
        Invoke(nameof(AfterForwardFlip), animationDuration);
    }

    
    void UpdatePageMidForward()
    {
        flipPage.flipX = true;
        currentPageIndex += 2;
        UpdatePages();

        
        flipPage.sprite = leftPage.sprite;
    }

    void AfterForwardFlip()
    {
        flipPage.enabled = false;
        flipPage.transform.localRotation = Quaternion.identity;

        isFlipping = false;
    }

    
    public void PrevPage()
    {
        if (isFlipping) return;
        if (currentPageIndex - 2 < 0) return;

        isFlipping = true;

        flipPage.sprite = leftPage.sprite;
        flipPage.transform.localRotation = Quaternion.identity;
        flipPage.enabled = true;

        flipPage.flipX= true;

        flipAnimator.SetTrigger("FlipBackwardTrigger");

        
        Invoke(nameof(UpdatePageMidBackward), halfDuration);

        
        Invoke(nameof(AfterBackwardFlip), animationDuration);
    }

    
    void UpdatePageMidBackward()
    {
        flipPage.flipX = false;
        currentPageIndex -= 2;
        UpdatePages();

        
        flipPage.sprite = rightPage.sprite;
    }

    void AfterBackwardFlip()
    {
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