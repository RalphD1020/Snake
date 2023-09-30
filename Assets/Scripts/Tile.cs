using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsOccupied { get; set; }
    public Vector3 Position { get; set; }
    public int X { get; set; }
    public int Z { get; set; }

    public void Initialize(Vector3 position, int x, int z)
    {
        IsOccupied = false;
        Position = position;
        X = x;
        Z = z;
    }
}
