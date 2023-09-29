using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum Direction {UP, DOWN, LEFT, RIGHT}
public class GameController : MonoBehaviour
{
    private int _score = 0;
    private float _moveInterval = 0.5f;
    private Direction _currentDirection;
    private Direction _lastDirection;
    private List<SegmentTileInfo> _snakeSegments;
    private GameObject _egg;
    private SegmentTileInfo _snakeHeadSegmentTileInfo;
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
        StartCoroutine(AutoMoveCoroutine());
    }
    
    private void Update()
    {
        Direction newDirection = _currentDirection;
        if (_lastDirection != Direction.DOWN && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))) // Move forward
        {
            newDirection = Direction.UP;
        }
        if (_lastDirection != Direction.UP && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))) // Move backward
        {
            newDirection = Direction.DOWN;
        }
        if (_lastDirection != Direction.RIGHT && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))) // Move left
        {
            newDirection = Direction.LEFT;
        }
        if (_lastDirection != Direction.LEFT && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))) // Move right
        {
            newDirection = Direction.RIGHT;
        }
        _currentDirection = newDirection;
    }
    
    private IEnumerator AutoMoveCoroutine()
    {
        while (true)
        {
            MoveSnakeInDirection();
            _lastDirection = _currentDirection; 
            yield return new WaitForSeconds(_moveInterval);
        }
    }

    private void MoveSnakeInDirection()
    {
        int targetX = _snakeSegments[0].CurrentTile.X;
        int targetZ = _snakeSegments[0].CurrentTile.Z;

        switch (_currentDirection)
        {
            case Direction.UP:
                targetZ++;
                break;
            case Direction.DOWN:
                targetZ--;
                break;
            case Direction.LEFT:
                targetX--;
                break;
            case Direction.RIGHT:
                targetX++;
                break;
        }
        MoveSnake(targetX, targetZ);
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
        _snakeSegments = new List<SegmentTileInfo>();
        var currentTile = _board[(width - 1) / 2, (height - 1) / 2];
        currentTile.IsOccupied = true;
        var tilePosition = currentTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        var snakeObject = Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
        var segmentTileInfoComponent = snakeObject.AddComponent<SegmentTileInfo>();
        segmentTileInfoComponent.Initialize(snakeObject, currentTile, currentTile);
        _snakeSegments.Add(segmentTileInfoComponent);
        _snakeHeadSegmentTileInfo = segmentTileInfoComponent;
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
        for (int i = 0; i < _snakeSegments.Count; i++)
        {
            SegmentTileInfo segment = _snakeSegments[i];
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
            segment.UpdateTileFields(targetTile);
            CheckPickupItem(segment);
        }
        
    }

    private void CheckPickupItem(SegmentTileInfo segment)
    {
        Collider[] hitColliders = Physics.OverlapSphere(_snakeSegments[0].transform.position, 0); // 1f is the radius, adjust based on your needs
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == _egg)
            {
                _score++;
                FeedSnake(segment.PrevTile);
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