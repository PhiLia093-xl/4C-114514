using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Search;
using System.Runtime.InteropServices.WindowsRuntime;


public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDataBaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData,furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IBuildingState buildingState;

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
        buildingState = new PlacementState(ID,grid,preview,database,floorData,furnitureData,objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
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
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 cellOriginWorldPos = grid.CellToWorld(gridPosition);
       
        buildingState.OnAction(gridPosition);
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

        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()   
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        //把鼠标的位置转化为网格坐标
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        //使用 CellToWorld 获取单元格左下角坐标
        Vector3 cellOriginWorldPos = grid.CellToWorld(gridPosition);

        //如果未移动网格指示器，可以停止更新
        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
      
    }

}
