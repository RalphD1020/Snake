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
    
    public void UpdateTileFields(Tile targetTile, Dictionary<int, Tile> unoccupiedTiles)
    {
        var tileId = CurrentTile.TileId;
        CurrentTile.IsOccupied = false;
        unoccupiedTiles.TryAdd(tileId, CurrentTile);

        PrevTile = CurrentTile;
        CurrentTile = targetTile;
        CurrentTile.IsOccupied = true;

        tileId = CurrentTile.TileId;
        CurrentTile = targetTile;
        
        if (unoccupiedTiles.ContainsKey(tileId))
        {
            unoccupiedTiles.Remove(tileId);
        }
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
