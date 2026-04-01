using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    MeshInstance _meshInstance;
    PreviewSystem previewSystem;
    BuildingGroup database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    public BuildingEvent buildingEvent;

    public PlacementState(int iD,
                          MeshInstance meshInstance,
                          PreviewSystem previewSystem,
                          BuildingGroup database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          BuildingEvent buildingEvent)
    {
        ID = iD;
        _meshInstance = meshInstance;
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
    
    public void OnAction(Vector3 pos , Vector2 box )
    {
        //检查放置的有效性
        if (_meshInstance.BoxIsUsedOrNot(box) == true)
            return;
        
        GameObject Buiding = objectPlacer.PlaceObject(database.buildingGroup[selectedObjectIndex].prefab, pos);
        _meshInstance.BrforePlace(box, selectedObjectIndex,
                            Buiding);
        //GridData selectedData = database.buildingGroup[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        //selectedData.AddObjectAt(pos,
        //    database.buildingGroup[selectedObjectIndex].size,
        //    database.buildingGroup[selectedObjectIndex].ID,
        //    index);
        //selectedData.AddObjectAt(pos,
        //    database.buildingGroup[selectedObjectIndex].size,
        //    database.buildingGroup[selectedObjectIndex].ID,
        //    ID); //T
        //放置完物体之后将该点设置为不能放置
        //previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);


        buildingEvent.Raise(database.buildingGroup[selectedObjectIndex]);
    }

    private bool CheckPlacementValidity(Vector3 gridPosition, int selectedObjectIndex)
    {
        //判断是地板还是建筑
        GridData selectedData = database.buildingGroup[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.buildingGroup[selectedObjectIndex].size);

    }

    public void UpdateState(Vector3 pos , Vector2 box)
    {
        bool placementValidity = !_meshInstance.BoxIsUsedOrNot(box);
        previewSystem.UpdatePosition(pos, placementValidity);
    }
}
