using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class BookCanvasChange : MonoBehaviour
{
     public GameObject[] canvas;
    public CameraController CameraController; 
    public Transform cameraTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private int lastIndex = 0;
    public Transform target;
    public Text Tip;
    private Color unColor = new Color(1, 1, 1, 0);

    private string[] books =
    {
        null,
        "外朝中路",
        "内廷中路",
        "内廷西路",
        "内廷东路",
        "内廷外西路",
        "内廷外东路"
    };


    public GameObject[] canvases;

    
    void Start()
    {
        ShowCanvas0(0);
        Tip.color = unColor;
    }


    public void ShowCanvas(int index)
    {
        //SaveManager.instance.TestForBook();

        

        Debug.Log($"将要打开{index}");
        if (index != 7 && index != 1 && index!=0) 
        { 
            if (!SaveManager.instance.BookBeReadOrNot(index - 1))
            {
                Debug.Log($"{index}打开失败，因为{index - 1}还没有被读完");
                Tip.text = $"无法打开{books[index]}，因为{books[index-1]}还没有读完";
                Tip.color = Color.white;
                Invoke("HideTip", 2f);
                return; 
            } 
        }
        
        StartCoroutine(SwitchCanvasWithTransition(index));
    }

    private void HideTip() 
    {
        Tip.color = unColor;
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







        if (index == 7 )
        {
            DisableCamera();
            if (lastIndex == 0)
            {
                originalPosition = cameraTransform.position;
                originalRotation = cameraTransform.rotation;

                cameraTransform.position = target.position;
                cameraTransform.rotation = target.rotation;
            }

           

            if (CameraController.isInteracting == true)
            {
                for (int j = 0; j < canvas.Length; j++)
                {
                    canvas[j].SetActive(false);


                }
            }


        }

        else if (index == 0)
        {
            EnableCamera();
            cameraTransform.position = originalPosition;
            cameraTransform.rotation = originalRotation;


             if (CameraController.isInteracting == true)
             {
                for (int j = 0; j < canvas.Length; j++)
                {
                    canvas[j].SetActive(true);

                }
             }
        }
        

        lastIndex = index;

        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].SetActive(i == index);
            
        }

        
    }
    

    // 禁止摄像机控制
    public void DisableCamera()
    {
        if (CameraController != null)
        {
            CameraController.enabled = false;
        }
    }

    // 恢复摄像机控制
    public void EnableCamera()
    {
        if (CameraController != null)
        {
            CameraController.enabled = true;
        }
    }

}

