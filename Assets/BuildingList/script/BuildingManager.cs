/*
*   该类用于管理对BuildingData数据的修改
*   例如建造数量的改变，建造被拆毁等
*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public BuildingEvent buildingEvent; //事件引用
    public DeleteEvent deleteEvent; //事件引用
    public BuildingListGenerator listGenerator; //建筑列表生成器引用
    public PlacementSystem placementSystem; //放置系统引用
    private void Start()
    {
        if(buildingEvent != null) buildingEvent.Register(OnBuildingPlaced); //注册事件监听
        if(deleteEvent!=null)deleteEvent.Register(OnBuildingDelete); //注册事件监听
    }
    public void OnBuildingPlaced(BuildingData data) //方法，处理建筑放置后的结果
    {
        data.count--;
        if (data.count <= 0)
        {
            if(listGenerator!=null)listGenerator.GenerateList(listGenerator.currDataList); //如果数量为0，刷新建筑列表
            else Debug.LogError("建筑列表生成器未设置");
            if (placementSystem != null) placementSystem.StopPlacement();
            else Debug.LogError("放置系统未设置");
        }
    }

    public void OnBuildingDelete(BuildingData data) //方法，处理建筑被拆毁后的结果
    {
       data.count++;
       if (data.count > 0)
        {
            if(listGenerator!=null)listGenerator.GenerateList(listGenerator.currDataList); //如果数量大于0，刷新建筑列表
            else Debug.LogError("建筑列表生成器未设置");
        }
    }
}

