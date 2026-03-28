using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurationAnimation: MonoBehaviour
{
    public string gameSceneName = "Game";
    public void ToNextScence()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {

            return;
        }
        SceneTransition.TransitionTo(gameSceneName);
    }
   
    
}
