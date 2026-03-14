using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    public DeleteEvent deleteEvent;
    public BuildingGroup database;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData furnitureData,
                         ObjectPlacer objectPlacer,
                         DeleteEvent deleteEvent,BuildingGroup database)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.database = database;
        previewSystem.StartShowingRemovePreview();
        this.deleteEvent = deleteEvent;
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
      
        GridData selectedData = null;
        if(furnitureData.CanPlaceObjectAt(gridPosition,Vector2Int.one) == false)
        {
            selectedData = furnitureData;
        }
        else if(floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = floorData;
        }

        if(selectedData == null)
        {
            //可以添加音效
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
           
            if (gameObjectIndex == -1)
                return;
            int Index = database.buildingGroup.FindIndex(data => data.ID == selectedData.GetIDAtPosition(gridPosition)); //获取buildingGroup下标
            deleteEvent.Raise(database.buildingGroup[Index]);

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
            Debug.Log(gameObjectIndex);
            
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
        if (selectedData == null)
        {
            //可以添加音效
            Debug.Log($"找不到可删除的物体！当前点击的网格坐标是: {gridPosition}");
        }
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) &&
            floorData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }
}
