using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum Direction {Up, Down, Left, Right}
public class GameController : MonoBehaviour
{
    private int _score;
    private Direction _currentDirection;
    private Direction _lastDirection;
    private HashSet<Tile> _unoccupiedTiles;
    private List<SegmentTileInfo> _snakeSegments;
    private GameObject _egg;
    private Tile[,] _board;

    public int width = 24;          // Number of cubes in the X axis
    public int height = 16;         // Number of cubes in the Z axis
    public float spacing = 1.1f;    // Space between each cube
    public float moveInterval = 0.1f;
    public GameObject cubePrefab;
    public GameObject eggPrefab;
    public GameObject snakePrefab;

    private void Start()
    {
        InstantiateBoard();
        InitializeAndSpawnSnakeAtCenter();
        PlaceEgg();
        StartCoroutine(AutoMoveCoroutine());
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_lastDirection != Direction.Down) _currentDirection = Direction.Up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_lastDirection != Direction.Up) _currentDirection = Direction.Down;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_lastDirection != Direction.Right) _currentDirection = Direction.Left;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_lastDirection != Direction.Left) _currentDirection = Direction.Right;
        }
    }
    
    private IEnumerator AutoMoveCoroutine()
    {
        while (true)
        {
            MoveSnakeInDirection();
            _lastDirection = _currentDirection; 
            yield return new WaitForSeconds(moveInterval);
        }
    }

    private void MoveSnakeInDirection()
        
    {
        var targetX = _snakeSegments[0].CurrentTile.X;
        var targetZ = _snakeSegments[0].CurrentTile.Z;

        switch (_currentDirection)
        {
            case Direction.Up:
                targetZ++;
                break;
            case Direction.Down:
                targetZ--;
                break;
            case Direction.Left:
                targetX--;
                break;
            case Direction.Right:
                targetX++;
                break;
        }
        MoveSnake(targetX, targetZ);
    }

    private void InstantiateBoard()
    {
        _board = new Tile[width, height];
        _unoccupiedTiles = new HashSet<Tile>();

        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < height; z++)
            {
                var position = new Vector3(x * spacing, 0, z * spacing);
                var tileObject = Instantiate(cubePrefab, position, Quaternion.identity, this.transform);
                var tileComponent = tileObject.AddComponent<Tile>();
                tileComponent.Initialize(position, x, z);
                _board[x, z] = tileComponent;
                _unoccupiedTiles.Add(tileComponent);
            }
        }
    }
    
    private void InitializeAndSpawnSnakeAtCenter()
    {
        _snakeSegments = new List<SegmentTileInfo>();
        var currentTile = _board[(width - 1) / 2, (height - 1) / 2];
        currentTile.IsOccupied = true;
        var tilePosition = currentTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        var snakeObject = Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
        var segmentTileInfoComponent = snakeObject.AddComponent<SegmentTileInfo>();
        segmentTileInfoComponent.Initialize(snakeObject, currentTile, currentTile);
        _snakeSegments.Add(segmentTileInfoComponent);
    }
    
    private void PlaceEgg()
    {
        var tilePosition =  GetRandomTile().Position;
        var eggPosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _egg = Instantiate(eggPrefab, eggPosition, Quaternion.identity, transform);
        _egg.SetActive(true);
    }
    
    private Tile GetRandomTile()
    {
        int count = _unoccupiedTiles.Count;
        return _unoccupiedTiles.ElementAt(Random.Range(0, count));
    }

    private void MoveSnake(int targetX, int targetZ)
    {
        for (var i = 0; i < _snakeSegments.Count; i++)
        {
            var segment = _snakeSegments[i];
            Tile targetTile;
            if (i == 0)
            {
                targetX = ValidateTargetIndex(0, targetX);
                targetZ = ValidateTargetIndex(1, targetZ);
                targetTile = _board[targetX, targetZ];
            }
            else
            {
                targetTile = _snakeSegments[i - 1].PrevTile;
            }

            if (targetTile.IsOccupied)
            {
                Debug.Log("Collision! Final Score: " + _score);
                EditorApplication.isPlaying = false;
            }
            var tilePosition = targetTile.Position;
            var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
            segment.transform.position = snakePosition;
            segment.UpdateTileFields(targetTile, _unoccupiedTiles);
            CheckPickupItem(segment);
        }
        
    }

    private void CheckPickupItem(SegmentTileInfo segment)
    {
        var hitColliders = Physics.OverlapSphere(_snakeSegments[0].transform.position, 0); // 1f is the radius, adjust based on your needs
        if (hitColliders.Any(hitCollider => hitCollider.gameObject == _egg))
        {
            _score++;
            FeedSnake(segment.PrevTile);
            PlaceEgg();
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

    private void FeedSnake(Tile prevTile)
    {
        _egg.SetActive(false);
        var tilePosition = prevTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        var snakeObject = Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
        var segmentTileInfoComponent = snakeObject.AddComponent<SegmentTileInfo>();
        segmentTileInfoComponent.Initialize(snakeObject, prevTile, prevTile);
        _snakeSegments.Add(segmentTileInfoComponent);
    }
}