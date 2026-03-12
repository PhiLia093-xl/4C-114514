/*
 *该类负责描述建筑列表，实现其生成与销毁 
*/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BuildingListGenerator : MonoBehaviour
{
    [Header("数据源")]
    public List<BuildingGroup> buildingGroups; //建筑组

    [Header("UI 引用")]
    public GameObject slotPrefab; // 刚才做的格子的预制体
    public Transform contentTransform; // ScrollView 的 Content
    public GameObject scrollView; // ScrollView 组件

    [HideInInspector]
    public List<BuildingData> currDataList; //当前显示的数据列表

    private List<GameObject> pool=new List<GameObject>();//对象池
    

    public void OpenMenu(string name) //打开建筑列表
    {
        if(scrollView!=null)scrollView.SetActive(true);
        else Debug.LogError("ScrollView未设置");
        BuildingGroup currGroup = buildingGroups.Find(group => group.fieldName == name);
        if(currGroup == null)
        {
            Debug.LogError("未找到对应的建筑组");
            return;
        }
        else
            Debug.Log("准备生成");
        GenerateList(currGroup.buildingGroup);
    }

    public void CloseMenu()  //关闭建筑列表
    {
        if(scrollView!=null)scrollView.SetActive(false);
        else Debug.LogError("ScrollView未设置");
    }
    public void GenerateList(List<BuildingData> dataList) //基本生成逻辑
    {
        Debug.Log("开始生成建筑列表");
        currDataList = dataList; //更新当前数据列表
        // 1. 清空旧格子(根据对象池操作)
        foreach (var obj in pool) obj.SetActive(false);
        
        // 2. 遍历数据，对象池有的就启动，没有就创建
        for (int i = 0; i < dataList.Count;i++)
        {
            if (dataList[i].count<= 0) continue; //如果数量为0就跳过
            GameObject slot=null;
            if(i<pool.Count)                     //够用就直接启用
            {
                Debug.Log("启用已有格子");
                slot =pool[i];
                slot.SetActive(true);
            }
            else                                 //不够用就创建新的
            {
                Debug.Log("创建新的格子");
                if (slotPrefab != null) {
                    if (contentTransform == null) Debug.LogError("Content Transform未设置");
                    else
                    {
                        slot = Instantiate(slotPrefab, contentTransform);
                        pool.Add(slot);
                    }
                }
                else Debug.LogError("格子预制体未设置");
                
            }
            slot.GetComponent<BuildingSlotUI>().Init(dataList[i]);//格子与数据分离，Init覆盖原有数据
        }
    }

   
}
