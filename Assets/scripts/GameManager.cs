using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public RoadManager roadManager;
    public InputManager inputManager;
    public PlacementManager placementManager;
    public UIController uiController;

    public StructureManager structureManager;

    private void Start()
    {
        uiController.OnRoadPlacement += RoadPlacementHandler;
        uiController.onClearGrid += ClearGrid;
        uiController.onFillGrid += FillGrid;
        //uiController.OnHousePlacement += HousePlacementHandler;
        //uiController.OnSpecialPlacement += SpecialPlacementHandler;
        inputManager.OnMouseClick += HandleMouseClick;
        

    }
    private void FillGrid()
    {
        placementManager.FillGrid();
        Debug.Log("GRID FILLED");
    }
    private void ClearGrid()
    {
        placementManager.ClearGrid();
        Debug.Log("GRID CLEARED");
    }
    private void HandleMouseClick(Vector3Int position)
    {
        Debug.Log(position);
    }

    private void SpecialPlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceSpecial;
    }

    private void HousePlacementHandler()
    {
        ClearInputActions();
        inputManager.OnMouseClick += structureManager.PlaceHouse;
    }

    private void RoadPlacementHandler()
    {
        ClearInputActions();

        inputManager.OnMouseClick += roadManager.PlaceRoad;
        inputManager.OnMouseHold += roadManager.PlaceRoad;
        inputManager.OnMouseUp += roadManager.FinishPlacingRoad;
    }

    private void ClearInputActions()
    {
        inputManager.OnMouseClick = null;
        inputManager.OnMouseHold = null;
        inputManager.OnMouseUp = null;
    }

    private void Update()
    {
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
    }
}