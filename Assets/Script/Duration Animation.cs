using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
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
