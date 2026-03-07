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

    private bool[,] grid;

    void Start()
    {
        grid = new bool[width, height];
        Spawn_Piece();
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

    Vector2Int Rotate(Vector2Int cell, int rotation)
    {
        return cell;
    }

    public void Spawn_Piece()
    {
        int rotation = Random.Range(0,4);
        
        var shapes = PieceDatabase.shapes;
        PieceShape shape = shapes[Random.Range(0, shapes.Length)];

        Debug.Log(shape.shapeName);

        Vector2Int randomSpawnPos = new Vector2Int(
            Random.Range(0, width),
            Random.Range(0, height)
        );

        bool notValid = true;
        int checkCount = 0;
        while (notValid && checkCount < 1000)
        {
            int temp = 0;
            foreach (var cell in shape.cells)
            {            
                Vector2Int rotatedPosition = cell;
                Vector2Int finalPos = randomSpawnPos + rotatedPosition;

                Vector2Int val = Is_Inside_Grid(finalPos);
                if (val.x != 0 || val.y != 0)
                {
                    randomSpawnPos = new Vector2Int(randomSpawnPos.x, randomSpawnPos.y) + val;
                    break;
                }
                else
                {
                    temp++;
                }                

            }
            if (temp >= 4)
            {
                notValid = false;
            }
            checkCount++;
        }
        if (checkCount >= 50)
        {
            Debug.Log("Couldn't find a good spot");
        }

        foreach(var cell in shape.cells)
        {
            Vector2Int rotatedPosition = cell;
            Vector2Int finalPos = randomSpawnPos + rotatedPosition;
            
            Set_Tile(finalPos, true);
        }
        Print_Grid();
    }
}

/*
public void Spawn_Piece()
{
    // Pick random shape and rotation
    int rotation = Random.Range(0, 4);
    var shapes = PieceDatabase.shapes;
    PieceShape shape = shapes[Random.Range(0, shapes.Length)];
    Debug.Log("Spawning piece: " + shape.shapeName);

    // Try to find a valid spawn position
    Vector2Int spawnPos = FindValidSpawnPosition(shape, rotation, 1000);
    if (spawnPos == Vector2Int.zero)
    {
        Debug.Log("Couldn't find a good spot for " + shape.shapeName);
        return;
    }

    // Place the piece
    PlacePiece(shape, spawnPos, rotation);

    // Debug print
    Print_Grid();
}

private Vector2Int FindValidSpawnPosition(PieceShape shape, int rotation, int maxAttempts)
{
    for (int attempt = 0; attempt < maxAttempts; attempt++)
    {
        Vector2Int spawnPos = new Vector2Int(
            Random.Range(0, width),
            Random.Range(0, height)
        );

        if (IsValidPosition(shape, spawnPos, rotation, out Vector2Int adjustedPos))
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

private bool IsValidPosition(PieceShape shape, Vector2Int spawnPos, int rotation, out Vector2Int correction)
{
    correction = Vector2Int.zero;

    foreach (var cell in shape.cells)
    {
        Vector2Int rotatedCell = cell; // Add Rotate(cell, rotation) here if you implement rotation
        Vector2Int finalPos = spawnPos + rotatedCell;

        Vector2Int adjust = Is_Inside_Grid(finalPos); // returns Vector2Int offset if out of bounds
        if (adjust != Vector2Int.zero)
        {
            correction = adjust;
            return false; // Position invalid
        }

        if (IsOccupied(finalPos))
        {
            return false; // Position blocked
        }
    }

    return true; // All cells valid
}

private void PlacePiece(PieceShape shape, Vector2Int spawnPos, int rotation)
{
    foreach (var cell in shape.cells)
    {
        Vector2Int rotatedCell = cell; // Add Rotate(cell, rotation) here if needed
        Vector2Int finalPos = spawnPos + rotatedCell;

        Set_Tile(finalPos, true);
    }
}
*/
