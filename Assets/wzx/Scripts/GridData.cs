using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    //已放置的物体占用的网格数据
    Dictionary<Vector3, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3 gridPosition,Vector2 objectSize,int ID,int placedObjectIndex)
    {
        //用来存储将要占用的位置的列表
        List<Vector3> positionsToOccupied = CaculatePositions(gridPosition, objectSize);
        //存储将要放置的物体占用位置的数据
        PlacementData data = new PlacementData(positionsToOccupied, ID, placedObjectIndex);
        //检查已放置的物体占用的网格数据是否已经包含将要放置的位置
        foreach(var pos in positionsToOccupied)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            //如果这个位置未被占用，就可以放置，并存储数据
            placedObjects[pos] = data;
        }
    }

    //计算物体将要占用的位置的坐标
    private List<Vector3> CaculatePositions(Vector3 pos, Vector2 objectSize)
    {
        List<Vector3> returnVal = new();
        for(int x = 0;x<objectSize.x;x++)
        {
            for(int y =0;y<objectSize.y;y++)
            {
                //通过循环嵌套，把物体将要占用的网格存储进列表中
                returnVal.Add(pos + new Vector3Int(x,0,y));
            }
        }
        return returnVal;
    }

    //判断能否在这个位置放置相应的物体
    public bool CanPlaceObjectAt(Vector3 gridPosition, Vector2 objectSize)
    {
        List<Vector3> positionToOccupy = CaculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3 gridPosition)
    {
        if(placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }
        return placedObjects[gridPosition].PlacedObjectIndex; 
    }

    internal void RemoveObjectAt(Vector3 gridPosition)
    {
        foreach(var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    public int GetIDAtPosition(Vector3 gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
        {
            throw new Exception($"No object placed at this position {gridPosition}");
        }
        return placedObjects[gridPosition].ID;
    }
}
public class PlacementData
{
    //已被占用的网格的位置
    public List<Vector3> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    //初始化数据
    public PlacementData(List<Vector3> occupiedPositions,int iD,int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }

}

