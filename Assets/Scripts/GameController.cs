using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int segmentId = 0;
    private List<SegmentTileInfo> snakeSegments;
    private Dictionary<int, SegmentTileInfo> segmentTileInfo;
    private GameObject _egg;
    private SegmentTileInfo snakeHeadSegmentTileInfo;
    private Tile[,] _board;
    
    public GameObject cubePrefab;
    public GameObject eggPrefab;
    public GameObject snakePrefab;
    public int width = 24;          // Number of cubes in the X axis
    public int height = 16;         // Number of cubes in the Z axis
    public float spacing = 1.1f;    // Space between each cube
    
    // todo: keep reference to last cube in chain
    // todo: keep reference to previous position

    private void Start()
    {
        InstantiateBoard();
        InitializeAndSpawnSnakeAtCenter();
        PlaceEgg();
    }
    
    private void Update()
    {
        var currentTile = snakeHeadSegmentTileInfo.CurrentTile; 
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // Move forward
        {
            MoveSnake(currentTile.X, currentTile.Z + 1);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // Move backward
        {
            MoveSnake(currentTile.X, currentTile.Z - 1);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Move left
        {
            MoveSnake(currentTile.X - 1, currentTile.Z);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Move right
        {
            MoveSnake(currentTile.X + 1, currentTile.Z);
        }
    }

    private void InstantiateBoard()
    {
        _board = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                var position = new Vector3(x * spacing, 0, z * spacing);
                var tileObject = Instantiate(cubePrefab, position, Quaternion.identity, this.transform);
                var tileComponent = tileObject.AddComponent<Tile>();
                tileComponent.Initialize(position, x, z);
                _board[x, z] = tileComponent;
            }
        }
    }
    
    private void InitializeAndSpawnSnakeAtCenter()
    {
        snakeSegments = new List<SegmentTileInfo>();
        var currentTile = _board[(width - 1) / 2, (height - 1) / 2];
        currentTile.IsOccupied = true;
        var tilePosition = currentTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        var snakeObject = Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
        var segmentTileInfoComponent = snakeObject.AddComponent<SegmentTileInfo>();
        segmentTileInfoComponent.Initialize(snakeObject, currentTile, currentTile);
        snakeSegments.Add(segmentTileInfoComponent);
        snakeHeadSegmentTileInfo = segmentTileInfoComponent;
    }
    
    private void PlaceEgg()
    {
        var randomTile = _board[Random.Range(0, _board.GetLength(0)), Random.Range(0, _board.GetLength(1))];
        var tilePosition = randomTile.Position;
        var eggPosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _egg = Instantiate(eggPrefab, eggPosition, Quaternion.identity, transform);
        _egg.SetActive(true);
    }

    private void MoveSnake(int targetX, int targetZ)
    {
        foreach (var segment in snakeSegments)
        {
            targetX = ValidateTargetIndex(0, targetX);
            targetZ = ValidateTargetIndex(1, targetZ);
        
            var targetTile = _board[targetX, targetZ];
            var tilePosition = targetTile.Position;
            var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
            segment.transform.position = snakePosition;
            CheckPickupItem();
            UpdateTileFields(segment, targetTile);
        }
        
    }

    private void CheckPickupItem()
    {
        Collider[] hitColliders = Physics.OverlapSphere(snakeSegments[0].transform.position, 0); // 1f is the radius, adjust based on your needs
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == _egg)
            {
                FeedSnake();
                PlaceEgg();
                return;
            }
        }
    }

    private int ValidateTargetIndex(int dimension, int index)
    {
        if (index >= _board.GetLength(dimension))
        {
            index = 0;
        }
        if (index < 0)
        {
            index += _board.GetLength(dimension);
        }

        return index;
    }

    private void UpdateTileFields(SegmentTileInfo tileInfo, Tile targetTile)
    {
        tileInfo.CurrentTile.IsOccupied = false;
        tileInfo.PrevTile = tileInfo.CurrentTile;
        tileInfo.CurrentTile = targetTile;
        tileInfo.CurrentTile.IsOccupied = true;
        tileInfo.CurrentTile = targetTile;
    }

    private void FeedSnake()
    {
        _egg.SetActive(false);
    }
}