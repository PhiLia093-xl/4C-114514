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
    public float minLoadingTime = 10f;

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

        // 1. 淡出
        yield return StartCoroutine(Fade(1f));

        // 2. 显示图标和云层
        if (loadingIcon != null) loadingIcon.SetActive(true);
        if (cloudBlur != null) cloudBlur.SetActive(true);

        // 3. 开始异步加载
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        float startTime = Time.time;

        // 等待加载完成
        while (!asyncLoad.isDone)
            yield return null;

        float elapsed = Time.time - startTime;
        Debug.Log($"加载完成用时：{elapsed:F2} 秒");


        if (elapsed < minLoadingTime)
        {
            float waitTime = minLoadingTime - elapsed;
            Debug.Log($"额外等待 {waitTime:F2} 秒");
            yield return new WaitForSeconds(waitTime);
        }

        // 5. 隐藏图标和云层，然后淡入
        if (loadingIcon != null) loadingIcon.SetActive(false);
        if (cloudBlur != null) cloudBlur.SetActive(false);

        yield return StartCoroutine(Fade(0f));

        isTransitioning = false;
    
}

    private IEnumerator Fade(float targetAlpha)
    {
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
}