using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PieceShape
{
    public string shapeName;
    public Vector2Int[] cells;
    public List<Vector2Int[]> rotations;
}
