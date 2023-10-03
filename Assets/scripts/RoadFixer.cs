using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadFixer : MonoBehaviour
{
    public GameObject deadEnd, roadStraight, corner, threeWay, fourWay;

    public void FixRoadAtPosition(PlacementManager placementManager, Vector3Int temporaryPosition)
    {
        var result = placementManager.GetNeighbourTypesFor(temporaryPosition);
        if (result.Length != 4)
        {
            Debug.LogError("Unexpected number of neighbors!");
            return;
        }

        int roadCount = result.Count(x => x == CellType.Road);
        switch (roadCount)
        {
            case 0:
            case 1:
                CreateDeadEnd(placementManager, result, temporaryPosition);
                break;
            case 2:
                if (!TryCreateStraightRoad(placementManager, result, temporaryPosition))
                {
                    CreateCorner(placementManager, result, temporaryPosition);
                }
                break;
            case 3:
                Create3Way(placementManager, result, temporaryPosition);
                break;
            default:
                Create4Way(placementManager, temporaryPosition);
                break;
        }
    }

    private void Create4Way(PlacementManager placementManager, Vector3Int temporaryPosition)
    {
        placementManager.ModifyStructureModel(temporaryPosition, fourWay, Quaternion.identity);
    }

    private void Create3Way(PlacementManager placementManager, CellType[] neighbors, Vector3Int position)
    {
        var rotations = new List<Quaternion>
        {
            Quaternion.identity,
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 270, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            if (neighbors.Skip(i).Take(3).All(n => n == CellType.Road))
            {
                placementManager.ModifyStructureModel(position, threeWay, rotations[i]);
                return;
            }
        }
    }

    private void CreateCorner(PlacementManager placementManager, CellType[] neighbors, Vector3Int position)
    {
        var rotations = new List<Quaternion>
        {
            Quaternion.identity,
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 270, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i] == CellType.Road && neighbors[(i + 1) % 4] == CellType.Road)
            {
                placementManager.ModifyStructureModel(position, corner, rotations[i]);
                return;
            }
        }
    }

    private bool TryCreateStraightRoad(PlacementManager placementManager, CellType[] neighbors, Vector3Int position)
    {
        if (neighbors[0] == CellType.Road && neighbors[2] == CellType.Road)
        {
            placementManager.ModifyStructureModel(position, roadStraight, Quaternion.identity);
            return true;
        }
        if (neighbors[1] == CellType.Road && neighbors[3] == CellType.Road)
        {
            placementManager.ModifyStructureModel(position, roadStraight, Quaternion.Euler(0, 90, 0));
            return true;
        }
        return false;
    }

    private void CreateDeadEnd(PlacementManager placementManager, CellType[] neighbors, Vector3Int position)
    {
        var rotations = new List<Quaternion>
        {
            Quaternion.Euler(0, 270, 0),
            Quaternion.identity,
            Quaternion.Euler(0, 90, 0),
            Quaternion.Euler(0, 180, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            if (neighbors[i] == CellType.Road)
            {
                placementManager.ModifyStructureModel(position, deadEnd, rotations[i]);
                return;
            }
        }
    }
}
