using UnityEngine;

[System.Serializable]
public class PieceShape
{
    public Vector2Int[] cells;
    public string shapeName;

    public PieceShape Clone()
    {
        return new PieceShape
        {
            shapeName = shapeName,
            cells = (Vector2Int[])cells.Clone()
        };
    }
}
