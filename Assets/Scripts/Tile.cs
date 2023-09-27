using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector3 Position { get; set; }
    public int X { get; set; }
    public int Z { get; set; }

    public void Initialize(Vector3 position, int x, int z)
    {
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
