using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public int width, height;
    GameGrid placementGrid;
    public GameGridVisualizer gameGridVisualizer;
    private Dictionary<Vector3Int, StructureModel> temporaryRoadobjects = new Dictionary<Vector3Int, StructureModel>();
    private Dictionary<Vector3Int, StructureModel> structureDictionary = new Dictionary<Vector3Int, StructureModel>();
    public Dictionary<Vector3Int, StructureModel> restoredOverlayedStructures = new Dictionary<Vector3Int, StructureModel>();

    public GameObject fillObject;
    private void Start()
    {
        placementGrid = new GameGrid(width, height);
        gameGridVisualizer.Initialize(placementGrid);

    }


    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
    }

    internal bool CheckIfPositionInBound(Vector3Int position)
    {
        
        if (position.x >= 0 && position.x < width && position.z >= 0 && position.z < height)
        {

            return true;
        }

        return false;
    }

    internal void PlaceObjectOnTheMap(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x, position.z] = type;
        StructureModel structure = CreateANewStructureModel(position, structurePrefab, type);
        structureDictionary.Add(position, structure);
        DestroyNatureAt(position);
    }

    private void DestroyNatureAt(Vector3Int position)
    {
        RaycastHit[] hits = Physics.BoxCastAll(position + new Vector3(0, 0.5f, 0), new Vector3(0.5f, 0.5f, 0.5f), transform.up, Quaternion.identity, 1f, 1 << LayerMask.NameToLayer("Nature"));
        foreach (var item in hits)
        {
            Destroy(item.collider.gameObject);
        }
    }

    internal bool CheckIfPositionIsFree(Vector3Int position)
    {
        bool free = CheckIfPositionIsOfType(position, CellType.Empty) || CheckIfPositionIsOfType(position, CellType.Ground);
        return free;
    }

    private bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
    {

        return placementGrid[position.x, position.z] == type;
    }

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        if (structureDictionary.TryGetValue(position, out StructureModel existingStructure))
        {
                restoredOverlayedStructures.Add(position, structureDictionary[position]);
                Destroy(existingStructure.gameObject);
                structureDictionary.Remove(position);
        }
        placementGrid[position.x, position.z] = type;
        StructureModel structure = CreateANewStructureModel(position, structurePrefab, type);
        temporaryRoadobjects.Add(position, structure);
    }

    internal void FixOverlayed()
    {
        foreach (var overlayed in restoredOverlayedStructures)
        {
            if (!structureDictionary.ContainsKey(overlayed.Key) && !temporaryRoadobjects.ContainsKey(overlayed.Key)) // Check if the object isn't in the structureDictionary or temporaryRoadobjects
            {
 
                placementGrid[overlayed.Key.x, overlayed.Key.z] = CellType.Ground;
                StructureModel structure = CreateANewStructureModel(overlayed.Key, fillObject, CellType.Ground);
                structureDictionary.Add(overlayed.Key, structure); // Add the newly created structure to structureDictionary
                restoredOverlayedStructures.Remove(overlayed.Key);
            }
        }
    }

    //internal void ClearOverlayStorage()
    //{
    //    Debug.Log("CLEARING OVERLAY");
    //    restoredOverlayedStructures.Clear(); // Clear restoredOverlayedStructures since we've fixed the overlays

    //}

    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
        {
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return neighbours;
    }


    internal CellType GetTypeFor(Vector3Int position)
    {
        var neighbourVertices = placementGrid.GetCellOf(position.x, position.z);


        return neighbourVertices;
    }

    private StructureModel CreateANewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        GameObject structure = new GameObject(type.ToString());
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        var structureModel = structure.AddComponent<StructureModel>();
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    internal List<Vector3Int> GetPathBetween(Vector3Int startPosition, Vector3Int endPosition)
    {
        var resultPath = GridSearch.AStarSearch(placementGrid, new Point(startPosition.x, startPosition.z), new Point(endPosition.x, endPosition.z));
        List<Vector3Int> path = new List<Vector3Int>();
        foreach (Point point in resultPath)
        {
            path.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return path;
    }

    internal void RemoveAllTemporaryStructures()
    {
        foreach (var structure in temporaryRoadobjects.Values)
        {
            var position = Vector3Int.RoundToInt(structure.transform.position);
            placementGrid[position.x, position.z] = CellType.Empty;
            Destroy(structure.gameObject);
        }
        temporaryRoadobjects.Clear();
    }

    internal void AddtemporaryStructuresToStructureDictionary()
    {
        foreach (var structure in temporaryRoadobjects)
        {
            var position = Vector3Int.RoundToInt(structure.Value.transform.position);

            // Check if there is an existing structure at the target position in structureDictionary.
            if (structureDictionary.TryGetValue(position, out StructureModel existingStructure))
            {
                // Check if the existing structure is on CellType.Empty.
                if (placementGrid[position.x, position.z] == CellType.Empty )
                {
                        // Destroy the existing structure if it's on CellType.Empty.
                    Destroy(existingStructure.gameObject);
                    structureDictionary.Remove(position);


                }
            }

            structureDictionary.Add(structure.Key, structure.Value);
            DestroyNatureAt(structure.Key);
        }

        temporaryRoadobjects.Clear();
    }


    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        if (temporaryRoadobjects.ContainsKey(position))
            temporaryRoadobjects[position].SwapModel(newModel, rotation);
        else if (structureDictionary.ContainsKey(position))
            structureDictionary[position].SwapModel(newModel, rotation);
    }

    public void ClearGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {

                    Vector3Int position = new Vector3Int(x, 0, z);
                    if (structureDictionary.TryGetValue(position, out StructureModel structure))
                    {
                        Destroy(structure.gameObject);
                    }
                    placementGrid[x, z] = CellType.Empty;

            }
        }


        structureDictionary.Clear();

    }

    public void FillGrid()
    {
        ClearGrid();
        for (int x = 0; x < width-5; x++)
        {
            for (int z = 0; z < height-5; z++)
            {
                if (placementGrid[x, z] == CellType.Empty)//maybe some things will be excluded from clear so leaving this hear
                {
                    PlaceObjectOnTheMap(new Vector3Int(x, 0, z), fillObject, CellType.Empty);
                }
            }
        }

    }


}