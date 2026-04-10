using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PieceShape
{
    public string shapeName;

    public List<Vector2Int> rotationOffsets;

    public Vector2Int[] cells;
    public List<Vector2Int[]> rotations;
}
