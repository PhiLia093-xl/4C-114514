using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("基础设置")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("加载指示器")]
    public GameObject loadingIcon;
    [Tooltip("加载图标最短显示时间（秒）")]
    public float minLoadingTime = 2f;

    [Header("云层模糊特效")]
    public GameObject cloudBlur;

    private static SceneTransition instance;
    private bool isTransitioning;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.raycastTarget = false;
        }
    }

    public static void TransitionTo(string sceneName)
    {
        if (instance == null)
        {
            Debug.LogError("SceneTransition 实例不存在！");
            return;
        }
        if (!instance.isTransitioning)
            instance.StartCoroutine(instance.TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(1f));

        if (loadingIcon != null) loadingIcon.SetActive(true);
        if (cloudBlur != null) cloudBlur.SetActive(true);

      

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        float startTime = Time.time;

        while (!asyncLoad.isDone)
            yield return null;

        float elapsed = Time.time - startTime;
        if (elapsed < minLoadingTime)
            yield return new WaitForSeconds(minLoadingTime - elapsed);

        if (loadingIcon != null) loadingIcon.SetActive(false);
        if (cloudBlur != null) cloudBlur.SetActive(false);

        yield return StartCoroutine(Fade(0f));

        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null)
            yield break;

        fadeImage.raycastTarget = (targetAlpha == 1f);

        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }


    public static void TransitionTo()
    {
        if (instance == null)
        {
            Debug.LogError("SceneTransition 实例不存在！");
            return;
        }
        if (!instance.isTransitioning)
            instance.StartCoroutine(instance.TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(1f));

        if (loadingIcon != null) loadingIcon.SetActive(true);
        if (cloudBlur != null) cloudBlur.SetActive(true);



        float startTime = Time.time;

        float elapsed = Time.time - startTime;
        if (elapsed < minLoadingTime)
            yield return new WaitForSeconds(minLoadingTime - elapsed);

        if (loadingIcon != null) loadingIcon.SetActive(false);
        if (cloudBlur != null) cloudBlur.SetActive(false);

        yield return StartCoroutine(Fade(0f));

        isTransitioning = false;
    }


}
