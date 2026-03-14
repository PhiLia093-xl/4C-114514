/*
 *该类用于存放建筑物的数据 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Construction/BuildingData")]
public class BuildingData : ScriptableObject, ISerializationCallbackReceiver
{
    [Header("配置数据 (永久保存)")]
    public string buildingName; //建筑名称
    public Sprite icon;         //建筑图标
    public GameObject prefab;   //建筑预制体
    public int maxCount;        //建筑最大数量
    public int ID;              //查找ID
    public Vector2Int size=Vector2Int.one; //建筑占地的宽和高

    [Header("运行时数据 (自动重置)")]
    [System.NonSerialized]
    public int count;           //建筑数量
   
    // 当资产被加载（进入播放模式或打包后启动）时执行
    public void OnAfterDeserialize()
    {
        Debug.Log("资产已重置");
        count=maxCount;
    }
    // 加载前执行
    public void OnBeforeSerialize() { }
}
