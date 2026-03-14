using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private static SceneTransition instance;
    private bool isTransitioning;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 切换场景时不销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public static void TransitionTo(string SampleScene)
    {
        
        if (!instance.isTransitioning)
            instance.StartCoroutine(instance.TransitionRoutine(SampleScene));
    }

    private IEnumerator TransitionRoutine(string SampleScene)
    {
        isTransitioning = true;

        // 1. 淡出至黑色
        yield return StartCoroutine(Fade(1f));

        // 2. 异步加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SampleScene);
        while (!asyncLoad.isDone)
            yield return null;

        // 3. 淡入
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

// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
