using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviour
{
    private static MyGameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    string[] SceneNames =
        {
            "Scene01",
            "Scene02",
            "Scene03",
            "Scene04",
            "Scene05",
            "Scene06",
            "Scene07",
        };
    private void Start()
    {
        CameraController.onBuildingModeEx.AddListener(ToNextScene);
    }
    private void OnDestroy()
    {
        CameraController.onBuildingModeEx.RemoveListener(ToNextScene);
    }

    int currentCount = 0;

    public void ToNextScene() 
    {
        if(SaveManager.instance.saveData_P==null || SaveManager.instance.saveData_P._MeshBePlaced.Count == 0) { return; }
        if(currentCount == SaveManager.instance.saveData_P._MeshBePlaced.Count) {  return; }
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Scene07" || currentSceneName == "Main Scene") { return; }
        currentCount = SaveManager.instance.saveData_P._MeshBePlaced.Count;
        SceneTransition.TransitionTo(SceneNames[SaveManager.instance.saveData_P._MeshBePlaced.Count]);
    }
}
