using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    public Image leftPage;
    public Image rightPage;
    public SpriteRenderer flipPage;
    private RectTransform rt;

    public Sprite[] pages;


    //public Animator flipAnimator;

    public AudioSource audioSource;
    public AudioClip flipSound;

    private int currentPageIndex = 0;
    private bool isFlipping = false;
    private bool isCooldown = false;


    private bool isRtoL = false;
    private bool isLtoR = false;

    private float animationDuration = 2f;
    private float halfDuration = 1f;

    // 新增翻页冷却
    private float flipCooldown = 0.5f;

    void Start()
    {
        ResetRy();
        flipPage.enabled = false;
        UpdatePages();
        rt = flipPage.GetComponent<RectTransform>();
    }

    public void NextPage()
    {
        Debug.Log("NextPage");
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

        //flipAnimator.SetTrigger("FlipForwardTrigger");

        isRtoL = true;

        Invoke(nameof(UpdatePageMidForward), halfDuration);
        Invoke(nameof(AfterForwardFlip), animationDuration);
    }

    void UpdatePageMidForward()
    {
        //flipPage.rectTransform.localScale = new(-flipPage.rectTransform.localScale.x,
        //    flipPage.rectTransform.localScale.y, flipPage.rectTransform.localScale.z);
        flipPage.flipX =true;
        flipPage.sprite = pages[currentPageIndex];
    }

    void AfterForwardFlip()
    {
        UpdateLeftPage();
        flipPage.enabled = false;
        flipPage.flipX=false;
        rt.localRotation = Quaternion.identity;

        isFlipping = false;
        isRtoL = false;
        ResetRy();

        StartCooldown();
    }

    public void PrevPage()
    {
        Debug.Log("PrevPage");
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

        //flipAnimator.SetTrigger("FlipBackwardTrigger");

        Invoke(nameof(UpdatePageMidBackward), halfDuration);
        Invoke(nameof(AfterBackwardFlip), animationDuration);
    }

    void UpdatePageMidBackward()
    {
        //flipPage.rectTransform.localScale = new(-flipPage.rectTransform.localScale.x,
        //    flipPage.rectTransform.localScale.y, flipPage.rectTransform.localScale.z);
        flipPage.flipX =true;
        if (currentPageIndex + 1 < pages.Length)
            flipPage.sprite = pages[currentPageIndex + 1];
    }

    void AfterBackwardFlip()
    {
        UpdateRightPage();
        flipPage.enabled = false;
        flipPage.flipX=false;
        rt.localRotation = Quaternion.identity;

        isFlipping = false;
        isLtoR= false;
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
    }

    /// <summary>
    /// RtoL = -1, LtoR = 1
    /// </summary>
    /// <param name="RY"></param>
    /// <param name="toward"></param>
    void Rol(float RY , int toward)
    {
        rt.rotation = Quaternion.Euler(0, RY, 0);
        //flipPage.rectTransform.position += new Vector3((float)318.5 * Time.deltaTime * toward, 0, 0);
        if (toward < 0) { rt.pivot = new Vector2(0, (float)0.5); }
        else { rt.pivot = new Vector2(1, (float)0.5); }
    }
    void ResetRy(){ _RY = 0; }

    float _RY = 0 ;

    private void Update()
    {
        if (isFlipping) 
        {
            if (isRtoL) 
            {
                Debug.Log("isRtoL");
                _RY +=90*Time.deltaTime;
                Rol(_RY,-1);
            }
            else
            {
                Debug.Log("isLtoR");
                _RY -= 90 * Time.deltaTime;
                Rol(_RY, 1);
            }
        }
    }
}





