using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject _snake;
    private GameObject _egg;
    private Tile currentTile;
    private Tile[,] _board;
    
    public GameObject cubePrefab;
    public GameObject eggPrefab;
    public GameObject snakePrefab;
    public int width = 24;          // Number of cubes in the X axis
    public int height = 16;         // Number of cubes in the Z axis
    public float spacing = 1.1f;    // Space between each cube
    
    // todo: keep reference to last cube in chain
    // todo: keep reference to previous position
    
    void Update()
    {
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

    private void MoveSnake(int targetX, int targetZ)
    {
        targetX = validateTargetIndex(0, targetX);
        targetZ = validateTargetIndex(1, targetZ);
        
        var targetTile = _board[targetX, targetZ];
        var tilePosition = targetTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _snake.transform.position = snakePosition;
        updateCurrentTile(targetTile);
        
    }

    private int validateTargetIndex(int dimension, int index)
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

    private void updateCurrentTile(Tile targetTile)
    {
        currentTile.IsOccupied = false;
        currentTile = targetTile;
        currentTile.IsOccupied = true;
    }

    public void FeedSnake()
    {
        // Implement what you want to happen in the GameController when an item is picked up
        Debug.Log("Item picked up!");
        placeEgg();
    }

    private void placeEgg()
    {
        var randomTile = _board[Random.Range(0, _board.GetLength(0)), Random.Range(0, _board.GetLength(1))];
        var tilePosition = randomTile.Position;
        var eggPosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _egg = Instantiate(eggPrefab, eggPosition, Quaternion.identity, transform);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FeedSnake();
        }
    }

    void Start()
    {
        InstantiateBoard();
        InitializeAndSpawnSnakeAtCenter();
        placeEgg();
    }

    void InstantiateBoard()
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
                currentTile = tileComponent;
            }
        }
    }
    
    private void InitializeAndSpawnSnakeAtCenter()
    {
        currentTile = _board[(width - 1) / 2, (height - 1) / 2];
        currentTile.IsOccupied = true;
        var tilePosition = currentTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _snake = Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
    }
}