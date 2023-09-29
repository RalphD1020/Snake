using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentTileInfo : MonoBehaviour
{
    private GameObject Segment { get; set; }
    public Tile CurrentTile { get; set; }
    public Tile PrevTile { get; set; }
    
    public void Initialize(GameObject segment, Tile currentTile, Tile prevTile)
    {
        Segment = segment;
        CurrentTile = currentTile;
        PrevTile = prevTile;
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
