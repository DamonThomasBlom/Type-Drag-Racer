using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class WFCGenerator : SerializedMonoBehaviour
{
    public int width = 10;   // Width of the output grid
    public int height = 10;  // Height of the output grid

    public List<GridElement> inputSamples;  // List of input samples

    private GridElement[,] outputGrid;     // Output grid

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        // Initialize the output grid
        outputGrid = new GridElement[width, height];

        // Iterate through each cell in the output grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.Log("Loop");
                // Initialize the cell with all possible samples
                outputGrid[x, y] = new GridElement(inputSamples);
            }
        }

        PrintGrid();

        // Perform the WFC iteration until the output grid is filled
        while (!IsOutputGridFilled())
        {
            Debug.Log("Iterations");

            // Find the cell with the fewest possible options
            Vector2Int cellPosition = FindCellWithFewestOptions();

            // If no valid cell is found, the generation failed
            if (cellPosition == Vector2Int.zero)
            {
                Debug.LogError("Failed to generate world!");
                return;
            }

            // Collapse the cell by selecting a valid element
            GridElement selectedElement = outputGrid[cellPosition.x, cellPosition.y].Collapse();

            // Update the output grid with the selected element
            outputGrid[cellPosition.x, cellPosition.y] = selectedElement;

            // Propagate the changes to neighboring cells
            PropagateChanges(cellPosition);
        }

        // The generation is complete, you can use the output grid for your game environment
        // Instantiate prefabs or apply other modifications as needed
        foreach (var element in outputGrid)
        {
            Debug.Log(element.Element);
        }
    }

    private bool IsOutputGridFilled()
    {
        // Check if any cell in the output grid still has multiple possibilities
        foreach (GridElement cell in outputGrid)
        {
            if (!cell.IsCollapsed)
            {
                return false;
            }
        }

        return true;
    }

    private Vector2Int FindCellWithFewestOptions()
    {
        //int minOptions = int.MaxValue;
        //Vector2Int cellPosition = Vector2Int.zero;

        //// Iterate through each cell in the output grid
        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        GridElement cell = outputGrid[x, y];

        //        Debug.Log($"Collapsed: {cell.IsCollapsed}  Options: {cell.Options.Count} Min Options: {minOptions}");
        //        Debug.Log("Cell Pos: " + cellPosition);

        //        // If the cell is not collapsed and has fewer options than the current minimum
        //        if (!cell.IsCollapsed && cell.Options.Count < minOptions)
        //        {
        //            minOptions = cell.Options.Count;
        //            cellPosition = new Vector2Int(x, y);
        //        }
        //    }
        //}

        //return cellPosition;

        int minOptions = int.MaxValue;
        Vector2Int cellPosition = Vector2Int.zero;
        bool foundUncollapsedCell = false;

        // Iterate through each cell in the output grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridElement cell = outputGrid[x, y];

                // If the cell is not collapsed and has fewer options than the current minimum
                if (!cell.IsCollapsed && cell.Options.Count < minOptions)
                {
                    minOptions = cell.Options.Count;
                    cellPosition = new Vector2Int(x, y);
                    foundUncollapsedCell = true;
                }
            }
        }

        // If no uncollapsed cells were found, return Vector2Int.zero
        if (!foundUncollapsedCell)
        {
            return Vector2Int.zero;
        }

        return cellPosition;
    }

    public void PrintGrid()
    {
        string output = "";
        for (int y = 0; y < height; y++)
        {
            string row = "";
            for (int x = 0; x < width; x++)
            {
                GridElement element = outputGrid[x, y];
                string elementString = element.IsCollapsed ? element.Element.ToString() : "X";
                row += elementString + " ";
            }

            output += row + "\n";
        }
        Debug.Log(output);
    }

    private void PropagateChanges(Vector2Int cellPosition)
    {
        // Get the selected element at the given cell position
        GridElement selectedElement = outputGrid[cellPosition.x, cellPosition.y];

        // Iterate through the neighboring cells
        foreach (Vector2Int neighborPosition in GetNeighborPositions(cellPosition))
        {
            GridElement neighbor = outputGrid[neighborPosition.x, neighborPosition.y];

            // Remove the selected element as a possible option from the neighboring cells
            neighbor.RemoveOption(selectedElement.Element);
        }
    }

    private List<Vector2Int> GetNeighborPositions(Vector2Int cellPosition)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Add the positions of the neighboring cells (left, right, top, bottom)
        neighbors.Add(new Vector2Int(cellPosition.x - 1, cellPosition.y));
        neighbors.Add(new Vector2Int(cellPosition.x + 1, cellPosition.y));
        neighbors.Add(new Vector2Int(cellPosition.x, cellPosition.y - 1));
        neighbors.Add(new Vector2Int(cellPosition.x, cellPosition.y + 1));

        // You can add more complex neighbor detection logic based on your grid structure

        return neighbors;
    }
}

[System.Serializable]
public class GridElement
{
    public int Element;
    public List<int> Options;
    public bool IsCollapsed;

    public GridElement(List<GridElement> samples)
    {
        // Copy the samples as options
        Options = new List<int>();
        foreach (GridElement sample in samples)
        {
            Debug.Log("add option");
            Options.Add(sample.Element);
        }

        IsCollapsed = false;
    }

    public GridElement Collapse()
    {
        // Randomly select one of the remaining options
        int randomIndex = Random.Range(0, Options.Count);
        int selectedOption = Options[randomIndex];

        // Create a new collapsed grid element with the selected option
        GridElement collapsedElement = new GridElement(new List<GridElement>());
        collapsedElement.Element = selectedOption;
        collapsedElement.IsCollapsed = true;

        return collapsedElement;
    }

    public void RemoveOption(int option)
    {
        // Remove the specified option from the remaining options
        Options.Remove(option);
    }
}
