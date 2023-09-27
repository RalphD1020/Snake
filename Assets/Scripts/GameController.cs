using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject tileCube;     // The prefab of the cube you want to instantiate
    public int width = 20;          // Number of cubes in the X axis
    public int height = 15;         // Number of cubes in the Z axis
    public float spacing = 1.1f;    // Space between each cube

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                Instantiate(tileCube, position, Quaternion.identity, this.transform);
            }
        }
    }
}