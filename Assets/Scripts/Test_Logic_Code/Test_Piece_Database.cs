using UnityEngine;

public static class PieceDatabase
{
    public static PieceShape[] shapes = new PieceShape[]
    {
        
        /*
            Z-piece

            .##
            ##.
        */
        new PieceShape
        {
            shapeName = "Z-piece",
            cells = new Vector2Int[]
            {
                new Vector2Int(0,0),
                new Vector2Int(1,0),
                new Vector2Int(1,1),
                new Vector2Int(2,1)
            }
        },

        /*
            L-piece

            ##. 
            #..
            #..
        */
        new PieceShape
        {
            shapeName = "L-piece",
            cells = new Vector2Int[]
            {
                new Vector2Int(0,0),
                new Vector2Int(0,1),
                new Vector2Int(0,2),
                new Vector2Int(1,2)
            }
        }
    };
}
