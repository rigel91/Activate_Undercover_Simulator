using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Main_Logic_Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PieceShape shape = PieceDatabase.shapes[0];
        GenerateRotationsForShape(shape);
        PrintShape(shape);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Prints a shape and its rotations
    void PrintShape(PieceShape shape)
    {
        Debug.Log("Shape Name:" + shape.shapeName);
        for (int rotation = 0; rotation < shape.rotations.Count; rotation++)
        {
            // Debug.Log("Rotation: " + rotation);
            // for (int cells = 0; cells < shape.rotations[rotation].Length; cells++)
            // {                
            //     Debug.Log("Cell: " + shape.rotations[rotation][cells]);
            // }
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
                        row += ".";
                    }
                    
                }
                Debug.Log(row);
            }
        }
    }

    // Once rotation is complete, there are negative numbers, so need to normalize to be around 0,0
    public Vector2Int[] NormalizeCell(Vector2Int[] cells)
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
    public Vector2Int[] RotateShape(Vector2Int[] cells)
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
    public void GenerateRotationsForShape(PieceShape shape)
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
