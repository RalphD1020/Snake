using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject snakePrefab;
    public Snake snake;
    public int width = 24;          // Number of cubes in the X axis
    public int height = 16;         // Number of cubes in the Z axis
    public float spacing = 1.1f;    // Space between each cube

    private Tile[,] _board;
    
    // todo: to update the positions cardinally, do x+/-1 * spacing or z+/-1 * spacing
    // todo: keep reference to last cube in chain
    // todo: keep reference to previous position
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
                tileComponent.Initialize(position);
                _board[x, z] = tileComponent;
            }
        }
    }
    
    private void InitializeAndSpawnSnakeAtCenter()
    {
        Vector3 tilePosition = _board[(width - 1)/2, (height - 1)/2].Position;
        Vector3 snakePosition = new Vector3(tilePosition.x, 1, tilePosition.z);
        Instantiate(snakePrefab, snakePosition, Quaternion.identity, transform);
        snake.Initialize(snakePosition);
    }
}