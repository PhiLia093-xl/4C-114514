using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("SampleScene")]
    public string gameSceneName = "Scene01";

    [Header("UI Panels")]
    [Tooltip("SettingsPanel")]
    public GameObject settingsPanel;

    // 开始游戏：加载游戏场景
    public void StartGame()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            
            return;
        }
        SceneTransition.TransitionTo(gameSceneName);
    }

    // 打开设置：显示设置面板
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    // 关闭设置：隐藏设置面板
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // 退出游戏
    public void QuitGame()
    { 
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
        }
    }
}