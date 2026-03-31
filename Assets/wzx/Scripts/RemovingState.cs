using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    int ID;
    private int gameObjectIndex = -1;
    MeshInstance _meshInstance;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    public DeleteEvent deleteEvent;
    public BuildingGroup database;

    public RemovingState(int ID,
                         MeshInstance meshInstance,
                         PreviewSystem previewSystem,
                         GridData floorData,
                         GridData furnitureData,
                         ObjectPlacer objectPlacer,
                         DeleteEvent deleteEvent,BuildingGroup database)
    {
        this.ID = ID;
        _meshInstance = meshInstance;
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

    public void OnAction(Vector3 pos , Vector2 box)
    {
      
        GridData selectedData = null;
        if(furnitureData.CanPlaceObjectAt(pos,Vector2.one) == false)
        {
            selectedData = furnitureData;
        }
        //else if(floorData.CanPlaceObjectAt(gridPosition, Vector2.one) == false)
        //{
        //    selectedData = floorData;
        //}

        if(selectedData == null)
        {
            //可以添加音效
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(pos);
           
            if (gameObjectIndex == -1)
                return;
            int Index = database.buildingGroup.FindIndex(data => data.ID == selectedData.GetIDAtPosition(pos)); //获取buildingGroup下标
            deleteEvent.Raise(database.buildingGroup[Index]);


            _meshInstance.OnBuildingDelet(box);

            selectedData.RemoveObjectAt(pos);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
            Debug.Log(gameObjectIndex);
            
        }
        previewSystem.UpdatePosition(pos, CheckIfSelectionIsValid(pos));
        if (selectedData == null)
        {
            //可以添加音效
            Debug.Log($"找不到可删除的物体！当前点击的网格坐标是: {box}");
        }
    }

    private bool CheckIfSelectionIsValid(Vector3 gridPosition)
    {
        return !(furnitureData.CanPlaceObjectAt(gridPosition, Vector2.one) &&
            floorData.CanPlaceObjectAt(gridPosition, Vector2.one));
    }

    public void UpdateState(Vector3 pos , Vector2 box )
    {
        previewSystem.UpdatePosition(pos, !_meshInstance.BoxIsUsedOrNot(box));
    }
}
