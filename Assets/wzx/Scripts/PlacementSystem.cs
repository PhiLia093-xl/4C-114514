using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;


public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    //[SerializeField]
    //private Grid grid;

    [SerializeField]
    private BuildingGroup database;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData,furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3 lastDetectedPosition = Vector3.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    [Header("事件")]
    public BuildingEvent buildingEvent;
    public DeleteEvent deleteEvent;

    IBuildingState buildingState;

    public MeshInstance meshInstance;
    private (Vector3, Vector2) BoxBeUsing;

    private void Start()
    {
        StopPlacement();
        furnitureData = new ();
        floorData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, meshInstance , preview,database,floorData,furnitureData,objectPlacer,buildingEvent);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(ID , meshInstance, preview, floorData, furnitureData, objectPlacer,deleteEvent,database);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnClicked += StopPlacement;
    }
    private void PlaceStructure()
    {
        if (buildingState == null) return;
        if (inputManager.IsPointerUI())
        {
            return;
        }
        //Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        //Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        //Vector3 cellOriginWorldPos = grid.CellToWorld(gridPosition);
       


        buildingState.OnAction(BoxBeUsing.Item1 , BoxBeUsing.Item2);
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    //判断是地板还是建筑
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : furnitureData;
    //    return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);

    //}

    public void StopPlacement()
    {
        if (buildingState == null) return;

        buildingState.EndState();
        //inputManager.OnClicked -= PlaceStructure;
        //inputManager.OnExit -= StopPlacement;
        gridVisualization.SetActive(false);
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()   
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Debug.Log($"hitpoint{mousePosition}");
        BoxBeUsing = meshInstance.GetPos(mousePosition);
        

        ////把鼠标的位置转化为网格坐标
        //Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        ////使用 CellToWorld 获取单元格左下角坐标
        //Vector3 cellOriginWorldPos = grid.CellToWorld(gridPosition);

        //如果未移动网格指示器，可以停止更新
        if(lastDetectedPosition != BoxBeUsing.Item1)
        {
            buildingState.UpdateState(BoxBeUsing.Item1 , BoxBeUsing.Item2);
            lastDetectedPosition = BoxBeUsing.Item1;
        }
      
    }

}
