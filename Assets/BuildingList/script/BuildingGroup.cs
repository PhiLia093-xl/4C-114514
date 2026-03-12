/*
 * 用于描述各个区域所需要的建筑组
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewBuilding", menuName = "Construction/BuildingGroup")]
public class BuildingGroup:ScriptableObject
{
    public List<BuildingData> buildingGroup;
    public string fieldName;

}
