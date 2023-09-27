using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject _snake;
    private Tile currentTile;
    private Tile[,] _board;
    
    public GameObject cubePrefab;
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
        if (targetX >= _board.GetLength(0))
        {
            targetX = 0;
        }
        if (targetX < 0)
        {
            targetX += _board.GetLength(0);
        }
        
        if (targetZ >= _board.GetLength(1))
        {
            targetZ = 0;
        }
        if (targetZ < 0)
        {
            targetZ += _board.GetLength(1);
        }
        
        var targetTile = _board[targetX, targetZ];
        var tilePosition = targetTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _snake.transform.position = snakePosition;
        currentTile = targetTile;
    }

    void Start()
    {
        InstantiateBoard();
        InitializeAndSpawnSnakeAtCenter();
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
        var tilePosition = currentTile.Position;
        var snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _snake = Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
    }
}