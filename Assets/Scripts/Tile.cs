using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int TileId;
    public bool IsOccupied { get; set; }
    public Vector3 Position { get; set; }
    public int X { get; set; }
    public int Z { get; set; }

    public void Initialize(int tileId, Vector3 position, int x, int z)
    {
        TileId = tileId;
        IsOccupied = false;
        Position = position;
        X = x;
        Z = z;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
