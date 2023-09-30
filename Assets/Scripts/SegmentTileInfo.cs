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
    
    public void UpdateTileFields(Tile targetTile, HashSet<Tile> unoccupiedTiles)
    {
        CurrentTile.IsOccupied = false;
        unoccupiedTiles.Add(CurrentTile);

        PrevTile = CurrentTile;
        CurrentTile = targetTile;
        CurrentTile.IsOccupied = true;
        
        CurrentTile = targetTile;
        
        if (unoccupiedTiles.Contains(CurrentTile))
        {
            unoccupiedTiles.Remove(CurrentTile);
        }
    }
}
