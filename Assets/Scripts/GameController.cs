using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject _snake;
    private GameObject _egg;
    private Tile currentTile;
    private Tile[,] _board;

    public static GameController Instance;
    public GameObject cubePrefab;
    public GameObject eggPrefab;
    public GameObject snakePrefab;
    public int width = 24;          // Number of cubes in the X axis
    public int height = 16;         // Number of cubes in the Z axis
    public float spacing = 1.1f;    // Space between each cube
    
    // todo: keep reference to last cube in chain
    // todo: keep reference to previous position
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  // Set the instance to this instance of the GameController
            DontDestroyOnLoad(gameObject);  // Optionally ensure that the GameController persists between scene changes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);  // Ensure there's only one instance of GameController
        }
    }
    
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
        CheckPickupItem();
        updateCurrentTile(targetTile);
        
    }

    private void CheckPickupItem()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_snake.transform.position, 0); // 1f is the radius, adjust based on your needs
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == _egg)
            {
                FeedSnake();
                return;
            }
        }
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
        _egg.SetActive(false);
        // todo: Instantiate new snake piece and make follow first prefab
        placeEgg();
    }

    private void placeEgg()
    {
        var randomTile = _board[Random.Range(0, _board.GetLength(0)), Random.Range(0, _board.GetLength(1))];
        var tilePosition = randomTile.Position;
        var eggPosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        _egg = Instantiate(eggPrefab, eggPosition, Quaternion.identity, transform);
        _egg.SetActive(true);
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