using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject _snake;
    private List<GameObject> snakeSegments;
    private GameObject _egg;
    private Tile _currentTile;
    private Tile _prevTile;
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) // Move forward
        {
            MoveSnake(_currentTile.X, _currentTile.Z + 1);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) // Move backward
        {
            MoveSnake(_currentTile.X, _currentTile.Z - 1);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Move left
        {
            MoveSnake(_currentTile.X - 1, _currentTile.Z);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Move right
        {
            MoveSnake(_currentTile.X + 1, _currentTile.Z);
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
                _currentTile = tileComponent;
            }
        }
    }
    
    private void InitializeAndSpawnSnakeAtCenter()
    {
        _currentTile = _board[(width - 1) / 2, (height - 1) / 2];
        _currentTile.IsOccupied = true;
        var tilePosition = _currentTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _snake = Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
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
        targetX = ValidateTargetIndex(0, targetX);
        targetZ = ValidateTargetIndex(1, targetZ);
        
        var targetTile = _board[targetX, targetZ];
        var tilePosition = targetTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _snake.transform.position = snakePosition;
        CheckPickupItem();
        UpdateCurrentTile(targetTile);
        
    }

    private void CheckPickupItem()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_snake.transform.position, 0); // 1f is the radius, adjust based on your needs
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

    private void UpdateCurrentTile(Tile targetTile)
    {
        _currentTile.IsOccupied = false;
        _currentTile = targetTile;
        _currentTile.IsOccupied = true;
    }

    private void FeedSnake()
    {
        _egg.SetActive(false);
    }
}