using System;
using System.Collections.Generic;
using UnityEngine;

public class GameGridVisualizer : MonoBehaviour
{
    public GameObject gridCellPrefab; // Reference to the grid cell prefab.
    public Transform gridContainer; // Optional: You can create an empty GameObject to parent the grid cells.

    private GameGrid _grid; // Reference to your GameGrid instance.

    // Initialize the grid and create visual representation.
    public void Initialize(GameGrid grid)
    {
        Debug.Log("INIT");
        _grid = grid;
        CreateGridVisual();
    }

    private void CreateGridVisual()
    {
        Debug.Log("1");

        for (int x = 0; x <= _grid.Width; x++)  // Changed to <= to add 1 more unit
        {
            Debug.Log("2");

            for (int y = 0; y <= _grid.Height; y++)  // Changed to <= to add 1 more unit
            {
                Debug.Log("3");

                Vector3 cellPosition = new Vector3(x, 0, y );

                // Add the position of the GameObject to position the grid around it
                cellPosition += transform.position;

                GameObject gridCell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity);
                Debug.Log("INSTANTIATED ");

                // Ensure the x and y indices are within the grid boundaries before accessing.
                if (x < _grid.Width && y < _grid.Height)
                {
                    // Set the grid cell's material or color based on the corresponding GameGrid cell type.
                    CellType cellType = _grid[x, y];
                    SetGridCellColor(gridCell, cellType);
                }
                else
                {
                    // Default behavior for the extra cells. You can set them to a default color or just make them transparent.
                    Renderer renderer = gridCell.GetComponent<Renderer>();
                    renderer.material.color = Color.clear;  // Set to transparent. Adjust as needed.
                }

                // Optional: Parent the grid cell to the grid container.
                if (gridContainer != null)
                {
                    gridCell.transform.parent = gridContainer.transform;
                }
            }
        }
    }




    private void SetGridCellColor(GameObject gridCell, CellType cellType)
    {
        // Adjust the material or color of the grid cell based on the cell type.
        // You can use different materials or assign colors here.
        Renderer renderer = gridCell.GetComponent<Renderer>();
        switch (cellType)
        {
            case CellType.Road:
                renderer.material.color = Color.gray;
                break;
            case CellType.Structure:
                renderer.material.color = Color.blue;
                break;
            case CellType.SpecialStructure:
                renderer.material.color = Color.green;
                break;
                // Add more cases for different cell types as needed.
        }
    }
}
