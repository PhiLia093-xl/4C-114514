using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    //这个脚本是直接照教程搬的，很多地方理解不了，对于renderer和material这一块很多东西都不懂
    [SerializeField]
    private float previewYOfffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;//透明的预制体（用来示例的）

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;
    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.gameObject.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacement(GameObject prefab,Vector2 size)
    {
        previewObject = Instantiate(prefab);//此时生成的并不是带有透明材质的预制体
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2 size)
    {
        if (size.x > 0 || size.y > 0) 
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            //相当于是把指示器的大小变成了和展示的预制体的大小一样了
            cellIndicatorRenderer.material.mainTextureScale = size;//保持原有的网格粗细（不太清楚具体作用）
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for(int i =0;i<materials.Length;i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null; 
        }

    }

    public void UpdatePosition(Vector3 position,bool validity)
    {
        if(previewObject != null)
        {
            MovePreview(position);
            ApplyFeedBackToPreview(validity);
        }  
        MoveCursor(position);        
        ApplyFeedBackToCursor(validity);
    }

    private void ApplyFeedBackToPreview(bool validity)
    {
        UnityEngine.Color c = validity ? UnityEngine.Color.white : UnityEngine.Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }
    private void ApplyFeedBackToCursor(bool validity)
    {
        UnityEngine.Color c = validity ? UnityEngine.Color.white : UnityEngine.Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        //网格指示器的位置
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        //预制体的位置
        previewObject.transform.position = new Vector3(
            position.x, 
            position.y + previewYOfffset, 
            position.z);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2.one);
        ApplyFeedBackToCursor(false);
    }
}
