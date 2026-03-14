using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    BuildingGroup database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    public BuildingEvent buildingEvent;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          BuildingGroup database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          BuildingEvent buildingEvent)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;
        this.buildingEvent = buildingEvent;

        //查找是否有这个ID的物体
        selectedObjectIndex = database.buildingGroup.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacement(
            database.buildingGroup[selectedObjectIndex].prefab,
            database.buildingGroup[selectedObjectIndex].size);
        }
        else
        {
            throw new System.Exception($"No Object With ID {ID}");
        }

        this.buildingEvent = buildingEvent;
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }
    
    public void OnAction(Vector3Int gridPosition)
    {
        //检查放置的有效性
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;
        int index = objectPlacer.PlaceObject(database.buildingGroup[selectedObjectIndex].prefab, grid.CellToWorld(gridPosition));
        GridData selectedData = database.buildingGroup[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        selectedData.AddObjectAt(gridPosition,
            database.buildingGroup[selectedObjectIndex].size,
            database.buildingGroup[selectedObjectIndex].ID,
            index);
        //放置完物体之后将该点设置为不能放置
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
        buildingEvent.Raise(database.buildingGroup[selectedObjectIndex]);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //判断是地板还是建筑
        GridData selectedData = database.buildingGroup[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.buildingGroup[selectedObjectIndex].size);

    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }
}
