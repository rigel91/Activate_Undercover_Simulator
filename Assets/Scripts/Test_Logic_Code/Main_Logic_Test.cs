using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Main_Logic_Test : MonoBehaviour
{
    /*
        ChatGPT prompt:
        I am creating a game in Unity where there is a 6x6 grid of tiles and there is a tetris like shape that the player has to find this shape in a randomly spot in the grid at a different rotation. I have implemented functions already where I can put the shape in a random rotation and put it into the grid where the correct shape has true values on its tiles and the other tiles have false values. I want to know how to put random shapes and random tiles to make it harder for the player to find the shape, but make it so that there is no mistakenly duplicated identical shape in the grid. For example, lets say there is a Z tetris like shape and its placed randomly in the grid at a 90 degree rotation. I want to generate random noise and possibly one identical but flipped shape to trick the player, but want to make it so there is no duplicate of the actual piece that the player must find.

        The cleanest way to do this in Unity is:
            1. Pick shape
            2. Pick rotation
            3. Place real shape
            4. Optionally place fake mirrored shape
            5. Fill rest with random noise
            6. Scan grid for duplicate real shapes
            7. If duplicate then regenerate noise(steps 5-7) or adjust the duplicate

    */

    // Grid Dimensions
    private int width = 6;
    private int height = 6;
    private int gridSpace = 6;

    private Tile_Data[,] activeGrid;

    // Prefabs for 3D grid
    [SerializeField]
    private GameObject gridCellPrefab;
    [SerializeField]
    private GameObject gridCellCorrectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool CheckInBounds(Vector2Int[] positions, Vector2Int spawnPos)
    {
        foreach(var cell in positions)
        {
            Vector2Int finalPos = cell + spawnPos;
            if (finalPos.x < 0 || finalPos.x >= width || finalPos.y < 0 || finalPos.y >= height)
            {
                return false;
            }
        }
        return true;
    }

    // TODO: Implement this function to adjust the spawn position of the shape if it took 100 attempts to find a position
    private void AdjustSpawnPosition()
    {

    }

    private void PlaceShapeOnGrid(PieceShape shape, int rotation)
    {
        int maxAttempts = 100;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2Int spawnPos = new Vector2Int(
                Random.Range(0, width),
                Random.Range(0, height)
            );
            
            Vector2Int[] positions = shape.rotations[rotation];
            if (CheckInBounds(positions, spawnPos))
            {
                Debug.Log("Random Spawn Position: " + spawnPos);
                foreach(var cell in positions)
                {
                    Vector2Int finalPos = cell + spawnPos;

                    //activeGrid[finalPos.x, finalPos.y] = true;
                    activeGrid[finalPos.x, finalPos.y].validPosition = true;
                }
                return;
            }            

            if (attempt == (maxAttempts-1))
            {
                Debug.Log("No correct spawn position");
                
                // Create function to adjust until find correct spot for shape
                AdjustSpawnPosition();

                return;
            }
        }        
    }

    private void GenerateGrid()
    {
        // Generate empty grid
        activeGrid = new Tile_Data[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile_Data tile = new Tile_Data();
                tile.gridPosition = new Vector2Int(x,y);
                tile.validPosition = false;
                tile.isNoiseTile = false;
                tile.tileObject = null;

                activeGrid[x,y] = tile;
            }
        }

        // Generate Shape and its list of rotations
        var shapes = PieceDatabase.shapes;
        PieceShape shape = shapes[Random.Range(0, shapes.Length)];
        GenerateRotationsForShape(shape);

        // Get random rotation
        int rotation = Random.Range(0,4);
        PrintShape(shape, rotation);

        // Place the shape onto the grid in a random spot
        PlaceShapeOnGrid(shape, rotation);
        PrintGrid();

        // Generate random shapes and noise to the grid

        // Generate 3D grid in the scene
        Create3DGrid();

        // Check if there exists a same shape in the grid, if there is then regenerate random noise
        GridCheck(shape, rotation);

    }

    public void GenerateRandomNoise(Tile_Data tile)
    {
        float noiseDensity = 0.35f;
        float val = Random.value;
        if (val <= noiseDensity)
        {
            // Add Correct Prefab
            tile.isNoiseTile = true;
            tile.tileObject = Instantiate(gridCellCorrectPrefab, new Vector3(tile.gridPosition.x * gridSpace, 0, tile.gridPosition.y * gridSpace), Quaternion.identity);
        }
        else
        {
            // Add blank Prefab
            tile.tileObject = Instantiate(gridCellPrefab, new Vector3(tile.gridPosition.x * gridSpace, 0, tile.gridPosition.y * gridSpace), Quaternion.identity);
        }
    }

    public void Create3DGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (activeGrid[x,y].validPosition)
                {
                    
                    activeGrid[x,y].tileObject = Instantiate(gridCellCorrectPrefab, new Vector3(x * gridSpace, 0, y * gridSpace), Quaternion.identity);
                }
                else
                {
                    GenerateRandomNoise(activeGrid[x,y]);
                }
            }
        }
    }

    // TODO: Check if there exists the same shape since it shouldnt exist; only one shape can exist at a time
    public void GridCheck(PieceShape shape, int rotation)
    {

    }

    public void PrintGrid()
    {        
        
        for(int y = height - 1; y >= 0; y--)
        {
            string row = "";
            for(int x = 0; x < width; x++)
            {
                row += activeGrid[x, y].validPosition ? "X " : ". ";
            }
            Debug.Log(row);
        }
    }

    // Prints a shape and its rotations
    private void PrintShape(PieceShape shape, int randomRotation = 4)
    {
        Debug.Log("Shape Name:" + shape.shapeName);
        for (int rotation = 0; rotation < shape.rotations.Count; rotation++)
        {
            // Debug.Log("Rotation: " + rotation);
            // for (int cells = 0; cells < shape.rotations[rotation].Length; cells++)
            // {                
            //     Debug.Log("Cell: " + shape.rotations[rotation][cells]);
            // }
            
            if (rotation == randomRotation)
            {
                for (int y = 6 - 1; y >= 0; y--)
                {
                    string row = "";
                    for (int x = 0; x < 6; x++)
                    {
                        Vector2Int temp = new Vector2Int(x,y);
                        if (shape.rotations[rotation].Contains(temp))
                        {
                            row += "X";
                        }
                        else
                        {
                            row += ". ";
                        }
                        
                    }
                    Debug.Log(row);
                }
            }
            
        }
    }

    // Once rotation is complete, there are negative numbers, so need to normalize to be around 0,0
    private Vector2Int[] NormalizeCell(Vector2Int[] cells)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        // Find the least number in each cell
        foreach (var c in cells)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
        }

        Vector2Int[] normalized = new Vector2Int[cells.Length];

        // Apply the change to every cell, so subtract the least number to every cell so that the negative numbers normalize to 0
        for (int i = 0; i < cells.Length; i++)
        {
            normalized[i] = new Vector2Int(
                cells[i].x - minX,
                cells[i].y - minY
            );
        }

        return normalized;
    }

    // Rotate all cells counter clockwise (x, y) -> (-y, x) by 90 degrees
    private Vector2Int[] RotateShape(Vector2Int[] cells)
    {
        Vector2Int[] rotated = new Vector2Int[cells.Length];

        // Loop through all cells that make up a shape and change the points in the shape by 90 degrees
        for (int i = 0; i < cells.Length; i++)
        {
            Vector2Int cell = cells[i];
            rotated[i] = new Vector2Int(-cell.y, cell.x);
        }

        // Normalize each cell
        for (int i = 0; i < rotated.Length; i++)
        {
            rotated = NormalizeCell(rotated);
        }

        return rotated;
    }

    // Rotates the shape 4 times and adds to the rotations List in PieceShape
    private void GenerateRotationsForShape(PieceShape shape)
    {
        List<Vector2Int[]> rotations = new List<Vector2Int[]>();

        Vector2Int[] currentCells = shape.cells;

        // Rotate only 4 times
        for (int i = 0; i < 4; i++)
        {
            rotations.Add(currentCells);
            currentCells = RotateShape(currentCells);
        }

        shape.rotations = rotations;
    }
}
