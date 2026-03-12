using UnityEngine;
using Debug = UnityEngine.Debug;

public class Test_Logic : MonoBehaviour
{
    /*

        Undercover room dimensions
        Looks like one shape in the middle then 6 smaller grids that are in a 6x6 grid separated by 4 tiles

    */

    public int width = 6;
    public int height = 6;

    private float gridSpaceSize = 6f;

    [SerializeField]
    private GameObject gridCellPrefab;
    [SerializeField]
    private GameObject gridCellCorrectPrefab;

    private bool[,] grid;
    private GameObject[,] gameGrid;

    void Start()
    {
        grid = new bool[width, height];
        Spawn_Piece();
        //Create_Basic_Grid();
        Generate_Grid();
    }

    private void Create_Basic_Grid()
    {
        gameGrid = new GameObject[width, height];

        if (gridCellPrefab == null)
        {
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gameGrid[x,y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);                
                
                //Vector3 spawnPosition = new Vector3(x * gridSpaceSize, 0f, y * gridSpaceSize);
                //Instantiate(gridCellPrefab, spawnPosition, Quaternion.identity, this.transform);
                
                gameGrid[x,y].transform.parent = transform;
                gameGrid[x,y].gameObject.name = "Grid Space " + x.ToString() + ", " + y.ToString();
            }
        }
    }

    public void Print_Grid()
    {        
        
        for(int y = height - 1; y >= 0; y--)
        {
            string row = "";
            for(int x = 0; x < width; x++)
            {
                row += grid[x, y] ? "X " : ". ";
            }
            Debug.Log(row);
        }
    }

    public Vector2Int Is_Inside_Grid(Vector2Int pos)
    {
        //return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;

        if (pos.x < 0)
        {
            return new Vector2Int(1,0);
        }
        else if (pos.y < 0)
        {
            return new Vector2Int(0,1);
        }
        else if (pos.x >= width)
        {
            return new Vector2Int(-1, 0);
        }
        else if (pos.y >= height)
        {
            return new Vector2Int(0, -1);
        }
        else
        {
            return new Vector2Int(0,0);
        }

    }

    public bool Is_Occupied(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public void Set_Tile(Vector2Int pos, bool value)
    {
        grid[pos.x, pos.y] = value;
    }

    private Vector2Int Rotate(Vector2Int cell, int rotation)
    {
        rotation = rotation % 4;

        switch (rotation)
        {
            case 0: // 0
                return cell;
            
            case 1: // 90
                return new Vector2Int(cell.y, -cell.x);
            
            case 2: //180
                return new Vector2Int(-cell.x, -cell.y);
            
            case 3: // 270
                return new Vector2Int(-cell.y, cell.x);
            
            default:
                return cell;
        }
    }

    private bool Is_Valid_Position(PieceShape shape, Vector2Int spawnPos, out Vector2Int correction)
    {
        correction = Vector2Int.zero;

        foreach (var cell in shape.cells)
        {
            Vector2Int finalPos = spawnPos;

            Vector2Int adjust = Is_Inside_Grid(finalPos); // returns Vector2Int offset if out of bounds
            if (adjust != Vector2Int.zero)
            {
                correction = adjust;
                return false; // Position invalid
            }

            if (Is_Occupied(finalPos))
            {
                return false; // Position blocked
            }
        }

        return true; // All cells valid
    }

    private Vector2Int Find_Valid_Spawn_Position(PieceShape shape, int maxAttempts)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2Int spawnPos = new Vector2Int(
                Random.Range(0, width),
                Random.Range(0, height)
            );

            if (Is_Valid_Position(shape, spawnPos, out Vector2Int adjustedPos))
            {
                return spawnPos; // Valid spawn
            }
            else if (adjustedPos != Vector2Int.zero)
            {
                // Adjust the spawn position based on feedback from Is_Inside_Grid
                spawnPos += adjustedPos;
            }
        }

        // Could not find a valid spot
        return Vector2Int.zero;
    }

    public void Spawn_Piece()
    {
        // Pick random shape, Piece and rotation
        int rotation = Random.Range(0, 4);
        Debug.Log(rotation * 90);
        var shapes = PieceDatabase.shapes;
        PieceShape shape = shapes[Random.Range(0, shapes.Length)];
        Debug.Log("Spawning piece: " + shape.shapeName);

        PieceShape newShape = shape.Clone();
        for(int x = 0; x < shape.cells.Length; x++)
        {
            newShape.cells[x] = Rotate(shape.cells[x], rotation);
        }

        foreach(var cell in newShape.cells)
        {
            Debug.Log(cell);
        }

        // Try to find a valid spawn position
        Vector2Int spawnPos = Find_Valid_Spawn_Position(newShape, 100000);
        if (spawnPos == Vector2Int.zero)
        {
            spawnPos = new Vector2Int(width/2, height/2);
            if (Is_Valid_Position(newShape, spawnPos, out Vector2Int adjustedPos))
            {
                spawnPos += adjustedPos;
                Debug.Log("Couldn't find a good spot for " + newShape.shapeName + " Defaulting spawn... " + spawnPos);
            }                        
        }

        Debug.Log(spawnPos);

        // Place the piece
        Place_Piece(newShape, spawnPos, rotation);

        // Debug print
        Print_Grid();
    }

    private void Place_Piece(PieceShape shape, Vector2Int spawnPos, int rotation)
    {
        foreach (var cell in shape.cells)
        {
            Vector2Int rotatedCell = cell; // Add Rotate(cell, rotation) here if needed
            Vector2Int finalPos = spawnPos + rotatedCell;

            //print(finalPos);
            Set_Tile(finalPos, true);
        }
    }

    private void Generate_Grid()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x,y])
                {
                    Instantiate(gridCellCorrectPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
                }
                else
                {
                    Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
                }
            }
        }
    }

}