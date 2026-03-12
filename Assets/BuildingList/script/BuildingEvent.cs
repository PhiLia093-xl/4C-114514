/*
 *该脚本用于管理事件 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewBuilding", menuName = "Event/BuildingEvent")]
public class BuildingEvent : ScriptableObject
{
    private Action<BuildingData> _listeners;
    public void Raise(BuildingData data)
    {
        _listeners?.Invoke(data);
    }
    public void Register(Action<BuildingData> listener)
    {
        _listeners += listener;
    }
    public void Unregister(Action<BuildingData> listener)
    {
        _listeners -= listener;
    }
}
