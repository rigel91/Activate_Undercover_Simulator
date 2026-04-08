using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Tile_Data
{
    public Vector2Int gridPosition;
    public bool validPosition;
    public bool isNoiseTile;
    public GameObject tileObject;
}
