using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;



public class BookCanvasChange : MonoBehaviour
{
    public GameObject[] canvases;

    
        void Start()
        {
            ShowCanvas0(0); 
        }


    public void ShowCanvas(int index)
    {
        StartCoroutine(SwitchCanvasWithTransition(index));
    }

    public void ShowCanvas0(int index)
    {
        
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].SetActive(i == index);
        }
    }

    IEnumerator SwitchCanvasWithTransition(int index)
    {
        
        SceneTransition.TransitionTo();

        
        yield return new WaitForSeconds(3f);

        
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].SetActive(i == index);
        }
    }

   
}

